 param (
    [Parameter(Mandatory=$true)][string]$azurePAT,
    [Parameter(Mandatory=$true)][string]$fileNames,
    [Parameter(Mandatory=$true)][string]$environment,
	[Parameter(Mandatory=$true)][AllowEmptyString()][string]$notifyList,
    [Parameter(Mandatory=$true)][AllowEmptyString()][string]$planStructure,
    [Parameter(Mandatory=$true)][AllowEmptyString()][string]$planName,
    [Parameter(Mandatory=$true)][AllowEmptyString()][string]$project,
    [Parameter(Mandatory=$true)][AllowEmptyString()][string]$releaseId,
    [Parameter(Mandatory=$true)][AllowEmptyString()][string]$buildNumber,
    [Parameter(Mandatory=$true)][AllowEmptyString()][string]$browser
 )

$header = @{Authorization = 'Basic' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($azurePAT)")) }

$UriOrg = "https://vsrm.dev.azure.com/csc-ddsb/" + $project 
$uriAccount = $UriOrg + "/_apis/release/releases?api-version=7.1"

Write-Host "Beginning PS execution for TAP to generate executions"
 
$parallelEx = $fileNames.Split(';')

foreach($execution in $parallelEx){

	$setName = $execution.Split("\")[-1]
	Write-Host = "FileName is " $setName

	$setName = "Automated " + $setName + " " + $environment + " " + $buildNumber

	# we will execute each item inside the list of items
	# create execution location with quotes around it, include release location
	$projectConfiguration = @{
		  "definitionId" = $releaseId;
		  "description" = "Automatically generated tests";
		  "isDraft" = "false";
		  "reason" = "none";
		  "manualEnvironments" = "null";
		  "variables" = @{
				"FilePath"=@{
					"allowOverride"="True"
					"value"= $execution 
				}
				"NotifyList"=@{
					"allowOverride"="True"
					"value"= $notifyList
				}
				"BuildNumber"=@{
					"allowOverride"="True"
					"value"= $buildNumber
				}
				"Environment"=@{
					"allowOverride"="True"
					"value"= $environment
				}
				"TestPlanName"=@{
					"allowOverride"="True"
					"value"= $planName
				}
				"TestPlanStructure"=@{
					"allowOverride"="True"
					"value"= $planStructure
				}
				"Browser"=@{
					"allowOverride"="True"
					"value"= $browser
				}
				"TestSetName"=@{
					"allowOverride"="True"
					"value"= $setName
				}
		  }
	} | ConvertTo-Json -Depth 5

	try
	{
		$Response = Invoke-WebRequest -Uri $uriAccount -Method post -Headers $header -Body $projectConfiguration -ContentType "application/json"
	    # This will only execute if the Invoke-WebRequest is successful.
		$StatusCode = $Response.StatusCode
	}
	catch
	{
		$StatusCode = $_.Exception.Response.StatusCode.value__
		Write-Host "Error details" $_.ErrorDetails.Message
	}

	Write-Host "Exit code " $StatusCode

	if ($StatusCode -ne 200){
		Write-Host "Ending program"
		exit $StatusCode
	}
}

Write-Host "Program Finished"

exit 0