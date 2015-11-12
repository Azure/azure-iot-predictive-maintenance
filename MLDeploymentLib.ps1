function ImportLibraries(){
    $success = $true
    $mydocuments = [environment]::getfolderpath("mydocuments")
    $nugetPath = "{0}\Nugets" -f $mydocuments
    if(-not (Test-Path $nugetPath)) {New-Item -Path $nugetPath -ItemType "Directory" | out-null}
    if(-not(Test-Path "$nugetPath\nuget.exe"))
    {
        Write-Host "nuget.exe not found. Downloading from http://www.nuget.org/nuget.exe ..." -ForegroundColor Yellow
        $wc = New-Object System.Net.WebClient
        $wc.DownloadFile("http://www.nuget.org/nuget.exe", "$nugetPath\nuget.exe");
    }

    # ActiveDirectory library
    $success = $success -and (LoadLibrary "Microsoft.IdentityModel.Clients.ActiveDirectory" $nugetPath)

    # Servicebus library
    $success = $success -and (LoadLibrary "WindowsAzure.ServiceBus" $nugetPath "Microsoft.ServiceBus")

    # Storage Library
    $success = $success -and (LoadLibrary "WindowsAzure.Storage" $nugetPath "Microsoft.WindowsAzure.Storage")

    return $success
}

function LoadLibrary()
{
    param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string]$library,
        [Parameter(Mandatory=$true, Position=1)]
        [string]$nugetPath,
        [Parameter(Mandatory=$false, Position=2)]
        [string]$dllName = $library
    )
    $success = $true
    $packageDirectories = (Get-ChildItem -Path $nugetPath -Filter ("{0}*" -f $library) -Directory)
    if($packageDirectories.Length -eq 0)
    {
        Write-Host ("{0} Library Nuget doesn't exist. Downloading now ..." -f $library) -ForegroundColor Yellow
        $nugetDownloadExpression = "& '$nugetPath\nuget.exe' install $library -OutputDirectory '$nugetPath' | out-null"
        Invoke-Expression $nugetDownloadExpression
        $packageDirectories = (Get-ChildItem -Path $nugetPath -Filter ("{0}*" -f $library) -Directory)
    }
    $assemblies = (Get-ChildItem ("{0}.dll" -f $dllName) -Path $packageDirectories[$packageDirectories.length-1].FullName -Recurse)
    if ($assemblies -eq $null)
    {
        Write-Error ("Unable to find {0}.dll assembly for {0} library, is the dll a different name?" -f $library)
        return $false
    }

    # Should figure out how to get correct version
    $assembly = $assemblies[0]
    if($assembly.Length -gt 0)
    {
        Write-Host ("Loading {0} Assembly ..." -f $assembly.Name) -ForegroundColor Green
        [System.Reflection.Assembly]::LoadFrom($assembly.FullName) | out-null
    }
    else
    {
        Write-Host ("Fixing {0} package directories ..." -f $library) -ForegroundColor Yellow
        $packageDirectories | Remove-Item -Recurse -Force | Out-Null
        Write-Error ("Not able to load {0} assembly. Restart PowerShell session and try again ..." -f $library)
        $success = $false
    }
    return $success
}

function HasEnvironment($name) 
{ 
    $environments = Get-AzureEnvironment 
    foreach ($environment in $environments) 
    { 
        if ($environment.Name -eq $name) { return $true } 
    } 
    return $false 
} 

function LoadDogfood()
{
	$dogfood = "dogfood"
    if (-not (HasEnvironment $dogfood))
	{
        Add-AzureEnvironment -Name $dogfood `
            -PublishSettingsFileUrl 'https://windows.azure-test.net/publishsettings/index' `
            -ServiceEndpoint 'https://management-preview.core.windows-int.net/' `
            -ManagementPortalUrl 'https://windows.azure-test.net/' `
            -ActiveDirectoryEndpoint 'https://login.windows-ppe.net/' `
            -ActiveDirectoryServiceEndpointResourceId 'https://management.core.windows.net/' `
            -ResourceManagerEndpoint 'https://api-dogfood.resources.windows-int.net/' `
            -GalleryEndpoint 'https://df.gallery.azure-test.net/' `
            -GraphEndpoint 'https://graph.ppe.windows.net/'
	}
}

