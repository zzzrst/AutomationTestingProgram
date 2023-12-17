# this script runs a powershell script which creates a release. 

$azureDevOpsPAT = "hfwv4s5f4nixa7vk4zr2cdut57ysw6qrq7eg64ufftovqe5m3khq"

$AzureDevOpsAuthenicationHeader = @{Authorization = 'Basic ' + [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$($AzureDevOpsPAT)")) }

$UriOrga = "https://dev.azure.com/csc-ddsb/AutomationAndAccessibility/" 
$uriAccount = $UriOrga + "_apis/release/releases/28?api-version=7.1-preview.8"

$projectConfiguration = @{
	  "definitionId" = 28
	    "description" = "Creating Sample release"
	      "isDraft" = "false"
	        "reason" = "none"
		  "manualEnvironments" = "null"
} | ConvertTo-Json -Depth 5


Invoke-RestMethod -Uri $uriAccount -Method Post -Headers $AzureDevOpsAuthenticationHeader -Body $projectConfiguration -ContentType "application/json"
