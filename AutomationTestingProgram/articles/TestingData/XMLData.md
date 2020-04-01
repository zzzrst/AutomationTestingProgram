# XMLData
* This will read the xml data from a .xml file and run the test set/case/step from it. The name of the test data is **"XML"**.
* The TestArgs point to the location of the .xml file to run.
* This is essentially a working copy of the [SeleniumPerfXML](https://github.com/zzzrst/SeleniumPerfXML).
* Refere to SeleniumPerfXML for writing test set/case/step.
* There is a generic class called XMLData which implements ITestData which all the XMLset/case/step Data implements. These are all the functions and variables that they have in common
## Mapping from XML attributes to Database column names
|XML Attribute|Database Column|
|---|---|
|xPath|object|
|useJS|comment|
|tabIndex|value?|
|selection|value|
|url|value|
|text|value|
|invisible|value?|
|seconds|value|
|collectionSearchField|
|collectionName|
