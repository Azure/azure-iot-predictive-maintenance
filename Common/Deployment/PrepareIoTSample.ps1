Param(
    [Parameter(Mandatory=$True,Position=0)]
    $environmentName,
    [Parameter(Mandatory=$True,Position=1)]
    $configuration,
    [Parameter(Mandatory=$False,Position=2)]
    $azureEnvironmentName = "AzureCloud"
    )

# Initialize Azure Cloud Environment
switch($azureEnvironmentName)
{
    "AzureCloud" {
        if ((Get-AzureEnvironment AzureCloud) -eq $null)
        {
            Add-AzureEnvironment –Name AzureCloud -EnableAdfsAuthentication $False -ActiveDirectoryServiceEndpointResourceId https://management.core.windows.net/ -GalleryUrl https://gallery.azure.com/ -ServiceManagementUrl https://management.core.windows.net/ -SqlDatabaseDnsSuffix .database.windows.net -StorageEndpointSuffix core.windows.net -ActiveDirectoryAuthority https://login.microsoftonline.com/ -GraphUrl https://graph.windows.net/ -trafficManagerDnsSuffix trafficmanager.net -AzureKeyVaultDnsSuffix vault.azure.net -AzureKeyVaultServiceEndpointResourceId https://vault.azure.net -ResourceManagerUrl https://management.azure.com/ -ManagementPortalUrl http://go.microsoft.com/fwlink/?LinkId=254433
        }

        if ((Get-AzureRMEnvironment AzureCloud) -eq $null)
        {
            Add-AzureRMEnvironment –Name AzureCloud -EnableAdfsAuthentication $False -ActiveDirectoryServiceEndpointResourceId https://management.core.windows.net/ -GalleryUrl https://gallery.azure.com/ -ServiceManagementUrl https://management.core.windows.net/ -SqlDatabaseDnsSuffix .database.windows.net -StorageEndpointSuffix core.windows.net -ActiveDirectoryAuthority https://login.microsoftonline.com/ -GraphUrl https://graph.windows.net/ -trafficManagerDnsSuffix trafficmanager.net -AzureKeyVaultDnsSuffix vault.azure.net -AzureKeyVaultServiceEndpointResourceId https://vault.azure.net -ResourceManagerUrl https://management.azure.com/ -ManagementPortalUrl http://go.microsoft.com/fwlink/?LinkId=254433
        }

        $global:iotHubSuffix = "azure-devices.net"
        $global:docdbSuffix = "documents.azure.com"
        $global:eventhubSuffix = "servicebus.windows.net"
        $global:websiteSuffix = "azurewebsites.net"
        $global:studioApiUrl = "https://studioapi.azureml.net/"
        $global:mlManagement = "https://management.azureml.net"
        $global:locations = @("East US", "North Europe", "East Asia", "West US", "West Europe", "Southeast Asia", "Japan East", "Japan West", "Australia East", "Australia Southeast")
    }
    "AzureGermanyCloud" {
        if ((Get-AzureEnvironment AzureGermanyCloud) -eq $null)
        {
            Add-AzureEnvironment –Name AzureGermanyCloud -EnableAdfsAuthentication $False -ActiveDirectoryServiceEndpointResourceId https://management.core.cloudapi.de/ -GalleryUrl https://gallery.cloudapi.de -ServiceManagementUrl https://management.core.cloudapi.de/ -SqlDatabaseDnsSuffix .database.cloudapi.de -StorageEndpointSuffix core.cloudapi.de -ActiveDirectoryAuthority https://login.microsoftonline.de/ -GraphUrl https://graph.cloudapi.de/ -trafficManagerDnsSuffix azuretrafficmanager.de -AzureKeyVaultDnsSuffix vault.microsoftazure.de -AzureKeyVaultServiceEndpointResourceId https://vault.microsoftazure.de -ResourceManagerUrl https://management.microsoftazure.de/ -ManagementPortalUrl https://portal.microsoftazure.de
        }

        if ((Get-AzureRMEnvironment AzureGermanyCloud) -eq $null)
        {
            Add-AzureRMEnvironment –Name AzureGermanyCloud -EnableAdfsAuthentication $False -ActiveDirectoryServiceEndpointResourceId https://management.core.cloudapi.de/ -GalleryUrl https://gallery.cloudapi.de -ServiceManagementUrl https://management.core.cloudapi.de/ -SqlDatabaseDnsSuffix .database.cloudapi.de -StorageEndpointSuffix core.cloudapi.de -ActiveDirectoryAuthority https://login.microsoftonline.de/ -GraphUrl https://graph.cloudapi.de/ -trafficManagerDnsSuffix azuretrafficmanager.de -AzureKeyVaultDnsSuffix vault.microsoftazure.de -AzureKeyVaultServiceEndpointResourceId https://vault.microsoftazure.de -ResourceManagerUrl https://management.microsoftazure.de/ -ManagementPortalUrl https://portal.microsoftazure.de
        }

        $global:iotHubSuffix = "azure-devices.de"
        $global:docdbSuffix = "documents.microsoftazure.de"
        $global:eventhubSuffix = "servicebus.cloudapi.de​"
        $global:websiteSuffix = "azurewebsites.de"
        $global:studioApiUrl = "https://germanycentral.studioapi.azureml.de/"
        $global:mlManagement = "https://germanycentral.management.azureml.de"
        $global:locations = @("Germany Central", "Germany Northeast")
    }
    default {throw ("'{0}' is not a supported Azure Cloud environment" -f $azureEnvironmentName)}
}
$global:azureEnvironment = Get-AzureEnvironment $azureEnvironmentName

