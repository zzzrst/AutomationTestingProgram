# How It Reports
## AxeDriver
There are 3 main ways that the AxeDriver uses to report. It will create a zip file containing the following
### Json Folder
There will be 3 folders underneath the Json folder.
* Incomplete
* Pass
* Violations
The Json files are formatted as follow:
```json
{
  "Rule ID": "name",
  "Result Type": "Pass/Incomplete/Violations",
  "Description": "Description",
  "Rule Tag": [
    "tag1",
    "tag2",
    "tag3"
  ],
  "Impact": null,
  "Help": "help",
  "Help URL": "url",
  "Nodes": [
    {
      "Page URL": "page tested url",
      "Provided Page Title": "title",
      "Browser Page Title": "title",
      "HTML": "Html code",
      "Target": [
        "target"
      ]
    }
  ]
}
```
### Csv File
2 Csv Files will be created in the format:

RulePageSummary.csv  

|Page URL|Provided Page Title|Browser Page Title|Result Type|Description|Rule Tag|Impact|Help|Help URL|Occurance on page|
|---|---|---|---|---|---|---|---|---|---|  
|url|title|title|result|description|tags|impact|help|help URL|#|

TalliedResult.csv

|Page URL|Provided Page Title|Browser Page Title|Passes|Violations|Incompete|Inapplicable|  
|---|---|---|---|---|---|---|  
|Url|Title|Title|#|#|#|#|
### Excel File
1 Excel File Named WATRReport will be generated. It has 4 Pages. 

The first page is instructions for the WATR Report. It will not be changed.

The Executive Summary Page will have the Progress to 100% Comlpliance filled out as well as the Evaluation Date. Project Name, Evaluator, Testing Approach, Methods and Tools, Remediation Plan, and Related Documents Listing must be filled out by hand.

The WCAG 2.0 Compliance Checklist will Have the Date and most of the criterias filled in. Under the column "Metts Criteria", It may be filled as "Pass", "Fail", "Criteria not applicable", or "Requires Manual Testing". Thoses results in the same row as "Requires Manual Testing" Will need a QA to manualy check the page for those criteria. Don't Forget to fill the Project Name and URL at the top.

The Mapping from Axe to WATR Report is as follows

|AXE|WATR REPORT|
|---|---|
|Pass|Pass|
|Violation|Fail|
|Incomplete|Incomplete|
|Inapplicable|Criteria Not applicable|
|*Default*|Requires Manual Testing|

The Issue Tracking Log is formatted and filled out as follows:

|Defect No.|Date of Test|Url|Success Criterion|Defect Description|Impact|Status|Anticipated Resolution Date|
|---|---|---|---|---|---|---|---|
|#|date|url|criterion|description|impact|Current|To be Determined|

The mapping from Axe to the Impact is as follows:

|AXE|Impact|
|---|---|
|Critical|High|
|Serious|Medium|
|Moderate|Low|
|Best Practices||