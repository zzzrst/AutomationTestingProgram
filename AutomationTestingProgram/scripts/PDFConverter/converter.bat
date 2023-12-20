@echo off

set argCount=0
set "dir=%cd%\temporary_files"

for %%x in (%*) do (
   set /A argCount+=1
   set "argVec[!argCount!]=%%~x"
)



if EXIST %dir%\expected.txt (
    del /f  %dir%\expected.txt
)

if EXIST  %dir%\actual.txt (
    del /f  %dir%\actual.txt
)




if %argcount% GEQ 1 (

    if %argcount% EQU 1 (

	goto caseOneFile

    ) Else (

	goto caseTwoPlusArguments

    )

) Else (

    goto caseHelp
)

:caseOneFile
if EXIST %1 (

    echo Converting '%~1' into 'expected.txt'.
    call "%~dp0\pdftotext" -nopgbrk -table "%~1"  %dir%\expected.txt

) ELSE ("
    echo '%~1' was not found!
    goto CaseHelp

)

goto caseExit



:caseTwoPlusArguments
if EXIST %1 (

    if EXIST %2 (

	goto caseTwoFiles

    ) Else (

	if %argcount% EQU 2 (

	    for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %2') do (
		if %%A EQU 1 (echo F parameter is %2.) ELSE (echo File doesn't exist or user inputted non-number: %2&&goto caseHelp)
	    )

	    echo Converting '%~1' into 'expected.txt' starting at %2
	    call "%~dp0\pdftotext" -f %2 -nopgbrk -table "%~1"   %dir%\expected.txt

	) Else (

	    for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %2') do (
		if %%A EQU 1 (echo F parameter is %2.) ELSE (echo User inputted non-number: %2&&goto caseHelp)
	    )

	    for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %3') do (
	        if %%A EQU 1 (echo L parameter is %3.) ELSE (echo User inputted non-number: %3&&goto caseHelp)
	    )

	    echo Converting '%~1' into 'expected.txt' from %2:%3
	    call "%~dp0\pdftotext" -f %2 -l %3 -nopgbrk -table "%~1"   %dir%\expected.txt
	)

    )

) Else (

    echo "%~1" was not found!
    goto caseHelp
)

goto caseExit

:caseTwoFiles
if %argcount% EQU 2 (

    echo Converting '%~1' into 'expected.txt'.
    call "%~dp0\pdftotext" -nopgbrk -table "%~1" %dir%\expected.txt
    echo Converting '%~2' into 'actual.txt'.
    call "%~dp0\pdftotext" -nopgbrk -table "%~2" %dir%\actual.txt

) Else (

    if %argcount% EQU 3 (

	for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %3') do (
		if %%A EQU 1 (echo F parameter is %3.) ELSE (echo User inputted non-number: %3&&goto caseHelp)
	    )

	echo Converting '%~1' into 'expected.txt' starting at %3
	call "%~dp0\pdftotext" -f %3 -nopgbrk -table "%~1" %dir%\expected.txt

	echo Converting '%~2' into 'actual.txt' starting at %3
	call "%~dp0\pdftotext" -f %3 -nopgbrk -table "%~2" %dir%\actual.txt

    ) Else (

	if %argcount% EQU 4 (

	    for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %3') do (
		if %%A EQU 1 (echo F parameter is %3.) ELSE (echo User inputted non-number: %3&&goto caseHelp)
	    )

	    for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %4') do (
		if %%A EQU 1 (echo F2 parameter is %4.) ELSE (echo User inputted non-number: %4&&goto caseHelp)
	    )

	    echo Converting '%~1' into 'expected.txt' starting at %3
	    call "%~dp0\pdftotext" -f %3 -nopgbrk -table "%~1" %dir%\expected.txt

	    echo Converting '%~2' into 'actual.txt' starting at %4
	    call "%~dp0\pdftotext" -f %4 -nopgbrk -table "%~2" %dir%\actual.txt

	) Else (

	    if %argcount% GEQ 6 (

		for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %3') do (
		    if %%A EQU 1 (echo F1 parameter is: %3.) ELSE (echo User inputted non-number: %3&&goto caseHelp)
	        )

		for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %4') do (
		    if %%A EQU 1 (echo L1 parameter is %4.) ELSE (echo User inputted non-number: %4&&goto caseHelp)
	        )


		for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %5') do (
		    if %%A EQU 1 (echo F2 parameter is %5.) ELSE (echo User inputted non-number: %5&&gotto caseHelp)
	        )

		for /F %%A in ('cscript //nologo "%~dp0\checkdigit.vbs" %6') do (
		    if %%A EQU 1 (echo L2 parameter is %6.) ELSE (echo User inputted non-number: %6&&goto caseHelp)
	        )

	        echo Converting '%~1' into 'expected.txt' from %3:%4
	        call "%~dp0\pdftotext" -f %3 -l %4 -nopgbrk -table "%~1" %dir%\expected.txt

	        echo Converting '%~2' into 'actual.txt' from %5:%6
	        call "%~dp0\pdftotext" -f %5 -l %6 -nopgbrk -table "%~2" %dir%\actual.txt

	     ) Else (

		goto caseHelp
	     )
	)
    )
)
goto caseExit



:caseHelp
echo For format, please read 'help.txt' located in the %~dp0
timeout 10
exit 1


:caseExit

