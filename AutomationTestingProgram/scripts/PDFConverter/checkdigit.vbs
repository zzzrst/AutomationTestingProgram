if WScript.Arguments.Count = 1 Then
    strInput = WScript.Arguments.Item(0)
    If IsNumeric(strInput) Then
    	WScript.Echo "1"
    Else
  	  WScript.Echo "0"
    End If
Else
    WScript.Echo "0"
End If