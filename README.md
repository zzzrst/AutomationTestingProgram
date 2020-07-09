# Automation Testing Program
This is a _.Net Core 3_ console application. This repository contains the source code for the Automation Testing Program. This application allows users to create and run tests with different data formats for web applications. Please read the [Documentation](https://zzzrst.github.io/AutomationTestingProgram/.) on how to use and modify the program to your needs.

## How to Run

Below are the command line arguments that can be passed in.


Mandatory

--setType 
* The type of data to use. ie "XML" or "txt"

--setArgs 
* The arguments to be passed in. Often the location of the file.

Optional

--b or --browser
* The browser type to use, such as Chrome, IE, Firefox

--e or --environment
* The environment you are running in.

--u or --url
* The url to start the browser in.

--respectRepeatFor

--respectRunAODAFlag

--timeOutThreshold
* The threshold when the program should time out. Defaults to 0.

--warningThreshold

--dataFile

--csvSaveFileLocation
* Location of the csv log report

--logSaveLocation 
* Defaults to the csvSaveFileLocation

--reportSaveLocation 
* Defaults to the csvSaveFileLocation

--screenShotSaveLocation 
* Defaults to the csvSaveFileLocation

--automationProgram 
* The type of testing program to use. ie "selenium"

--caseType 
* Defaults to setType

--stepType 
* Defaults to caseType

--caseArgs 
* Defaults to setArgs

--stepArgs 
* Defaults to caseArgs

### Table of Test Set/Case/Step Arguments
|DataName|TestType|Arguments|Examples|
|---|---|---|---|
|XML|Set/Case/Step|.xml File location|--setType "XML" --setArgs "C:\XMLDocs\Test.xml"|
|Txt|Case/Step|.txt File Location|--setType "Txt" --setArgs "C:\TxtDocs\Test.txt"|
|Database|Case/Step|Collection,Release| --caseType "Database" --caseArgs "1,3"|
|ALM|Set|Test Set ID OR Test Set Name|--setType "ALM"  --setArgs "12345"|
|None|Set|Test Case Name|--setType "None" --setArgs "Test Case Name"|


## Dependancies

* [TestingDrivers](https://github.com/zzzrst/TestingDrivers)
* [AutomationTestSetFramework](https://github.com/zzzrst/AutomationTestSetFramework)
* [DatabaseConnector](https://github.com/zzzrst/DatabaseConnector)
* [TextInteractor](https://github.com/zzzrst/TextInteractor)
* [ComparePDF](https://github.com/zzzrst/ComparePDF)

To Be added
* [ALM](https://github.com/zzzrst/ALM)

## Version Number
The version number for this repository follows (MAJOR).(MINOR).(dailyBuild).(buildTime).

Major version is updated when a large functionality has changed / is introduced.

Minor version is updated when methods are added / removed for bug fixes or a helper class is added due to refactoring.
