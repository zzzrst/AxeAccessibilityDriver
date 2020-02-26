# AxeAccessibilityDriver
### This repository contains the source code for:
- The .Net Standard 2.0 Axe Driver implementation for Selenium. 
- The .Net Core 2.2 Tester to run the Accessibility Driver.
# Overview
Please take a few minutes to review the overview below before diving into the code.
## Interfaces
### IAccessibilityChecker
Each Accessibility Implemntation must have the following two methods:

```c#
    void CaptureResult(IWebDriver driver, string providedPageTitle);

    void LogResults(string folderLocation);
```
## Implementations
### AxeDriver
Takes in a Selenium WebDriver and analyzes according to AODA. It will generate in a zip file 2 csv files for the result data, a json file and a WATR Report.

## Templates
The currently used template is the WATR_Template.xlsx. The program will add information to the template as needed.