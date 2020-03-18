# TextData
Text Data takes test cases/steps from .txt files and excecutes them. This class should NOT be relied on as an actual test case and step as it was made to test the ability for different types of test case/steps. In this case it was testing the compatibility of the XML test set and txt test case/steps. The name of the data is **"Txt"**
## TestCaseData
* `TestArgs` Should be the location of the txt file whos name is the name of the testCase and the test steps are in the test case.
* Example

OpenClose.txt
```
Opening Browser
Closing Browser
```
* The text data will run through all the test steps in the order they are presented.
## TestStepData
* `TestArgs` Should be the location of the txt file that contains *All* the test steps that will be ran. This is not specific to any test case.
* Example

TestStep.txt
```
Opening Browser;OpenBrowser;name=Hello World - Opening Browser
Closing Browser;CloseBrowser;name=Hello World - Closing Browser
```
* Each test step should be in the format
```
Name;NameOfTestStepObject;Arguments
```
* Arguments are in the format
```
argName=value
```
or
```
arg1Name=value1,arg2Name=value2
```
* There are no spaces between the ";","=", and ",".