function GetAuthenticationResult()
{
    param
    (
        [Parameter(Mandatory=$true, Position=0)]
        [string]$tenant,
        [Parameter(Mandatory=$true, Position=1)]
        [string]$authUri,
        [Parameter(Mandatory=$true, Position=2)]
        [string]$resourceUri,
        [Parameter(Mandatory=$false, Position=3)]
        [string]$user = $null,
        [Parameter(Mandatory=$false)]
        [string]$prompt = "Auto"
    )
    $AADClientId = "1950a258-227b-4e31-a9cf-717495945fc2"
    [Uri]$AADRedirectUri = "urn:ietf:wg:oauth:2.0:oob"
    $authority = "{0}{1}" -f $authUri, $tenant
    write-verbose ("Authority: '{0}'" -f $authority)
    $authContext = New-Object "Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext" -ArgumentList $authority,$true
    $userId = [Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier]::AnyUser
    if (![string]::IsNullOrEmpty($user))
    {
        $userId = new-object Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier -ArgumentList $user, "OptionalDisplayableId"
    }
    write-Verbose ("{0}, {1}, {2}, {3}" -f $resourceUri, $AADClientId, $AADRedirectUri, $userId.Id)
    $authResult = $authContext.AcquireToken($resourceUri, $AADClientId, $AADRedirectUri, $prompt, $userId)
    return $authResult
}

function SendRequest()
{
	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$method,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$uri,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$application,
	    [Parameter(Mandatory=$false, Position=3)]
            $payload = $null,
            [Parameter(Mandatory=$false)]
            [string]$xmsversion = "2014-10-01"
	)
	$subscription = Get-AzureSubscription -Current
	$authResult = GetAuthenticationResult $subscription.TenantId "https://login.windows.net/" $application $subscription.DefaultAccount
	$header = $authResult.CreateAuthorizationHeader()
	write-host ("Sending request to: {0} {1}" -f $method, $uri)
        write-host ("body: {0}" -f $payload)
        $result = Invoke-RestMethod -Method $method -Uri $uri -Headers @{"Authorization"=$header;"Content-Type"="application/json";"x-ms-version"=$xmsversion} -Body $payload
	return $result
}

function ValidateResourceName()
{
    Param(
        [Parameter(Mandatory=$true,Position=0)] [string] $resourceBaseName,
        [Parameter(Mandatory=$true,Position=1)] [string] $resourceType,
        [Parameter(Mandatory=$true,Position=2)] [string] $resourceGroupName
    )

    # Return name for existing resource if exists
    $resources = Get-AzureResource -ResourceGroupName $resourceGroupName -ResourceType $resourceType -OutputObjectFormat New
    if ($resources -ne $null)
    {
        foreach($resource in $resources)
        {
            if ($resource.Name.ToLowerInvariant().StartsWith($resourceBaseName.ToLowerInvariant()))
            {
                return $resource.Name
            }
        }
    }

    # Generate a unique name
    $resourceUrl = " "
    switch ($resourceType.ToLowerInvariant())
    {
        "microsoft.devices/iothubs"
        {
            $resourceUrl = "azure-devices.net"
        }
        "microsoft.storage/storageaccounts"
        {
            $resourceUrl = "blob.core.windows.net"
            $resourceBaseName = $resourceBaseName.Substring(0, [System.Math]::Min(19, $resourceBaseName.Length))
        }
        "microsoft.documentdb/databaseaccounts"
        {
            $resourceUrl = "documents.azure.com"
        }
        "microsoft.eventhub/namespaces"
        {
            $resourceUrl = "servicebus.windows.net"
        }
        "microsoft.web/sites"
        {
            $resourceUrl = "azurewebsites.net"
        }
        default {}
    }
    return GetUniqueResourceName $resourceBaseName $resourceUrl
}

function GetUniqueResourceName()
{
    Param(
        [Parameter(Mandatory=$true,Position=0)] [string] $resourceBaseName,
        [Parameter(Mandatory=$true,Position=1)] [string] $resourceUrl
    )
    $name = $resourceBaseName
    $max = 200
    while (HostEntryExists ("{0}.{1}" -f $name, $resourceUrl))
    {
        $name = "{0}{1:x5}" -f $resourceBaseName, (get-random -max 1048575)
        if ($max-- -le 0)
        {
            throw ("Unable to create unique name for resource {0} for url {1}" -f $resourceBaseName, $resourceUrl)
        }
    }
    Clear-DnsClientCache
    return $name
}

