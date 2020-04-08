 <#dir <Path of file> /O:-D /b#>
 param (
    [Parameter(Mandatory=$true)][string]$emailSharedLocationPath,
    [Parameter(Mandatory=$true)][string]$emailLocalPath,
    [Parameter(Mandatory=$true)][string]$Username, 
	[Parameter(Mandatory=$true)][string]$Password
 )


# Call net use with username and password.
try 
{
    Write-Host "We are going to map $emailSharedLocationPath with user '$Username'."
    net use "$emailSharedLocationPath" /user:$Username $Password
} 
catch 
{
    Write-Host "$emailSharedLocationPath has been mapped already"
}

# Remove the file if exist already
Remove-Item $emailLocalPath -ErrorAction Ignore -Recurse -Force

robocopy $emailSharedLocationPath $emailLocalPath /maxage:1