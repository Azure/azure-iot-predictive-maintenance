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