# Initialize library
. "$(Split-Path $MyInvocation.MyCommand.Path)\DeploymentLib.ps1"
ClearDNSCache

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
    $global:site = "https://{0}.{1}/" -f $environmentName, $global:websiteSuffix
    $cloudDeploy = $true
}
$resourceGroupName = (GetResourceGroup -Name $suiteName -Type $suiteType).ResourceGroupName
$storageAccount = GetAzureStorageAccount $suiteName $resourceGroupName
$iotHubName = GetAzureIotHubName $suitename $resourceGroupName
$eventhubName = GetAzureEventhubName $suitename $resourceGroupName
$simulatorDataFileName = "data.csv"

# Setup AAD for webservice
UpdateResourceGroupState $resourceGroupName ProvisionAAD
$global:AADTenant = GetOrSetEnvSetting "AADTenant" "GetAADTenant"
$global:AADClientId = GetEnvSetting "AADClientId"
UpdateEnvSetting "AADInstance" ($global:azureEnvironment.ActiveDirectoryAuthority + "{0}")

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
    ehName=$eventhubName; `
    storageEndpointSuffix=$($global:azureEnvironment.StorageEndpointSuffix)}

Write-Host "Suite name: $suitename"
Write-Host "Storage Name: $($storageAccount.StorageAccountName)"
Write-Host "IotHub Name: $iotHubName"
Write-Host "Servicebus Name: $eventhubName"
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
        packageUri=$webPackage; `
        webJobPackageUri=$webJobPackage; `
        simulatorDataFileName=$simulatorDataFileName; `
        mlApiUrl=$machineLearningService.ApiLocation; `
        mlApiKey=$machineLearningService.PrimaryKey; `
        aadTenant=$($global:AADTenant); `
        aadInstance=$($global:azureEnvironment.ActiveDirectoryAuthority + "{0}"); `
        aadClientId=$($global:AADClientId)}
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
UpdateEnvSetting "ServiceSBName" $eventhubName
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
    $webEndpoint = "{0}.{1}" -f $environmentName, $global:websiteSuffix
    if (!(HostEntryExists $webEndpoint))
    {
        Write-Host "Waiting for website url to resolve." -NoNewline
        while (!(HostEntryExists $webEndpoint))
        {
            Write-Host "." -NoNewline
            ClearDNSCache
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