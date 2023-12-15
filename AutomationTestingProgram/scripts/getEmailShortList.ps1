 <#dir <Path of file> /O:-D /b#>
 param (
    [Parameter(Mandatory=$true)][int]$timeSpanValue,
    [Parameter(Mandatory=$true)][string]$emailPath,
    [Parameter(Mandatory=$true)][string]$subject, 
	[Parameter(Mandatory=$true)][string]$emailList
 )
 $timeSpanSample = (Get-Date).AddMinutes(-$timeSpanValue)
 $shell = New-Object -ComObject Wscript.Shell
 
 <#Delete all .csv files starting with "Result_Email" in the $path folder#>
 Remove-Item $emailList -ErrorAction Ignore -Force

 <#Retrieve the list of emails#>
 $emailListContents = ls $emailPath -Name
 $subjectMatchCount = 0
 $i = 0
 <#Retrieve the date/time that the latest email was sent#>
 $sentOn = (Get-Item "$emailPath\$($emailListContents[$i])").LastWriteTime

 Write-Host "Email was last written at: " $sentOn

 <#Retrieve all emails that are within the timespan#>
  while ($i -lt $emailListContents.Length -and ($sentOn -gt $timespanSample -Or $sentOn -eq $timeSpanSample)){

	 $AdoDbStream = New-Object -ComObject ADODB.Stream
	 $AdoDbStream.Open()

	 $AdoDbStream.LoadFromFile("$emailPath\$($emailListContents[$i])")
	 $CdoMessage = New-Object -ComObject CDO.Message
	 $CdoMessage.DataSource.OpenObject($AdoDbStream ,"_Stream")

     # Use the following line to get the subject line
     # $CdoMessage.Subject
     
     # original: check if they are of the same length ($subject.Length -eq $CdoMessage.Subject.Length)
     # check that the given subject is less than or equal the email's subject
     if ($subject.Length -le $CdoMessage.Subject.Length) {

        # remove any non-alphanumeric
        $pattern = "\W"
        $subject = $subject -replace $pattern
        $emailSubject = $CdoMessage.Subject -replace $pattern

        Write-Host "Email subject found is " $emailSubject ". Wanted " $subject

        #original : $subject -eq $emailSubject  || $emailSubject -contains $subject
        if ($emailSubject -match $subject) {
		    $subjectMatchCount ++
            $emailListContents[$i] | Out-File -FilePath $emailList -NoClobber -Append
	    }
     }

	 $i++
	 $sentOn = (Get-Item "$emailPath\$($emailListContents[$i])").LastWriteTime
	 $AdoDbStream.Close()
}
<#Read-Host -Prompt "Press Enter to exit"#>
if ($subjectMatchCount = 0) {
    $LASTEXITCODE = 1
    exit 1
}