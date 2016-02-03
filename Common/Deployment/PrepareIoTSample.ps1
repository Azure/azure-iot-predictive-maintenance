Param(
    [Parameter(Mandatory=$True,Position=0)]
    $environmentName,
    [Parameter(Mandatory=$True,Position=1)]
    $configuration
    )
    
# Check version

$module = Get-Module -ListAvailable | Where-Object{ $_.Name -eq 'Azure' }
$moduleNumber = $module.Version
$expected = @{}
$expected.Major = 1
$expected.Minor = 0
$expected.Build = 3
if ($moduleNumber.Major -lt $expected.Major -or $moduleNumber.Minor -lt $expected.Minor -or $moduleNumber.Build -lt $expected.Build)
{
    Write-Host "This script Azure Cmdlets requires $($expected.Major).$($expected.Minor).$($expected.Build)"
    Write-Host "Found $($moduleNumber.Major).$($moduleNumber.Minor).$($moduleNumber.Build) installed."
    Write-Host "Try updating and running again."
    return
}
if ($moduleNumber.Major -gt $expected.Major -or $moduleNumber.Minor -gt $expected.Minor -or $moduleNumber.Build -gt $expected.Build)
{
    Write-Warning "This script Azure Cmdlets was tested with $($expected.Major).$($expected.Minor).$($expected.Build)"
    Write-Warning "Found $($moduleNumber.Major).$($moduleNumber.Minor).$($moduleNumber.Build) installed; continuing, but errors might occur"
}

# Initialize library
. "$(Split-Path $MyInvocation.MyCommand.Path)\DeploymentLib.ps1"
Clear-DnsClientCache

# Sets Azure Accounts, Region, Name validation, and AAD application
InitializeEnvironment $environmentName

# Set environment specific variables 
$suitename = "LocalPM"
$suiteType = "LocalPredictiveMaintenance"
$deploymentTemplatePath = "$(Split-Path $MyInvocation.MyCommand.Path)\LocalPredictiveMaintenance.json"
$global:site = "https://localhost:44305/"
$global:appName = "iotsuite"
$cloudDeploy = $false
$projectRoot = Join-Path $PSScriptRoot "..\.." -Resolve

if ($environmentName -ne "local")
{
    $suiteName = $environmentName
    $suiteType = "PredictiveMaintenance"
    $deploymentTemplatePath = "$(Split-Path $MyInvocation.MyCommand.Path)\PredictiveMaintenance.json"
    $global:site = "https://{0}.azurewebsites.net/" -f $environmentName
    $cloudDeploy = $true
}
$resourceGroupName = (GetResourceGroup -Name $suiteName -Type $suiteType).ResourceGroupName
$storageAccount = GetAzureStorageAccount $suiteName $resourceGroupName
$iotHubName = GetAzureIotHubName $suitename $resourceGroupName
$sevicebusName = GetAzureServicebusName $suitename $resourceGroupName
$simulatorDataFileName = "data.csv"

# Setup AAD for webservice
UpdateResourceGroupState $resourceGroupName ProvisionAAD
$global:AADTenant = GetOrSetEnvSetting "AADTenant" "GetAADTenant"
UpdateEnvSetting "AADMetadataAddress" ("https://login.windows.net/{0}/FederationMetadata/2007-06/FederationMetadata.xml" -f $global:AADTenant)
UpdateEnvSetting "AADAudience" ($global:site + $global:appName)
UpdateEnvSetting "AADRealm" ($global:site + $global:appName)

# Provision Machine Learning workspace
$experimentName = "Remaining Useful Life [Predictive Exp.]"
$machineLearningService = ProvisionML $suiteName $resourceGroupName $experimentName
UpdateEnvSetting "MLApiUrl" $machineLearningService.ApiLocation
UpdateEnvSetting "MLApiKey" $machineLearningService.PrimaryKey
UpdateEnvSetting "MLHelpUrl" $machineLearningService.HelpLocation

# Deploy via Template
UpdateResourceGroupState $resourceGroupName ProvisionAzure
$params = @{ `
    suiteName=$suitename; `
    storageName=$($storageAccount.StorageAccountName); `
    iotHubName=$iotHubName; `
    sbName=$sevicebusName}

