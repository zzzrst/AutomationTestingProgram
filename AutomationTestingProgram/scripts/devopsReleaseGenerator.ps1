# this script runs a powershell script which creates a release. 

$azureDevOpsPAT = "***********************"

$header = @{Authorization = 'Basic' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($AzureDevOpsPAT)")) }

$UriOrg = "https://vsrm.dev.azure.com/csc-ddsb/Picasso/" 
$uriAccount = $UriOrg + "_apis/release/releases?api-version=7.1"

Write-Host "Beginning PS execution for TAP to generate multiple executions"
 
$csvString = $env:HIGHSMOKE
$planStructure = "HIGH/SMOKE"

$parallelEx = $csvString.Split(';')

foreach($execution in $parallelEx){
	# we will execute each item inside the list of items
	# create execution location with quotes around it, include release location
	$projectConfiguration = @{
		  "definitionId" = 2;
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
					"value"= $env:NOTIFYLIST
				}
				"BuildNumber"=@{
					"allowOverride"="True"
					"value"= $env:BUILDNUMBER
				}
				"Environment"=@{
					"allowOverride"="True"
					"value"= $env:ENVIRONMENT
				}
				"TestPlanStructure"=@{
					"allowOverride"="True"
					"value"= $planStructure
				}
				"Browser"=@{
					"allowOverride"="True"
					"value"= $env:BROWSER
				}
				"TestSetName"=@{
					"allowOverride"="True"
					"value"= "Automatically Generated Test"
				}
		  }
	} | ConvertTo-Json -Depth 5

	Invoke-RestMethod -Uri $uriAccount -Method post -Headers $header -Body $projectConfiguration -ContentType "application/json"

}
	
Write-Host "Program Successfully Completed"