function HostEntryExists()
{
    Param(
        [Parameter(Mandatory=$true,Position=0)] $hostName
    )
    try
    {
        $hostName = [Net.Dns]::GetHostEntry($hostName)
        if ($hostName -ne $null)
        {
            Write-Verbose ("Found hostname: {0}" -f $hostName)
            return $true
        }
    }
    catch {}
    Write-Verbose ("Did not find hostname: {0}" -f $hostName)
    return $false
}

function GetResourceGroup()
{
    Param(
        [Parameter(Mandatory=$true,Position=0)] [string] $name,
        [Parameter(Mandatory=$true,Position=1)] [string] $type,
        [Parameter(Mandatory=$true,Position=2)] [string] $allocationRegion
    )
    $resourceGroup = Get-AzureResourceGroup -Tag @{Name="IotSuiteType";Value=$type} | ?{$_.ResourceGroupName -eq $name}
    if ($resourceGroup -eq $null)
    {
        $resourceGroup = New-AzureResourceGroup -Name $name -Location $allocationRegion -Tag @{Name="IoTSuiteType";Value=$type}, @{Name="IoTSuiteVersion";Value=$global:version}, @{Name="IoTSuiteState";Value="Created"}
    }
    return $resourceGroup
}

function GetAzureStorageAccount()
{
    Param(
        [Parameter(Mandatory=$true,Position=0)] [string] $storageBaseName,
        [Parameter(Mandatory=$true,Position=1)] [string] $resourceGroupName
    )
    $storageTempName = $storageBaseName.ToLowerInvariant().Replace('-','')
    $storageAccountName = ValidateResourceName $storageTempName.Substring(0, [System.Math]::Min(24, $storageTempName.Length)) Microsoft.Storage/storageAccounts $resourceGroupName
    $storage = Get-AzureStorageAccount -ResourceGroupName $resourceGroupName -Name $storageAccountName -ErrorAction SilentlyContinue
    if ($storage -eq $null)
    {
        Write-Host "Creating new storage account: $storageAccountName"
        $storage = New-AzureStorageAccount -ResourceGroupName $resourceGroupName -StorageAccountName $storageAccountName -Location $global:AllocationRegion -Type Standard_LRS
    }
    return $storage
}

function CreateWorkSpace()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$name,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$resourceGroupName,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$storageAccountName
	)
        $endpoint = "https://management.core.windows.net/"
	$subscription = Get-AzureSubscription -Current
	$environment = Get-AzureEnvironment $subscription.Environment
        $storageAccountKey = (Get-AzureStorageAccountKey -StorageAccountName $storageAccountName -ResourceGroupName $resourceGroupName).Key1
        $liveId = (Get-AzureSubscription -Current).DefaultAccount
        $endpointId = [System.Guid]::NewGuid()
        $body = "{{""Name"":""{0}"",""Location"":""South Central US"",""StorageAccountName"":""{1}"",""StorageAccountKey"":""{2}"",""OwnerId"":""{3}"",""ImmediateActivation"":true,""Source"":""SolutionAccelerator""}}" -f $name, $storageAccountName, $storageAccountKey, $liveId 
	return SendRequest "PUT" ("{0}{1}/cloudservices/{2}/resources/machinelearning/~/workspaces/{3}" -f $endpoint, $subscription.SubscriptionId, $name, $endpointId )  $endpoint $body
}

function GetWorkSpace()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$name,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$workspaceId
	)
	$endpoint = "https://management.core.windows.net/"
	$subscription = Get-AzureSubscription -Current
	$environment = Get-AzureEnvironment $subscription.Environment
	return SendRequest "GET" ("{0}{1}/cloudservices/{2}/resources/machinelearning/~/workspaces/{3}" -f $endpoint, $subscription.SubscriptionId, $name, $workspaceId )  $endpoint
}

