# Adding More Test Data
When adding your own data to the existing program, please keep in mind the following. Also refer to the ITestData page for what each method and class should do.
1. Add a new Class to "src/TestingData/TestDrivers".
1. Make sure the name of the data does not already exist in the program. There is currently no way of keeping track of what names has been used. Usualy the name is the same as the data type.
2. When making a `testSetData`, also make a `GeneralData` class with the same name as the `testSetData`.
3. It is possible for an implemntation of a test driver to implement all 3 testData interface. Keep in mind the program does create a new instance for each of the test case, so don't assume the `testSetData` object is the same as the `testCaseData` object.
4. You can also create a class that implements ITestData which your data can implement. This is so that you can have a base class that the test Set/Case/Step can implement. The implementation tree would look like this:

|Interface|<-|Implementation|
|---|---|---
|ITestData | <- | ExampleData|
|^||^|
|ITestSetData|<-|ExampleSetData|
|ITestCaseData|<-|ExampleCaseData|
|ITestStepData|<-|ExampleStepData|

**Key**
* <- Represents implementation
* ^ Represents implementation of the column name