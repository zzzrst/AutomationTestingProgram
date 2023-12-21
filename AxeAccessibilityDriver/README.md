# AxeAccessibilityDriver
### This repository contains the source code for:
- The .Net Standard 2.0 Axe Driver implementation for Selenium. 
- The .Net Core 2.2 Tester to run the Accessibility Driver.
# Overview

Please take a few minutes to review the overview below before diving into the code.
## Documentation
For api documentation, please refer to [here](https://zzzrst.github.io/AxeAccessibilityDriver/api/index.html).
For examples of usages, please refer to the [article section](https://zzzrst.github.io/AxeAccessibilityDriver/articles/intro.html).

## Interfaces
### IAccessibilityChecker
Each Accessibility Implemntation must have the following two methods:

```c#
    void CaptureResult(string providedPageTitle);

    void LogResults(string folderLocation);
```
## Implementations
### AxeDriver
Takes in a Selenium WebDriver and analyzes according to AODA. It will generate in a zip file 2 csv files for the result data, a json file and a WATR Report.

## Templates
The currently used template is the WATR_Template.xlsx. The program will add information to the template as needed.
## Other Sources

* [Description of AODA Rules](https://github.com/dequelabs/axe-core/blob/develop/doc/rule-descriptions.md)
* [Axe Core Source Code](https://github.com/dequelabs/axe-core)
* [Selenium.Axe Source Code](https://github.com/TroyWalshProf/SeleniumAxeDotnet)

## Other notes
Version number are labed as MAJOR.MINOR.dailyBuild.buildTime  
Major changes are when new classes are added, except helper classes, or when a large change is made to the code such as a large functionality change.  
Minor changes are when methods are added/removed or a helper class is added.
