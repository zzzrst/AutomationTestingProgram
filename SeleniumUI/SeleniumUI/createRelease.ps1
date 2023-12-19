# this script runs a powershell script which creates a release. 

$azureDevOpsPAT = "pq2nhsdttstev7k4use72rc75odn6v6gtriarjarumpnxd3iqsaa"

$header = @{Authorization = 'Basic' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($AzureDevOpsPAT)")) }

# $UriOrg = "https://dev.azure.com/csc-ddsb/AutomationAndAccessibility/" 
# $uriAccount = $UriOrg + "_apis/wit/workitems/51534?api-version=7.0"

$UriOrg = "https://vsrm.dev.azure.com/csc-ddsb/AutomationAndAccessibility/" 
$uriAccount = $UriOrg + "_apis/release/releases?api-version=7.1"

$projectConfiguration = @{
	  "definitionId" = 28;
	  "description" = "Queuing automated tests";
	  "isDraft" = "false";
	  "reason" = "none";
	  "manualEnvironments" = "null";
} | ConvertTo-Json

Invoke-RestMethod -Uri $uriAccount -Method post -Headers $header -Body $projectConfiguration -ContentType "application/json"

# Invoke-RestMethod -Uri $uriAccount -Method Post -Headers 
# $AzureDevOpsAuthenticationHeader -Body $projectConfiguration -ContentType 
# "application/json"
