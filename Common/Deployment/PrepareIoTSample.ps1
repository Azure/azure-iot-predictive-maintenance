Param(
    [Parameter(Mandatory=$True,Position=0)]
    $environmentName,
    [Parameter(Mandatory=$True,Position=1)]
    $configuration
    )

# Initialize library
$environmentName = $environmentName.ToLowerInvariant()
. "$(Split-Path $MyInvocation.MyCommand.Path)\DeploymentLib.ps1"
Switch-AzureMode AzureResourceManager
Clear-DnsClientCache

# Sets Azure Accounts, Region, Name validation, and AAD application
InitializeEnvironment $environmentName

# Set environment specific variables 
$suitename = "LocalPredictiveMaintenance"
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
    #[string]$branch = "$(git symbolic-ref --short -q HEAD)"
    $cloudDeploy = $true
}
$resourceGroupName = (GetResourceGroup -Name $suiteName -Type $suiteType).ResourceGroupName
$storageAccount = GetAzureStorageAccount $suiteName $resourceGroupName
$iotHubName = GetAzureIotHubName $suitename $resourceGroupName
$simulatorDataContainer = "simulatordata"
$simulatorDataFileName = "data.csv"

# Deploy via Template
UpdateResourceGroupState $resourceGroupName ProvisionAzure
$params = @{ `
    suiteName=$suitename; `
    storageName=$($storageAccount.Name); `
    iotHubName=$iotHubName}

Write-Host "Suite name: $suitename"
Write-Host "Storage Name: $($storageAccount.Name)"
Write-Host "IotHub Name: $iotHubName"
Write-Host "ResourceGroup Name: $resourceGroupName"
Write-Host "Deployment template path: $deploymentTemplatePath"

# Upload WebPackages
if ($cloudDeploy)
{
    # $webPackage = UploadFile ("$projectRoot\DeviceAdministration\Web\obj\{0}\Package\Web.zip" -f $configuration) $storageAccount.Name $resourceGroupName "WebDeploy" $true
    # $params += @{packageUri=$webPackage}
    FixWebJobZip ("$projectRoot\WebJobHost\obj\{0}\Package\WebJobHost.zip" -f $configuration)
    $webJobPackage = UploadFile ("$projectRoot\WebJobHost\obj\{0}\Package\WebJobHost.zip" -f $configuration) $storageAccount.Name $resourceGroupName "WebDeploy" $true
    $params += @{ `
        webJobPackageUri=$webJobPackage; `
        simulatorDataContainer=$simulatorDataContainer; `
        simulatorDataFileName=$simulatorDataFileName}
}

# Upload simulator data
UploadFile "$projectRoot\Simulator.WebJob\Engine\Data\$simulatorDataFileName" $storageAccount.Name $resourceGroupName $simulatorDataContainer $false

# Stream analytics does not auto stop, and requires a start time for both create and update as well as stop if already exists
[string]$startTime = (get-date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
$null = StopExistingStreamAnalyticsJobs $resourceGroupName
$params += @{asaStartBehavior='CustomTime'}
$params += @{asaStartTime=$startTime}

Write-Host "Provisioning resources, if this is the first time, this operation can take up 10 minutes..."
$result = New-AzureResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile $deploymentTemplatePath -TemplateParameterObject $params -Verbose

if ($result.ProvisioningState -ne "Succeeded")
{
    UpdateResourceGroupState $resourceGroupName Failed
    throw "Provisioing failed"
}

# Set Config file variables
UpdateResourceGroupState $resourceGroupName Complete
UpdateEnvSetting "ServiceStoreAccountName" $storageAccount.Name
UpdateEnvSetting "ServiceStoreAccountConnectionString" $result.Outputs['storageConnectionString'].Value
UpdateEnvSetting "IotHubName" $result.Outputs['iotHubHostName'].Value
UpdateEnvSetting "IotHubConnectionString" $result.Outputs['iotHubConnectionString'].Value
UpdateEnvSetting "DeviceTableName" "DeviceList"
UpdateEnvSetting "SimulatorDataFileName" $simulatorDataFileName
UpdateEnvSetting "SimulatorDataContainer" $simulatorDataContainer

Write-Host ("Provisioning and deployment completed successfully, see {0}.config.user for deployment values" -f $environmentName)