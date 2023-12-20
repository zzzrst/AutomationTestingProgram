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
 # $emailListContents = ls $emailPath -Name | Sort-Ojbect -Property LastAccessTime
 $subjectMatchCount = 0
 $i = 0
 <#Retrieve the date/time that the latest email was sent#>
 $sentOn = (Get-ChildItem -File $emailPath | Sort-Object -Property LastAccessTime | Select-Object -Last 1).LastWriteTime

 Write-Host "Email was last written at: " $sentOn
 Write-Host "Email i length" $i 
 Write-Host "Time span sample" $timeSpanSample 
 Write-Host "Sent on: " $sentOn 
 

 <#Retrieve all emails that are within the timespan#>
  while ($i -lt $emailListContents.Length -and ($sentOn -gt $timespanSample -Or $sentOn -eq $timeSpanSample)){

	 $AdoDbStream = New-Object -ComObject ADODB.Stream
	 $AdoDbStream.Open()

	 $AdoDbStream.LoadFromFile("$emailPath\$($emailListContents[$i])")
	 $CdoMessage = New-Object -ComObject CDO.Message
	 $CdoMessage.DataSource.OpenObject($AdoDbStream ,"_Stream")


    Write-Host "Email subject found is " $CdoMessage.Subject
	
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
			
			Write-Host "Email subject matched found is " $emailSubject " Found: " $subject
	    }
     }

	 $i++
	 $sentOn = (Get-Item "$emailPath\$($emailListContents[$i])").LastWriteTime
	 
	 # we want to find the next sentOn time that is in the range. 
	 while ($i -lt $emailListContents.Length -and $sentOn -lt $timespanSample){
		$i++
		$sentOn = (Get-Item "$emailPath\$($emailListContents[$i])").LastWriteTime
	 }
	 
	 $AdoDbStream.Close()
}

<#Read-Host -Prompt "Press Enter to exit"#>
if ($subjectMatchCount = 0) {
	Write-Host "None matched" $subjectMatchCount
    $LASTEXITCODE = 1
    exit 1
}