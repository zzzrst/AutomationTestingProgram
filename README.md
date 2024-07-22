# Automation Testing Program
This is a ``.Net 6`` console application. This repository contains the source code for the Automation Testing Program. This application allows users to create and run tests with different data formats for web applications. Please read the [Documentation](https://zzzrst.github.io/AutomationTestingProgram/.) on how to use and modify the program to your needs.

The framework runs on a Key Word Driven model. 

## How to Run

Below are the command line arguments that can be passed in.

Mandatory

--setArgs 
* The arguments to be passed in. Often the location of the file.

--e or --environment
* The environment you are running in.

Optional

--buildNumber
* The build number of the execution.

--setType 
* The type of data to use. ie "XML" or "txt" or "ALM". Default is Excel

--b or --browser
* The browser type to use, such as Chrome, IE, Firefox. Default is Chrome

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


--kvSecrets
* key vault secret information for the AAD user (can also be non aad)

--maxFailures
* maximum number of consecutive failures before failing test set

--notifyList
* List of emails to notify users after execution

--RunParameters
* Key Value pair separating values that are to be used for overwriting parameters in the Excel

Optional DevOps Specific parameters

--planName
* name of the test plan to be used for generating tests

--projectName
* name of hte test project to be used for generating tests

--azurePAT
* PAT token for API calls to DevOps

--executionURL
* URL to the execution on DevOps Test Run

--appConfig
* Overrides for App.Config

--testPlanStructure
* Structure of azure Test Plans, indicates folder structure

--testerContact
* Email or contact for the tester to be reached at

--testerName
* Name of the tester executing the code

--releaseEnvUri
* Azure DevOps release environment uri separated by a comma


### Table of Test Set/Case/Step Arguments
|DataName|TestType|Arguments|Examples|
|---|---|---|---|
|XML|Set/Case/Step|.xml File location|--setType "XML" --setArgs "C:\XMLDocs\Test.xml"|
|Txt|Case/Step|.txt File Location|--setType "Txt" --setArgs "C:\TxtDocs\Test.txt"|
|Database|Case/Step|Collection,Release| --caseType "Database" --caseArgs "1,3"|
|ALM|Set|Test Set ID OR Test Set Name|--setType "ALM"  --setArgs "12345"|
|Excel|Set/Case/Step|.xlsx File Location| --setType "Excel" --setArgs "C:\ExcelDocs\Text.xlsx"|
|||Optional arguments, choose test case|--setType "Excel" --setArgs "C:\ExcelDocs\Text.xlsx;Testcase3"|
|None|Set|Test Case Name|--setType "None" --setArgs "Test Case Name"|


## Dependancies

* [TestingDrivers](https://github.com/zzzrst/TestingDrivers) - Removed
* [AutomationTestSetFramework](https://github.com/zzzrst/AutomationTestSetFramework)
* [DatabaseConnector](https://github.com/zzzrst/DatabaseConnector)
* [TextInteractor](https://github.com/zzzrst/TextInteractor)
* [ComparePDF](https://github.com/zzzrst/ComparePDF)
* [ALM](https://github.com/zzzrst/ALM)

## Version Number
The version number for this repository follows (MAJOR).(MINOR).(dailyBuild).(buildTime).

Major version is updated when a large functionality has changed / is introduced.

Minor version is updated when methods are added / removed for bug fixes or a helper class is added due to refactoring.


# Release Notes

## Version 6.0.0 (December 22)
- configure test runner available for eveyrone
- Working version with correct alignment
- update formatting and alignment
- Update ALM Connector and AxeAccessibility from netstandard to dotnet 6
- configure yaml files to latest version of dotnet
- wait after clicking to ensure ready state
- Updated System.Management
- update versioning of nuget packages
- rm mentions of playwright
- rm nunit mentions
- earlyon away from the list
- improve verifications to ensure that exceptions are handled
- button clicking typo
- remove unused code for login
- App config modify to work with any org
- sync api docs
- modify exception handling and values of DDLs
- move usings away from namespace for many files
- select radio buttons with correct handling of exceptions
- remove devops unused files
- configure chrome driver to 114, modify kill process
- move back chromium to 114 due to issues with save file
- fix firefox compatability: already private browsing
- update solution to not require the other folders
- Add test runner and also add app.config for test runner
- test runner allow runs unlimitted from the ui

## Version 5.5.5 (December 19)
- add file for demoing how to execute on an agent
- updated result code
- check elemeent state after clicks
- change status of run to aborted after sigint signal
- add log for finished aoda and determine why aoda has problems
- fix: if the email is an empty string, then we will not send any emails
- rm unused files causing errors, update to 5.5.2, update selenium
- Upgrade Selenium.AxeDriver to Deque's Selenium Axe
- add environmentlist table to devops
- Login and DatabaseStepData fix to remove dependencies
- configure executions to not have to have any reporting to devOps
- demo of having no reporting to devops
- sync docs, scripts
- update testing driver nifo to latest versions
- sync old driver
- added notes on the new testing driver
- add notes, comments, and docs for TestingDriver project
- update warnings for SeleniumDriver
- configure spacing and warnings
- configure correct browser sizes for mobile testing
- ensure no dependencies on TestingDriver with App.Config
- rm System.Configuration from Testing Driver file
- rm unused app.config
- fix auth issues with authenticating into AAD
- change from C:\TEMP to %dir%\TEMP (aka current working directory)
- merge conflict
- update merge conflict
- Modify text file close file after use
- comment out wait
- Should fix the temporary folder not being created, also rename from temp to temporary_files
- update migrator readme
- fix alm Save to file path back to C:\TEMP
- keychain file to TEMP folder
- fix typo
- roll back changes for Axe due to issues
- Execution commands modify a little bit
- merge changes regarding TEMP Files
- update saveFile and filepath resolver
- update to delete temporary files folder after execution
- Configure temp files to be definable in temporary folder
- update to use appconfig variable instead of hard coded
- Revert "Configure temp files to be definable in temporary folder"
- create release script
- added create release ps1 with ability to create releases and parameters
- correctly create releases
- sync changes to ps1 scripts
- Merged PR 10996: Pull changes for TEMP folder to Development
- increase version

## Version 5.5 (December 11)
 - initial fixes to the reporting mechanisms
 - fix identity ref to have the right identity for the users
 - ability to add test step attachment to test step
 - added ability to report to specific test step with correct attachment
 - added ability to specify who's testing the program
 - add release uri to the deveops execution release uri sync
 - correct iteration time in devops
 - correct actual results to be viewable in DevOps results
 - fix expected and title location
 - fix ordering of test cases and naming conventions
 - introduce update analysis owner
 - Fix reporting to DevOps with screenshots, actual results, expected results
 - fix for problems with no value for the release content
 - fix tester name


## Version 5.2 (December 5)
- Added filepathresolver to verify excel file
- remove unused code and whitespace
- sync changes with delete empty lines and unused code
- fix pr sql headers for comments
- verify email fix to verify from network drive
- sync xml file
- git ignore update to avoid ignoring files for docfx
- sync docfx files
- for USPP, field is F for data_exist, so remove
- verify email and for now remove check if not an email
- verify email remove unused code
- fix parsing of gotostep error message
- if file doesn't exist, then we will also append the file path name
- add ability create tests into SMOKE or REGRESSION folders, respectively
- add ability for users to overwrite app.config values from the command line as parameters
- fix test plan inside folders to only be created based on assumption of no other test sets with the same name
- sync auto created apis
- if choose collection fails, handle exceptoin
- Removed test plan name and test project name away from EnvVars
- excel set data parse set parameters prepare
- fix email sending to send without random values not necessary
- Working version of folder structure
- Sync latest version numbering
- fix unused code and sync changes
- handle exceptions gracefully
- Update chromium to v119

## Version 5.0 (November 24)
-  Rename test case names to Automated Test generated by Testing Automation Program
-  Test Step type 2 make it the default
-  Ability to use RunParameters and SaveParameters from the command line
-  if param is string.Empty, then ignore and continue
-  Added execution URL to the HTML reports
-  Update HTML reporter to report also the link
-  Deleted ALM_TestCase_Migration, as it's not used
-  fix errors with reporting the correct order for tests cases to the HTML reporter
-  Added feature to override SaveParameter by specifying "y" under comments column
-  fixed new development execution html files with links to accurate test suite, test plan, test run
-  Report to DevOps is implemented, fix comment
-  Fix verify web element content issues with parameter does not exist. Requires tseting
-  added envvar and build number to the execution head for mailjet
-  Here we'll save files at a lower res to avoid errors with the bitmap taking up too much space and memory issues
-  If BitMap creation fails, then continue and create one file instead with one photo
-  Include environment in test run name
-  Fix excel case data to allow for both lower and upper case values for excel execution
-  Fix Delete template to avoid errors with multiple data files
-  initial development of accordion
-  results column align for all executions
-  hide results column right after the failure
-  Configured working HTML removing unused parts for the email and sending correctly
-  rm save to C Temp and testing add attachment
-  Configure HTML reports to also contain failed test step result
-  fix excel report ordering
-  Working version of test step reporting
-  fix colour of devops test case warning
-  fix typo causing result for test step to take test case
-  start at step number 1
-  sync to read DevOps release URL
-  attach files for Convert PDF
-  If test step 0 or null, set it to default to 2
-  Change version to 5.0

## Version 4.5 (November 16)
  -  Include machine information in the html report
  -  print out the test run link for everyone to use
  -  Create ActionOnObject for upload data file
  -  Added ability to run from files locally stored to remove -dependency on K drive
  -  added data files folder
  -  If outcome == failed, write Awaiting Analysis instead
  -  remove comment
  -  If not should execute, then don't set args
  -  Fix close tab ability
  -  When searching for Action On Objects, remove all spaces to find -match. This will help with errors involving spacing in Actions
  -  raise error message when format of GoToStep is incorrect



## Version 4.0 (November 10)
- handle unhandled exceptions in framework driver
- Configure blocking after failure of tests onto DevOps.
- remove comments for Test Set, Test Case, and Test Steps
- headless mode should be false
- block all remaining tests after login failure
- Update so that Failed Login and RunSQL scripts should only have max - number of attempts set always
- Update so that login faillure and sql scripts can only have up to - one failed run before continuing.
- instead of storing temp_tap_exec in C:\TEMP, move it to local log - folder. Log folder will not be overwritten
- explicit wait so that button clicking is better
- Configure emails to send to uft and victor.zheng
- configure emails to be sent to email list command line arg
- Added Get WebElement Text information resulting data.

## Version 3.5 (November 8)

 - Fail test set after auth failure and change of password. Also - implement immediate failure.
 - Merged PR 10737: Auto Reset Password feature
 - change reporter for testing using a different organization - defined in App.Config.
 - Fix login functionality waiting
 - created private key values for email for UFT
 - show GB for report
 - Run mailjet with email specified
 - Update FindElements and VerifyElementText to have timeout - parameter
 - update interface to be correct for timouet
 - configure local timeout for executions workign
 - don't print out the local timeout
 - Configured Maximizing browser and maximize only at one point in - time, also configured headless executions
 - added attachment for executed file
 - catch unhandled exceptions involved with wrong file written down
 - Add sql script for pr sql run
 - fix executing a sql script from a network drive by copying the - file to the executing assembly


## Vesrion 3.4 (November 1, 2023)
- Upgrade to .NET 6 for lal projects
- print out framework type and execution format fo pdf
- Fix attaching CSV file of execution to output
- Configure reporter to include the PDF report execution
- remove OLD error screenshotting
- Verify WebElement Content also Verify TextBox Content code.
- Fix for clicking buttons
- remove full screen maximization after every AODA run and add comment for fullscreen
- Very basic emailing functinoality using MailJet
- fix date time of the execution and the resutls for the test case and test set
- Configure sending additional information and links to the tests on DeVOps
- hotfix delete and then re-add test suites instead of only updating test suites
- launch browser alert if url is incorrect
- remove old close tab
- add comment for api token for mailjet
- launch tab functionality added
- close tab and close browser functionality fixed
- if the temp file exists for the temp location then delete
- configure pdf reporting better format
- create better notes for sql scripts
- catch exceptions for decrypting passwords
- Verify Object Trim whitespace and change Enabled==Clickable, Disabled==Disabled, Exist==Visible, Does not exist == Invisible
- Send keys fixing problems with unable to refresh. Requires further investigation
- Rm commented dependencies
- add note/comment regarding changes to verifying link
- Added feature to Login actiononobject where it resets the password if login fails

## Version 2.6 (October 24, 2023)
- Fix Select DDL printing out more info, and print out screen res for OpenSIMS testing
- Create test suites using the original test suite and remove date/time execution stamp
- Handle console interrupts
- comment out LoginBPS since we use Login only now
- print out test plan instance
- print basic test run info into html reporter
- Fix for test plan instance not getting updated for tests that we do not want to add to DevOps/are deleted
- configure execution results better to report results with details


## Version 2.5.9 (October 17)
- fix verify webelement as we were not verifying properly before
- added optional value for secret creds
- configure logging in to AD using provided creds of user
- Fixed verifywebelement content action
- configure login to work for both BPS and GoSecure
- action on object for taking screenshot
- fix for values that are trimmed for the keychain file
- console writeline parameter print
- added debug mode to app.config and print out values during execution
- added links tag to app.config for whitelist for links
- Wait until page is loaded and also rm screenshot print height/width
- select value in element also wait for page to complete
- Overloaded method VerifyElementText to give an option to use 'value' attribute of html element - to allow textareas to have text verification
- Rename to VerifyFieldValue for clarity
- Update Select DDL multi values update name
- added ability to specify PAT via command line
- login aad combined with login action on object
- Fix login AD by changing xpath builder to not use double quotes with backslash
- Reviewed code and removed try except block for if/else block
- Merged PR 10540: LoginAD combined with the Login Actiononobject
- Created reporter that will not need to report to devops. It's now set as an App.Config param
- configure logging in for AAD so that it takes either the secret value or the value provided by the DefaultAzureCredentail
- added fix for conditional step where its the last test step in test case and we should check if a GoToStep is initialized
- configure maximum number of loops before failing and maximum consecutive failed test cases
- if go to step is not the same step, then report error
- increased speed of creating test cases and test steps
- upload 50 test cases at a time instead of hundreds to prevent errorsr
- fix for generating new test cases and reordering
- Configure Enter AAD Credentials ActionOnObject to allow us to login to OpenSIMS
-  select DDL value in element fix so that click is outside select by text
- Select Lookup ActionOnObject handle exception
- console write number of seconds waited
- report GoToStep, Type, Object, and Value to DevOps with better naming convention and more details
- hotfix: edge case involving test cases generating when skipping tests
- hotfix fix test case instance to be skip greater than test cases

## Version 2.5.5 (September 30)
- functionality for exporting alm folder 
- hotfix: since max queries are reached, we will use the same query called "Query Test Cases going forward"
- fix for querying so that querying only updates the existing query 
- hotfix for expiry of PAT
- comments for unused files
- unique identifiers at the start of execution
- add log folder in build
- Changes on Sept 29
    - created templates for SQL script execution for PR
    - configure automatic running as incog
    - fix maximize browser
    - launch firefox and edge in incog mode
    - provide a fix for executing tests with excel empty values
    - configured correctly checking if there exists a test step using trim
    - fixing click button for 3.5 secs and excel data empty fields - for figuring out if new test step exists
    - added test for key vault auth
    - keyvault auth include correct libraries in TAP
    - create functionality for OPS BPS login
    - Created Sharepoint getter class, although it doesn't work due - to CSOM and Azure. Will need to investigate PnP.Framework / PnP. Core later
    - Functionality for Sharepoint Getter for querying db
    - added package and small code for future addition of querying - for build data
    - configure logins using BPS, AD, and regular login
    - configuration for ability to execute with App.Config urls if - the db cannot find value


## Version 2.5 (September 15th, 2023) 

- Fix for GoToStep to allow for going backwards and reporting results to DevOps. ability for it to run, but the DevOps reporting is difficult 
- AODA reports using the latest file format | latest file format added 
-  Ability to generate test plans on execution with names specified as a parameter. Test Suites would go under the main test plan. Multiple test suites under one test plan would become possible.  
- Generate and execute multiple tests under one run via powershell functionality
- - Fix ability to execute on DB, ie. specify ALM to get test case details and still work 

- Configure attaching logs on execution inluding CSV files
- - Assign file names correctly with detailed description -  with date and file name onto DevOps 
  - Playwright functional ability to run and create suites. 
- Fix order of test runs displayed on DevOps | Order is displayed as Test Case id number order 
- Mandatory validation step that requires a step to ensure tests are executed. Another column added triggering execution 
- Ability to ensure that info in Test Case Action type is correct in the excel. Similar to DB triggers to ensure that the values are being used. 
- Testing functionality for Edge, Firefox, Safarii/webkit | Tested and fixed for Edge and Firefox execution. Will require future safarii testing.   
- SQL template that will auto populate and get the latest SQL info. will also delete instead of update 
- Button for generating all tests from ALM to Excel 

## Version 2.0 (August 31st, 2023) 
- introduce excel executions functionality
- Ability to integrate with DevOps API and generate executions with it
	- Test Step, Case, Suite, Plan, and Runs are accounted for
- Basic ability to use Playwright features. 
- Creates Test Steps on execution and publishes to DevOps. 
- On failure, moves to the next test case
- Test can be run on DevOps simply by specifying the place for execution. 
	- No need to click more than 3 buttons
	- Have reports stored on Logs, generate AODA reports using new file format (although format needs to be updated soon)
- Functional ability to use paramters at run time using SaveParameters and brackets {}
- Ability to use Full Screen screenshotting 
	- images concatanate with each other to create one major image. 
	- last image may be slightly cut, so it is something that may need to be fixed
- Azure Agent configuration
	- Currently able to execute onto azagent of over 15 agents on approx 5 machines 
- Ability to use vstest to run tests
- Deployment onto C:\TAP for all agents configured to be able to run.
- fix for configurations for EDCS applications
- Ability to associate a test suite with more than one test case
- Configured so that last test run is able to be executed
- ALM migrator button on ALM
- ALM migrator to combine more than one test set
- ALM migrator to create excel files that have the test set information filled out. 
- ALM migrator to generate a list of test sets
- ALM migrator for more than one test set at a time based on both network path and test set ids
- DevOps connection project to test api calls for DevOps
- Display test step information on Devops
- Fix network path problems with K drive for select actiononobjects	
	- Run SQL, Run Shell, Run other scripts
- Change keychain folder path back to newtork path
- fix RunSQLScript to allow double quotes
- Skip Step functionality
- Fix for check boxes so that it correctly clicks ON or OFF flag
- Congirure working CUNCE while regression testing
- Add all sql scripts and test sql script execution. 
	- began creating an actinonobject for parameterizing the sql script
- Create ActionOnObject for setting an org from Non-Participating back to Participating
- Ability to display run results in a CSV file. 
- Wait for 10 seconds between each verification step
- Fix loading spinner
- Warning for run Test Case
- Removed Verify SQL because not used
- Added Picasso application mappings in App.Config
- Add traceability matrix for CUNCE
- Choose Collection fix for choosing xpaths only
- Query keychain and overwrite old file into %dir%\temporary_files
- Fix for disabled item flag since disabled items was spelt wrong
- Tested EarlyON with TAP
- Fixed Conditional executions and optional test cases
- Created batch execution of TAP
- Handle exceptions when querying for DB results in nothing returned
- Added HTML whitelist information and configuration correctly
- Query DB data for test environment and PR environment information
- Ability to continue after failing to go to the next available test case
- ALM Migrator to get a list of all used values in the DB.
- Option 1 mandatory test case working
- fix for reporting time of execution
- choose collection fix
- Port AxeAccessibility driver into the project and update to latest version.
- Fixes for connections to avoid 507 and 407 errors.

## Version 1.0 (July 31st, 2023)
- Ability to use Oracle DB to query and get test case data. 
- include details on which test attempt we are on, and result
- tested ability to launch in incognico mode chromium
- Ported Testing Driver into project
- Output tool to C:\TEMP for ALM Migrator
- remove unncessary logging for Logger.Info
- removed unused migration files
- Get all test case info from DB using UNION ALL
- Exit code 1 for failures
- Comment not used prints
- Use same format for Excel as what is used in DB
- Add selenium wrapper from TFS and run.
- Run using bat files
- Fixed ability to not exit when fail mandaotry test step
- populate test box fix
- git ignore for some files
	- ignore api files
- Fix MapEmailFolder, added FilePathResolver from SeleniumFramework. 
- Added SeleniumFramework into the AutomationAndAccessibility project. 
- Fixed choosing collection based off of not only xpaths
- Fix email notification and getting an email shortlist
- Update AODA to correctly generate AODA zip file and create results.
- Fix for login errors and launching browser errors. Fix for excel format
- Add buildNumber functionality into execution
- fix clicking innertext problems due to not having a HTML whitelist (major problem)
- CI for Azure pipelines
- Change keychain file path to correct file path
- Sync initial files 
- Project start date: Jun 27, 2023
