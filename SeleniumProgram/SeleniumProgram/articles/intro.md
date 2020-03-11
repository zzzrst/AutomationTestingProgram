# Welcome!
This page contains articles about the implemntation for the Automation Test Program as well as how to add your own modules to it.  
Please refer to the [UML Diagram]() for a better visualization of the source code.
## Testing Data
This is how you get what test set/case/step to run.
## Automation Framework
This is the skeleton of how the program runs. Basicaly acting as the controler/link for the Testing Data to Test Automation Driver.
## Test Automation Driver
This is where the testing actualy happens. It takes commands from the testing data and excecutes them.

## Running the program.
You need to pass in arguments when running this through command line. Below are arguments that you can use.

Mandatory
* --setType
  * The type of data to use. ie "XML" or "txt"
* --setArgs
   * The arguments to be passed in. Often the location of the file.

Optional
* --b or --browser
* --e or --environment
* --u or --url
* --respectRepeatFor
* --respectRunAODAFlag
* --timeOutThreshold
* --warningThreshold
* --dataFile
* --csvSaveFileLocation
* --logSaveLocation
  * Defaults to the csvSaveFileLocation
* --reportSaveLocation
  * Defaults to the csvSaveFileLocation
* --screenShotSaveLocation
  * Defaults to the csvSaveFileLocation
* --automationProgram
  * The type of testing program to use. ie "selenium"
* --caseType
  * Defaults to setType
* --stepType
  * Defaults to caseType
* --caseArgs
  * Defaults to setArgs
* --stepArgs
  * Defaults to caseArgs
## Environment Variables
Most of these variables are the parameters passed in as well as some additional parameters from test sets.
* browser
* environment
* url
* respectRepeatFor
* resepctRunAODAFlag
* timeOutThreshold
* warinigThreshold
* dataFile
* csvSaveFileLocation
* logSaveFileLocation
* reportSaveFileLocation
* screenshotSaveLocation
* testAutomationDriver
* testSetDataType
* testCaseDataType
* testStepDataType
* testSetDataArgs
* testCaseDataArgs
* testStepDataArgs
* loadingSpinner
* errorContainer
## Information Object
There is a static class called InformationObject. It contains static variables that any class can access. They are as follows:
```c#
bool RespectReapetFor;
bool RespectRunAODAFlag;
string LogSaveFileLocation;
string ScreenshotSaveLocation;
CSVLogger CsvLogger;
ITestSetData TestSetData;
ITestCaseData TestCaseData;
ITestStepData TestStepData;
ITestAutomationDriver TestAutomationDriver;
Reporter Reporter;
```
## Test Flow
The order of execution is as follow. Note that Some methods appear both in AutomationFrameWork and TestData. Automation Framework is the skeleton of how the program is ran. So For example, `GetNextTestCase()` Will first be ran in the automation Framework's TestSet class which will then call the TestSetData from the information object to run it's own `GetNextTestCase()`. The only exception is `SetUp()`. The automation framework's `SetUp()` is ran when that instance is created.

* START
* Parse Command Line Arguments. *The test Set type and args must be provided.*
* Parse test set parameters.
  * Gets the `TestGeneralData` that corresponds to the TestSetType and runs `Verify()` and `ParseParameters`
* If parsing the parameters was successful, run the following
* Set up the Information Object and set its variables to the correct value.
* SetUp the Testing Datas
  * Create a new `TestSetData` based off of the `setType` parameter and pass in the `setArgs`.
    * Run `SetUp()` for test set.
  * Run `SetUpTestSet()`
  * Create a new `TestCaseData` based off of the `caseType` 
  parameter and pass in the `caseArgs`.
    * Run `SetUp()` for test case.
  * Create a new `TestStepData` based off of the `stepType` parameter and pass in the `stepArgs`.
    * Run `SetUp()` for test step.
* Setup and create the automationDriver based off of the `automationProgram` parameter
* Run The test Set
  * Run `ExistNextTestCase()`
  * If `True` Run `GetNextTestCase()`
    * `GetNextTestCase()` will call `SetUpTestCase()` *(This is based off how you should implement it but it can change)*
    * Run the Test Case
      * Run `ExistNextTestStep()
      * If `True` Run `GetNextTestStep()`
          * `GetNextTestStep()` will call `SetUpTestStep()` *(This is based off how you should implement it but it can change)*
          * Run `ShouldExcecute()`
          * If `True` Run `Execute()`
          * Also Run from TestAutomationDriver, `RunAODA` if enabled.
      * Report the Test Step to the Reporter
    * Report the Test Case to the Reporter
  * Report the Test Step to the Reporter
* END