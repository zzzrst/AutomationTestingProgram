# this powershell script demonstrates how to execute on an agent

# Write-Host "Running Automation Testing Program built using Azure DevOps version" + 

Write-Host "Beginning PS execution for TAP from inline PS script"
 
$csvString = $env:FILEPATH
$planStructure = $env:TESTPLANSTRUCTURE

$environment = $env:ENVIRONMENT
$browser = $env:BROWSER
$testPlanName = $env:TESTPLANNAME
$buildNumber = $env:BUILDNUMBER
$notifyList = $env:NOTIFYLIST
$runParams = $env:RUNPARAMETERS
$executionURL = $env:RELEASE_RELEASEWEBURL
$runAODA = $env:RUNAODA
$appConfig = $env:APPCONFIGOVERRIDES
$testerEmail = $env:RELEASE_REQUESTEDFOREMAIL
$testerName = $env:RELEASE_REQUESTEDFOR


$releaseUriAndEnv = $env:RELEASE_RELEASEURI + "," + $env:RELEASE_ENVIRONMENTURI

# project name is very important! Remember that this shouldn't be used lightly. 
# $projectName = $env:SYSTEM_TEAMPROJECT
$projectName = $env:TESTPROJECTNAME


Write-Host "Project Name is:" $projectName
Write-Host "Build Number is:" $buildNumber
Write-Host "CSV string:" $csvString
Write-Host "Environment:" $environment

$items = $csvString.Split(',')

# for testing from agent
$testStorageDirectory = "$(System.DefaultWorkingDirectory)"
# $executionDirectory = "$(System.DefaultWorkingDirectory)"
$executionDirectory = "$(Agent.WorkFolder)\TAP"

Write-Host $executionDirectory

Write-Host "PAT is: " $env:PersonalAccessToken

# get the execution file that we will be using
$execFile = $executionDirectory + "\AutomationTestingProgram.exe"

$returnExitCode = 0

# we will execute each item inside the list of items
foreach ($item in $items) {
    $item = $item.Trim() # trim any leading or training whitespace
    Write-Host "Processing item: $item"

    # create execution location with quotes around it, include release location
    $item = """" + $testStorageDirectory + "\" + $env:RELEASELOCATION + "\" +  $item + """"
    
    # create arguments
    $args = "--environment " + $environment + " --setArgs " + $item +  " --projectName  " + $projectName + " --browser " + $browser + " --planName " + $testPlanName + " --buildNumber " + $buildNumber + " --azurePAT " + $env:PersonalAccessToken + " --notifyList " + $notifyList + " --runParams " + $runParams + " --executionURL " + $executionURL + " --respectRunAODAFlag " + $runAODA + " --appConfig " + $appConfig + " --testPlanStructure " + $planStructure  + " --releaseEnvUri " + $releaseUriAndEnv + " --testerName " + """" +  $testerName +   """" + " --testerContact " + $testerEmail

    # print out the arguments that we will be executing with
    Write-Host "Executing:" $execFile $args

    # start the process
    # Start-Process -Wait -FilePath $execFile -ArgumentList $args

    # we want to redirect standard input and print while doing so.
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = $execFile
    $pinfo.RedirectStandardError = $true
    $pinfo.RedirectStandardOutput = $true
    $pinfo.UseShellExecute = $false
    $pinfo.Arguments = $args
    $p = New-Object System.Diagnostics.Process
    $p.StartInfo = $pinfo
    $p.Start() | Out-Null
    
    # print while there is still standard output to read
    while (($line = $p.StandardOutput.ReadLine()) -ne $null)
    {
        Write-Host "                 " $line
    }

    $p.WaitForExit()
    
    Write-Host "exit code: " + $p.ExitCode

     # exit with this exit code
     if ($p.ExitCode -ne 0)
    {
          Write-Host "program failed"
          $returnExitCode = 1
     }
}

Write-Host "program successfully completed"

exit $returnExitCode