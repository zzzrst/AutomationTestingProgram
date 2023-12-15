# Testing Data
This is the part of the program which gets all the test sets/case/steps to execute. Each set/case/step can be extended from and you can create your own data file for them. The ones that are currently implemented is XML files for test set/case/step and txt file for test case/step.  
The 3 testing datas are stored in the static **InformationObject** as static variables **TestSetData**, **TestCaseData**, and **TestStepData**.
## Interfaces
* ITestData
## Implementation
* XML Data
* Database Data
* Text Data
* Adding other data files