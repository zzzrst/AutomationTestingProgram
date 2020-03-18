# ITestData
There are four interfaces in this part
## ITestData
This is what all the other test data implements
### Variables
```c#
string Name
```
* The name to use when you want to use this data type from the 
cmd. ie, if the test set data was called "XML" then when running in cmd, the test set data parameter will be "XML" too.
```c#
string TestArgs
```
* Arguments that are passed in. Often the location of the file.
### Methods
```c#
void SetUp()
``
* Ran when the test data is initilized. Can be used for loading the test set file into the program.
## ITestSetData
### Methods
```c#
ITestCase GetNextTestCase()
```
* Gets the next test case. 
* Often needs to call `InformationObject.TestCaseData.SetUpTestCase(string,bool)` since that is where the test case data is located.
```c#
bool ExistNextTestCase()
```
* Checks if the next test case exists.
```c#
void SetUpTestSet()
```
* Ran before it is called at the start of the program. 
* You can use it to set up your logic for the test set.
## ITestCaseData
### Methods
```c#
ITestStep GetNextTestStep()
```
* Gets the next test step. 
* Often needs to call `InformationObject.TestStepData.SetUpTestStep(string,bool)` since that is where the test step data is located.
```c#
bool ExistNextTestStep()
```
* Checks to see if there is a next test step.
```c#
ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
```
* Usualy ran when the next test case is needed.
* You can also set up any logic needed in the test case here. 
## ITestStepData
### Methods
```c#
ITestStep SetUpTestSet(string testStepName, bool performAction = true)
```
* Usualy ran when the next test step is needed.
* You can also set up any logic needed in the test step here .