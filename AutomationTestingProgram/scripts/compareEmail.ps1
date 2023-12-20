 param (
    [Parameter(Mandatory=$true)][string]$emailPath,
    [Parameter(Mandatory=$true)][string]$ExpectedPath,
	[Parameter(Mandatory=$true)][string]$emailShortList,
    [Parameter(Mandatory=$true)][string]$resultFilePath
 )


 function String-Compare {
    param(
        [Parameter()]
        $expected,
        $actual
    )
    # Remove any non-alphanumeric
    $pattern = "\W"
    $actual = $actual -replace $pattern
    $expected = $expected -replace $pattern
    if ($actual -eq $expected) {
        $true
    } else {
        $false
    }
}

function String-Match {
    param(
        [Parameter()]
        $expected,
        $actual
    )
    # Remove any non-alphanumeric
    $pattern = "\W"
    $actual = $actual -replace $pattern
    $expected = $expected -replace $pattern
    if ($actual -match $expected) {
        $true
    } else {
        $false
    }

}

if (Test-Path $resultFilePath) {
    Remove-Item $resultFilePath
}

if (-not (Test-Path $emailShortList)) {
    Write-Host "Could not find $emailShortList. Please check if email was generated."
    $LASTEXITCODE = 1
    exit 1
}


$notFoundError = "Could not find " + $ExpectedPath
if (Test-Path $ExpectedPath) {
    $AdoDbStream = New-Object -ComObject ADODB.Stream
    $AdoDbStream.Open()
    $AdoDbStream.LoadFromFile("$ExpectedPath")
    $CdoMessage = New-Object -ComObject CDO.Message
    $CdoMessage.DataSource.OpenObject($AdoDbStream ,"_Stream")

    $expectedSubject = $CdoMessage.Subject
    $expectedTo = $CdoMessage.To
    $expectedFrom = $CdoMessage.From
    $expectedCc = $CdoMessage.Cc
    $expectedBcc = $CdoMessage.Bcc
    $expectedTxtBody = $CdoMessage.TextBody
    $expectedHtmlBody =  $CdoMessage.Htmlbody
    $AdoDbStream.Close()
    
    $exactMatch = $false

    foreach($line in Get-Content -path "$emailShortList") {

        Write-Host "We are comparing $ExpectedPath to $emailPath\$line"

        $tempCond = $true
 
	    $AdoDbStream = New-Object -ComObject ADODB.Stream
	    $AdoDbStream.Open()
	    $AdoDbStream.LoadFromFile("$emailPath\$line")
	    $CdoMessage = New-Object -ComObject CDO.Message
	    $CdoMessage.DataSource.OpenObject($AdoDbStream ,"_Stream")

         
        $actualSubject = $CdoMessage.Subject
        $actualTo =  $CdoMessage.To
        $actualFrom =  $CdoMessage.From
        $actualCc =  $CdoMessage.Cc
        $actualBcc = $CdoMessage.Bcc
        $actualTxtBody = $CdoMessage.TextBody
        $actualHtmlBody = $CdoMessage.Htmlbody

        $AdoDbStream.Close()

        if (-not (String-Compare $expectedSubject $actualSubject)) {
            "Expected Subject is: " + $expectedSubject >> $resultFilePath
            "Actual Subject is: " + $actualSubject >> $resultFilePath
            "" >> $resultFilePath
            $tempCond = $false
        }

        if (-not (String-Compare $expectedFrom $actualFrom)) {
            "Expected From is: " + $expectedFrom >> $resultFilePath
            "Actual From is: " + $actualFrom >> $resultFilePath
            "" >> $resultFilePath
            $tempCond = $false
        }

        if (-not (String-Compare $expectedTxtBody $actualTxtBody)) {
            "Expected Text Body is: " + $expectedTxtBody >> $resultFilePath
            "Actual Text Body is: " + $actualTxtBody >> $resultFilePath
            "" >> $resultFilePath
            $tempCond = $false
        }

        if (-not (String-Compare $expectedHtmlbody $actualHtmlbody)) {
            "Expected HTML Body is: " + $expectedHtmlbody >> $resultFilePath
            "Actual HTML Body is: " + $actualHtmlbody >> $resultFilePath
            "" >> $resultFilePath
            $tempCond = $false
        }

        if (-not(String-Match $expectedTo $actualTo)) {
            "Expected To is: " + $expectedTo >> $resultFilePath
            "Actual To is: " + $actualTo >> $resultFilePath
            "" >> $resultFilePath
            $tempCond = $false
        }


        if (-not(String-Match $expectedCc $actualCc)) {
            "Expected Cc is: " + $expectedCc >> $resultFilePath
            "Actual Cc is: " + $actualCc >> $resultFilePath
            "" >> $resultFilePath
            $tempCond = $false
        }

        if (-not(String-Match $expectedBcc $actualBcc)){
            "Expected Bcc is: " + $expectedBcc >> $resultFilePath
            "Actual Bcc is: " + $actualBcc >> $resultFilePath
            "--------------------------------------------------------" >> $resultFilePath
            $tempCond = $false
        }
    
        if ($tempCond) {
            $exactMatch = $true
            Write-Host "We have found a match! $emailPath\$line"
        }
    }
} else {
    $notFoundError >> $resultFilePath
}

if ($exactMatch) {
    Remove-Item $resultFilePath -ErrorAction Ignore -Force
}
else {
    $LASTEXITCODE = -1;
}