# How To Run
Implement any IAccessibilityChecker in your code.

Example
```c#
    public void RunAODA(IWebDriver driver)
    {
        IAccessibilityChecker checker = new AxeDriver(driver);
        checker.CaptureResult("Title");
        checker.LogResults("/result");
    }
```

Below are the current Implementation you can use:
## Axe Driver
```c#
public AxeDriver(IWebDriver driver)
public AxeDriver(IWebDriver driver, ILogger logger)
```