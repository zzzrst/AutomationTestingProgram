# Automation Testing Program
This is a .Net Core 3 Console Application. This Directory contains the source code for the Automation Testing program application. This program allows users to create and run tests for application from different files. Please read the [Documentation](https://zzzrst.github.io/AutomationTestingProgram/.) on how to use and modify the program to your needs.

## How To run

You need to pass in arguments when running this through command line. Below are arguments that you can use.

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

## Dependancies

* [TestingDrivers](https://github.com/zzzrst/TestingDrivers)
* [AutomationTestSetFramework](https://github.com/zzzrst/AutomationTestSetFramework)

To Be added
* [ALM](https://github.com/zzzrst/ALM)
* [DatabaseConnector](https://github.com/zzzrst/DatabaseConnector)
* [TextInteractor](https://github.com/zzzrst/TextInteractor)
* [ComparePDF](https://github.com/zzzrst/ComparePDF)

## Other Notes
Version number are labed as MAJOR.MINOR.dailyBuild.buildTime
Major changes are when new classes are added, except helper classes, or when a large change is made to the code such as a large functionality change.
Minor changes are when methods are added/removed or a helper class is added.
