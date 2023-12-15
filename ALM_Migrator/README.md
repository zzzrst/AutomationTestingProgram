## What this project is about.

This project contains functionality that helps us export items from ALM, Database, and into DevOps.

Items that will require import into DevOps include: 
* Test Cases and their execution flows in the correct order
* Test Set IDS
* Test Steps and each of their items
* Test Defects

## OPTION 0: No parameters
If someone does not specify a paramter, then it will do OPTION 1 below with only one test set id. It is used in MicroFocus ALM as a way to export individual test sets. 

## OPTION 1: **For getting test set info based on ALM test set ids**
To run the exe, simply type .\ALM_Migrator.exe 1 false {TESTSETID} {TESTSETID}

It takes infinite number of test set ids, separated by spaces as arguments. 

Test Set Information sheet contains the test sets that were generated, and the execution order.

The result is stored in C:\TEMP with the id, collection, and release separating each excel file. 

If you want to concatenate multiple test sets, use the value true.

eg. .\ALM_Migrator.exe 1 false 93436

* The second parameter, is a boolean response input (true/false) indicating whether or not to concatenate results together. If true, then the alm ids will be copied into the first ALM id plus "FULL". 

## OPTION 2: **For getting test set info based on ALM Network Path(s)**
To run the exe, simply type .\ALM_Migrator.exe 2 false true "ROOT\Open SIMS"

It takes infinite number of network paths and unlimited test sets under the directory. The second parameter behaves the same as option 1. See above for details.
The third parameter indicates whether or not to recursively search all subfolders for test sets. Note that recursive search may place test sets in an unexpected order.

The result is stored in C:\TEMP with the id, collection, and release separating each excel file. Test Set Information sheet contains the test sets that were generated, and the execution order.

If you want to concatenate multiple test sets, use the value true.

eg. .\ALM_Migrator.exe 2 false "ROOT\Open SIMS"

eg.  .\ALM_Migrator.exe 2 false "Root\1. Patches and Releases\2023-06 June Release\EarlyON 2.0 (code freeze May 15)" 

* The second parameter, is a boolean response input (true/false) indicating whether or not to concatenate results together. If true, then the alm ids will be copied into the first ALM id plus "FULL".

## OPTION 3: **For getting test set id list based on ALM Network Path**
To run the exe, simply type .\ALM_Migrator.exe 3 "ROOT\Open SIMS"

Note that from testing, you cannot use only the ROOT folder. Likely because it overseeds the maximum size of a list. However, "ROOT\\1. Patches and Releases" works with 56995 test set ids as of July 31st works fine. 

The result is stored in C:\TEMP with the name of the path indicated. 

eg. .\ALM_Migrator.exe 3 "ROOT\Open SIMS"

eg.  .\ALM_Migrator.exe 3 "Root\1. Patches and Releases\2023-06 June Release\EarlyON 2.0 (code freeze May 15)" 

## OPTION 4: **Query distinct items based on column**
To run the exe, simply type .\ALM_Migrator.exe 4 ACTIONONOBJECT

This returns an excel in C:\TEMP that includes all unique values in the DB. 

eg. .\ALM_Migrator.exe 4 TESTCASE TESTCASEDESCRIPTION STEPNUM ACTIONONOBJECT COMMENTS RELEASE TESTSTEPTYPE GOTOSTEP