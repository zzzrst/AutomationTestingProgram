# Reporter and Loggers
## Reporter
### Default Reporter
There are 3 Lists that take in a TestStatus.
```c#
public List<ITestSetStatus> TestSetStatuses { get; set; }
```
* There is usualy only one object in this list since you will usualy only run one test set.
```c#
public List<ITestCaseStatus> TestCaseStatuses { get; set; }
```
* This is the list of all the test cases.
```c#
public Dictionary<ITestCaseStatus, List<ITestStepStatus>> TestCaseToTestSteps { get; set; }
```
* This maps the test case to the list of all test steps it ran

There are 4 Methods that the report uses. These methods can be overrided by another reporter class. See ALM Reporter
```c#
public virtual void AddTestCaseStatus(ITestCaseStatus testCaseStatus)
```
* This is called after the test case is ran.
```c#
public virtual void AddTestSetStatus(ITestSetStatus testSetStatus)
```
* This is called after the entire test set is ran.
```c#
public virtual void AddTestStepStatusToTestCase(ITestStepStatus testStepStatus, ITestCaseStatus testCaseStatus)
```
* This is called after the test step is ran. The test case status is the current test case.
```c#
public virtual void Report()
```
* This is ran at the very end of the program.

## Loggers
Each Test Set/Case/Step has a log function that runs at the end of excecuting the test Set/Case/Step
```c#
public void Log(ITestSet testSet)
public void Log(ITestCase testCase)
public void Log(ITestStep testStep)
```
Use this to log the status of the test set/case/step.

## Built in Loggers
There are a few built in loggers you can use in replace of Console.WriteLine
### Logger (Class)
```c#
public static void Debug(object message)
public static void Error(object message)
public static void Fatal(object message)
public static void Info(object message)
public static void Warn(object message)
```