Write-Host "Suite name: $suitename"
Write-Host "Storage Name: $($storageAccount.StorageAccountName)"
Write-Host "IotHub Name: $iotHubName"
Write-Host "Servicebus Name: $sevicebusName"
Write-Host "AAD Tenant: $($global:AADTenant)"
Write-Host "ResourceGroup Name: $resourceGroupName"
Write-Host "Deployment template path: $deploymentTemplatePath"

# Upload WebPackages
if ($cloudDeploy)
{
    $webPackage = UploadFile ("$projectRoot\Web\obj\{0}\Package\Web.zip" -f $configuration) $storageAccount.StorageAccountName $resourceGroupName "WebDeploy" $true
    FixWebJobZip ("$projectRoot\WebJobHost\obj\{0}\Package\WebJobHost.zip" -f $configuration)
    $webJobPackage = UploadFile ("$projectRoot\WebJobHost\obj\{0}\Package\WebJobHost.zip" -f $configuration) $storageAccount.StorageAccountName $resourceGroupName "WebDeploy" $true
    $params += @{ `
        aadTenant=$($global:AADTenant); `
        packageUri=$webPackage; `
        webJobPackageUri=$webJobPackage; `
        simulatorDataFileName=$simulatorDataFileName; `
        mlApiUrl=$machineLearningService.ApiLocation; `
        mlApiKey=$machineLearningService.PrimaryKey}
}

# Upload simulator data
UploadFile "$projectRoot\Simulator.WebJob\Engine\Data\$simulatorDataFileName" $storageAccount.StorageAccountName $resourceGroupName "simulatordata" $false

# Stream analytics does not auto stop, and if already exists should be set to LastOutputEventTime to not lose data
if (StopExistingStreamAnalyticsJobs $resourceGroupName)
{
    $params += @{asaStartBehavior='LastOutputEventTime'}
}

Write-Host "Provisioning resources, if this is the first time, this operation can take up 10 minutes..."
$result = New-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $deploymentTemplatePath -TemplateParameterObject $params -Verbose

if ($result.ProvisioningState -ne "Succeeded")
{
    UpdateResourceGroupState $resourceGroupName Failed
    throw "Provisioning failed"
}

# Set Config file variables
UpdateResourceGroupState $resourceGroupName Complete
UpdateEnvSetting "ServiceStoreAccountName" $storageAccount.StorageAccountName
UpdateEnvSetting "ServiceStoreAccountConnectionString" $result.Outputs['storageConnectionString'].Value
UpdateEnvSetting "ServiceSBName" $sevicebusName
UpdateEnvSetting "ServiceSBConnectionString" $result.Outputs['ehConnectionString'].Value
UpdateEnvSetting "ServiceEHName" $result.Outputs['ehDataName'].Value
UpdateEnvSetting "IotHubName" $result.Outputs['iotHubHostName'].Value
UpdateEnvSetting "IotHubConnectionString" $result.Outputs['iotHubConnectionString'].Value
UpdateEnvSetting "DeviceTableName" "DeviceList"
UpdateEnvSetting "SimulatorDataFileName" $simulatorDataFileName

Write-Host ("Provisioning and deployment completed successfully, see {0}.config.user for deployment values" -f $environmentName)

if ($environmentName -ne "local")
{
    $maxSleep = 40
    $webEndpoint = "{0}.azurewebsites.net" -f $environmentName
    if (!(HostEntryExists $webEndpoint))
    {
        Write-Host "Waiting for website url to resolve." -NoNewline
        while (!(HostEntryExists $webEndpoint))
        {
            Write-Host "." -NoNewline
            Clear-DnsClientCache
            if ($maxSleep-- -le 0)
            {
                Write-Host
                Write-Warning ("website unable to resolve {0}, please wait and try again in 15 minutes" -f $global:site)
                break
            }
            sleep 3
        }
        Write-Host
    }
    if (HostEntryExists $webEndpoint)
    {
        start $global:site
    }
}