function GetWorkspaceId()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$name
	)
	$endpoint = "https://management.core.windows.net/"
	$subscription = Get-AzureSubscription -Current
	$environment = Get-AzureEnvironment $subscription.Environment
	return SendRequest "GET" ("{0}{1}/cloudservices/{2}/resources/machinelearning/~/workspaces/" -f $endpoint, $subscription.SubscriptionId, $name)  $endpoint | ?{$_.Name -eq $name}
}

function GetWebService()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$webServiceId
	)
	$endpoint = "https://management.azureml.net/"
	$uri = "{0}workspaces/{1}/webservices/{2}/endpoints/default" -f $endpoint, $workspaceId, $webServiceId
	$header = "Bearer {0}" -f $key
	return Invoke-RestMethod -Method "GET" -Uri $uri -Headers @{"Authorization"=$header;"Content-Type"="application/json"}
}

function CopyExperimentToWorkspace()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$packageUri
	)
	$endpoint = "https://studioapi.azureml.net/"
	$uri = "{0}api/workspaces/{1}/packages?api-version=2.0&packageUri={2}" -f $endpoint, $workspaceId, $packageUri
	Write-Verbose $uri
	return Invoke-RestMethod -Method "PUT" -Uri $uri -Headers @{"x-ms-metaanalytics-authorizationtoken"=$key}
}

function GetExperimentCopyResult()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$activityId
	)
	$endpoint = "https://studioapi.azureml.net/"
	$uri = "{0}api/workspaces/{1}/packages?unpackActivityId={2}" -f $endpoint, $workspaceId, $activityId
	return Invoke-RestMethod -Method "GET" -Uri $uri -Headers @{"x-ms-metaanalytics-authorizationtoken"=$key}
}

function CreateProject()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$trainingId,
	    [Parameter(Mandatory=$true, Position=3)]
	    [string]$scoringId
	)
	$endpoint = "https://studioapi.azureml.net/"
	$uri = "{0}api/workspaces/{1}/projects" -f $endpoint, $workspaceId, $activityId
	$trainingExperiment = GetExperiment $workspaceId $key $trainingId
	$scoringExperiment = GetExperiment $workspaceId $key $scoringId
	$body = "{{""Experiments"":[{{""ProjectExperiment"":{{""ExperimentId"":""{0}"",""Role"":""Training"",""Experiment"":{2}}}}},{{""ProjectExperiment"":{{""ExperimentId"":""{1}"",""Role"":""Scoring"",""Experiment"":{3}}}}}]}}" -f $trainingId, $scoringId, $trainingExperiment, $scoringExperiment
	Write-Verbose $body
	return Invoke-RestMethod -Method "POST" -Uri $uri -Headers @{"x-ms-metaanalytics-authorizationtoken"=$key} -Body $body
}

function CreateWebService()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$scoringExperimentId
	)
	$endpoint = "https://studioapi.azureml.net/"
	$uri = "{0}api/workspaces/{1}/experiments/{2}/webservice" -f $endpoint, $workspaceId, $scoringExperimentId
	Write-Verbose $uri
	return Invoke-RestMethod -Method "POST" -Uri $uri -Headers @{"x-ms-metaanalytics-authorizationtoken"=$key}
}

function GetWebServiceCreateResult()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$activityId
	)
	$endpoint = "https://studioapi.azureml.net/"
	$uri = "{0}api/workspaces/{1}/experiments/{2}/webservice" -f $endpoint, $workspaceId, $activityId
	Write-Verbose $uri
	return Invoke-RestMethod -Method "GET" -Uri $uri -Headers @{"x-ms-metaanalytics-authorizationtoken"=$key}
}

function GetExperiment()
{
  	param
	(
	    [Parameter(Mandatory=$true, Position=0)]
	    [string]$workspaceId,
	    [Parameter(Mandatory=$true, Position=1)]
	    [string]$key,
	    [Parameter(Mandatory=$true, Position=2)]
	    [string]$experimentId
	)
	$endpoint = "https://studioapi.azureml.net/"
	$uri = "{0}api/workspaces/{1}/experiments/{2}" -f $endpoint, $workspaceId, $experimentId
	Write-Verbose $uri
	return  (Invoke-WebRequest -Method "GET" -Uri $uri -Headers @{"x-ms-metaanalytics-authorizationtoken"=$key}).Content
}