# ITestGeneralData
This class is usualy only used at the start of the program. It helps set up the test set and the test flow.
## Variables
```c#
string Name
```
* The name of the data type.
* This is the same name as the test data that will be used.
## Method
```c#
bool Verify(string testArgs)
```
* Verifies to see if the data is correct. 
* `testArgs` is the same as the testData variable `TestArgs`.
```c#
Dictonary<string, string> ParseParameters(string testArgs, string dataFile)
```
* Gets any parameters from the data file and uses those parameters if they were not already overwritten by the command line arguments.
* `testArgs` is the same as the testData variable `TestArgs`.