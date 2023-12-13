// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALM_Migrator
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using ALMConnector;
    //using AutomationTestingProgram;
    using TDAPIOLELib;

    using Spire.Xls;
    using NPOI.SS.Formula.Functions;
    using Spire.Pdf.Tables;
    using static NPOI.HSSF.Util.HSSFColor;
    using NPOI.OpenXmlFormats.Spreadsheet;

    /// <summary>
    /// Main program.
    /// </summary>
    public class TestCaseMigrator
    {
        // functionalities available. See Readme.md for more info
        private const int TESTSETBYID = 1;
        private const int TESTSETBYPATH = 2;
        private const int TESTSETLIST = 3;
        private const int QUERYUNIQUE = 4;

        /// <summary>
        /// The Main functionality.
        /// </summary>
        /// <param name="args">Arguments to be passed in.</param>
        public static int Main(string[] args)
        {
            bool errorParsing;
            int resultCode = 0;

            errorParsing = ParseCommandLine(args);

            if (!errorParsing)
            {
                Console.WriteLine("Seting up the parameters");
            }

            Console.WriteLine($"Running AutomationFramework Version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");

            string username = ConfigurationManager.AppSettings["ALMusername"].ToString();
            string password = ConfigurationManager.AppSettings["ALMpassword"].ToString();
            string domain = ConfigurationManager.AppSettings["ALMdomain"].ToString();
            string project = ConfigurationManager.AppSettings["ALMproject"].ToString();

            Console.WriteLine("username: " + username);
            Console.WriteLine("password: " + password);
            Console.WriteLine("domain: " + domain);
            Console.WriteLine("project: " + project);

            // check if password is encrypted

            Connector alm = new Connector(username, password, domain, project);

            // for ALM button
            if (args.Length == 1)
            {
                Workbook workbookLog = new Workbook();
                workbookLog.Worksheets.Clear();
                Worksheet worksheetLog = workbookLog.Worksheets.Add("Test Cases List");

                // set the first cell row as a header
                int cellRowNum = 1;
                worksheetLog.Range[cellRowNum, 1].Text = "Test Set ID";
                cellRowNum += 1;

                DatabaseData data = new DatabaseData();
                data.ConnectToDatabase(data.TestDB);
                data.initializeWorkSheetDB();

                // iterate through all args and get report for each almID
                foreach (string almID in args)
                {
                    Console.WriteLine($"We are going to attempt to get the TestSetIDs for all test cases under {almID}");

                    // parse the almID into an integer
                    int parsed = Int32.Parse(almID);

                    // create a test set based on the alm ID
                    TestSetInstance testSet = alm.SetTestSetByUID(parsed);

                    // check if the test set is null and if it is, throw and return
                    if (testSet == null)
                    {
                        Console.WriteLine("Failed to set test set by uid");
                        throw new Exception();
                    }

                    // check if test set has no test cases
                    if (testSet.CurrTestCase == null)
                    {
                        Console.WriteLine("No test cases found. File will not be created");
                        continue;
                    }

                    CreateDatabaseData(testSet, almID);
                    // record the test set id into the worksheet log
                    worksheetLog.Range[cellRowNum, 1].Text = almID;
                    cellRowNum += 1;
                }

                string saveLogFile = "C:\\TEMP\\Report.xlsx";
                workbookLog.SaveToFile(saveLogFile, ExcelVersion.Version2016);

                return 0;
            }

            switch (int.Parse(args[0]))
            {
                case TESTSETBYID:
                    {
                        bool concat = bool.Parse(args[1]);

                        List<string> almIDs = args.Skip(2).ToList();

                        Workbook workbookLog = new Workbook();
                        workbookLog.Worksheets.Clear();
                        Worksheet worksheetLog = workbookLog.Worksheets.Add("Test Cases List");

                        // set the first cell row as a header
                        int cellRowNum = 1;
                        worksheetLog.Range[cellRowNum, 1].Text = "Test Set ID";
                        cellRowNum += 1;

                        DatabaseData data = new DatabaseData();
                        data.ConnectToDatabase(data.TestDB);
                        data.initializeWorkSheetDB();

                        // iterate through all args and get report for each almID
                        foreach (string almID in almIDs)
                        {
                            Console.WriteLine($"We are going to attempt to get the TestSetIDs for all test cases under {almID}");

                            // parse the almID into an integer
                            int parsed = Int32.Parse(almID);

                            // create a test set based on the alm ID
                            TestSetInstance testSet = alm.SetTestSetByUID(parsed);

                            // check if the test set is null and if it is, throw and return
                            if (testSet == null)
                            {
                                Console.WriteLine("Failed to set test set by uid");
                                throw new Exception();
                            }

                            // check if test set has no test cases
                            if (testSet.CurrTestCase == null)
                            {
                                Console.WriteLine("No test cases found. File will not be created");
                                continue;
                            }

                            if (concat)
                            {
                                data.writeTestSetDetails(testSet.Name, testSet.Description);
                                //data.writeTestSetDetails(testSet.Name, testSet.Description);
                                GenerateDatabaseInfo(data, testSet, almID);
                                data.resetTestCaseNum();
                            }
                            else
                            {
                                CreateDatabaseData(testSet, almID);

                            }
                            // record the test set id into the worksheet log
                            worksheetLog.Range[cellRowNum, 1].Text = almID;
                            cellRowNum += 1;
                        }

                        if (concat)
                        {

                        Validation(data.workbook);
                        data.SaveToFile(almIDs[0] + "_FULL");
                        }

                        string saveLogFile = "C:\\TEMP\\Report.xlsx";
                        workbookLog.SaveToFile(saveLogFile, ExcelVersion.Version2016);
                        break;
                    }
                // if we want to generate a report of test case info and test set info
                case TESTSETBYPATH:
                    {
                        bool concat = bool.Parse(args[1]);
                        bool recursiveSearch = bool.Parse(args[2]);
                        // split each network path by spaces. We can accept more than one
                        List<string> networkPaths = args.Skip(3).ToList();

                        Workbook workbookLogPath = new Workbook();
                        workbookLogPath.Worksheets.Clear();
                        Worksheet worksheetLogPath = workbookLogPath.Worksheets.Add("Test Cases List");

                        // set the first cell row as a header
                        int cellRowNumPath = 1;
                        worksheetLogPath.Range[cellRowNumPath, 1].Text = "Test Set ID";
                        cellRowNumPath += 1;


                        DatabaseData data = new DatabaseData();
                        data.ConnectToDatabase(data.TestDB);
                        data.initializeWorkSheetDB();
                        foreach (string networkPath in networkPaths)

                        {
                            Console.WriteLine($"We are going to get test case info for everything under {networkPath}");

                            // get all test sets by network path on Microfocus ALM
                            List<string> testSets = alm.GetAllTestSetByPath(networkPath, recursiveSearch);

                            // if there are no test sets in the root path, then we will return
                            if (testSets.Count == 0)
                            {
                                Console.WriteLine($"No tests were found with the provided path {networkPath}");
                                return 0;
                            }
                            else
                            {
                                // else, print out the number of test sets that were found
                                Console.WriteLine($"Total # of test sets found were: {testSets.Count}");
                            }

                            // iterate through each test set in the list of test sets
                            foreach (string almID in testSets)
                            {
                                // parse the almID into an integer
                                int parsed = Int32.Parse(almID);

                                // create a test set based on the alm ID
                                TestSetInstance testSet = alm.SetTestSetByUID(parsed);

                                // check if the test set is null and if it is, throw and return
                                if (testSet == null)
                                {
                                    Console.WriteLine("Failed to set test set by path");
                                    throw new Exception();
                                }

                                // check if test set has no test cases
                                if (testSet.CurrTestCase == null)
                                {
                                    Console.WriteLine("No test cases found. File will not be created");
                                    continue;
                                }

                                if (concat)
                                {
                                    data.writeTestSetDetails(testSet.Name, testSet.Description);
                                    //data.writeTestSetDetails(testSet.Name, testSet.Description);
                                    GenerateDatabaseInfo(data, testSet, almID);
                                    Validation(data.workbook);
                                    data.resetTestCaseNum();
                                }
                                else
                                {
                                    CreateDatabaseData(testSet, almID);
                                }


                                // GenerateDatabaseInfo(dataUnique, testSet, almID);

                                // record the test set id into the worksheet log
                                worksheetLogPath.Range[cellRowNumPath, 1].Text = almID;
                                cellRowNumPath += 1;
                            }
                        }
                        if (concat)
                        {
                            int indxLastBackSlash = networkPaths[0].LastIndexOf('\\');
                            data.SaveToFile(networkPaths[0].Substring(indxLastBackSlash + 1) + "_FULL");
                        }

                        string saveLogFilePath = "C:\\TEMP\\Report.xlsx";
                        workbookLogPath.SaveToFile(saveLogFilePath, ExcelVersion.Version2016);
                        break;
                    }
                case TESTSETLIST:
                    {
                        // this functionality provides a list of test sets
                        Console.WriteLine($"We are going to get a list of all test sets under path {args[1]}");

                        // get the full list of test sets
                        List<string> testSetsList = alm.GetAllTestSetByPath(args[1]);
                        if (testSetsList.Count == 0)
                        {
                            Console.WriteLine($"No tests were found with the provided path {args[1]}");
                            return 0;
                        }
                        else
                        {
                            // else, print out the number of test sets that were found
                            Console.WriteLine($"Total # of test sets found were: {testSetsList.Count}");
                        }

                        // create a workbook to write the test set ids to
                        Workbook workbook = new Workbook();
                        workbook.Worksheets.Clear();

                        // indicate the name of the spreadsheet
                        Worksheet worksheet = workbook.Worksheets.Add("Test Set IDS");

                        // set the first cell row as a header
                        int cellRow = 1;
                        worksheet.Range[cellRow, 1].Text = "Test Set ID";
                        cellRow += 1;

                        int count = 0;

                        // loop through each test set ID in the test set list and append to a new row
                        foreach (string testSetID in testSetsList)
                        {
                            worksheet.Range[cellRow, 1].Text = testSetID;
                            cellRow += 1;
                            count++;
                        }

                        // record the total number of test sets that were recorded in the excel
                        Console.WriteLine($"  A total of {count} test sets were recorded.");

                        // replace the slashes with underscores in the new file name
                        string replaced = args[1].Replace('\\', '_');

                        // create the new file and save to C TEMP
                        string fileName = $"C:\\TEMP\\excel_testset_list_{replaced}.xlsx";
                        workbook.SaveToFile(fileName, ExcelVersion.Version2016);
                        Console.WriteLine($"Finished running get and set test set list. Saved to: {fileName}");
                        break;
                    }
                case QUERYUNIQUE:
                    {
                        // this functionality records all unique values in the db
                        List<string> queryCols = args.Skip(1).ToList();

                        // create a workbook to write each query into a new file
                        Workbook workbookQuery = new Workbook();
                        workbookQuery.Worksheets.Clear();

                        DatabaseData dataNew = new DatabaseData();
                        dataNew.ConnectToDatabase(dataNew.TestDB);

                        foreach (string queryArg in queryCols)
                        {
                            Console.WriteLine($"We are going to get a list of all unique values in {queryArg}");

                            List<List<object>> oracleData = dataNew.QueryForLatestDistinct(queryArg);

                            if (oracleData == null)

                            {
                                Console.WriteLine("Incorrect parameter input");
                                break;
                            }

                            // indicate the name of the spreadsheet
                            Worksheet worksheetQuery = workbookQuery.Worksheets.Add(queryArg);

                            // set the first cell row as a header
                            int cellRowCounter = 1;
                            worksheetQuery.Range[cellRowCounter, 1].Text = queryArg;
                            cellRowCounter += 1;

                            // loop through each queried data and append to new row
                            foreach (List<object> line in oracleData)
                            {
                                worksheetQuery.Range[cellRowCounter, 1].Text = line[0].ToString();
                                cellRowCounter += 1;
                            }

                            // create the new file and save to C TEMP
                            string queriedFileName = $"C:\\TEMP\\excel_queried_list_report.xlsx";
                            workbookQuery.SaveToFile(queriedFileName, ExcelVersion.Version2016);
                            Console.WriteLine($"Finished running get and set test set list. Saved to: {queriedFileName}");
                        }
                        break;
                    }
                default:
                    Console.WriteLine("Incorrect parameter");
                    break;
            }

            // disconnect from server
            alm.DisconnectFromServer();

            Console.WriteLine("Finished executing");

            Environment.Exit(resultCode);
            return resultCode;
        }

        // this function generates database information and writes it into an excel
        private static void GenerateDatabaseInfo(DatabaseData data, TestSetInstance testSet, string almID)
        {
            // create a loop precondition. 
            bool keepSearching = true;

            // record the collection and release version
            string collection = testSet.GetField("Application Collection"); // gets field code
            string release_version = testSet.GetField("Test Case Version"); // gets field code

            // print out the collection and release we are iterating through
            Console.WriteLine("Collection: " + collection);
            Console.WriteLine("Release: " + release_version);

            // initialize while loop and keep searching until there are no test cases left
            while (keepSearching)
            {
                // we can get the current test case as follows
                //string currTestCaseName = testSet.GetCurrentTestCaseName();
                // Console.WriteLine("Test case name: " + currTestCaseName);

                // query database based on the current test case name
                List<List<object>> oracleData = data.QueryTestCase(testSet.GetCurrentTestCaseName(), collection, release_version);

                // for each line inside the oracle db data, we will write it to excel
                foreach (List<object> line in oracleData)
                {
                    data.WriteTestStepsToExcel(line);
                }

                // assign the variable keep searching to the next test case if it exists. Mutates curr test case
                keepSearching = testSet.MoveToNextTestCase();
            }

        }

        private static bool ParseCommandLine(string[] args)
        {
            Console.WriteLine("Parsing command line");

            return false;
        }


        private static void CreateDatabaseData(TestSetInstance testSet, String almID)
        {
            // create a new database data
            DatabaseData dataUnique = new DatabaseData();
            dataUnique.ConnectToDatabase(dataUnique.TestDB);
            dataUnique.initializeWorkSheetDB();
            dataUnique.writeTestSetDetails(testSet.Name, testSet.Description);
            GenerateDatabaseInfo(dataUnique, testSet, almID);
            Validation(dataUnique.workbook);
            dataUnique.resetTestCaseNum();
            String testSetName = (testSet.Name).Replace("/", "-");
            dataUnique.SaveToFile(testSetName);
        }

        // this function adds data validation on test set workbook with ACTIONONOBJECT
        private static void Validation(Workbook workbook)
        {
            // create a workbook to write each query into a new file
            Workbook workbookQuery = new Workbook();
            workbookQuery.Worksheets.Clear();

            DatabaseData dataNew = new DatabaseData();
            dataNew.ConnectToDatabase(dataNew.TestDB);

            string queryArg = "ACTIONONOBJECT";
            Console.WriteLine($"We are going to get a list of all unique values in {queryArg}");

            List<List<object>> oracleData = dataNew.QueryForLatestDistinct(queryArg);

            if (oracleData == null)
            {
                Console.WriteLine("Incorrect parameter input");
            }

            // indicate the name of the spreadsheet
            Worksheet worksheetQuery = workbookQuery.Worksheets.Add(queryArg);

            // set the first cell row as a header
            int cellRowCounter = 1;
            worksheetQuery.Range[cellRowCounter, 1].Text = queryArg;
            cellRowCounter += 1;

            // loop through each queried data and append to new row
            foreach (List<object> line in oracleData)
            {
                worksheetQuery.Range[cellRowCounter, 1].Text = line[0].ToString();
                cellRowCounter += 1;
            }

            // create the new file and save to C TEMP
            string queriedFileName = $"C:\\TEMP\\excel_queried_list_report.xlsx";
            workbookQuery.SaveToFile(queriedFileName, ExcelVersion.Version2016);
            Console.WriteLine($"Finished running get and set test set list. Saved to: {queriedFileName}");

            // adds ACTIONONOBJECT list to a new sheet in workbook
            Worksheet newWorksheet = workbook.Worksheets.Add("ACTIONONOBJECTS");
            newWorksheet.CopyFrom(worksheetQuery);

            newWorksheet.Visibility = WorksheetVisibility.Hidden;

            Worksheet worksheet = workbook.Worksheets[0];

            // finds the column with ACTIONONOBJECT
            int columnIndex = -1;
            for (int i = 1; i < worksheet.Columns.Length; i++)
            {
                if (worksheet.Range[1, i].Text == "ACTIONONOBJECT")
                {
                    columnIndex = i;
                    break;
                }
            }

            // adds data validation to each cell under ACTIONONOBJECT
            for (int j = 2; j <= worksheet.Rows.Length; j++)
            {
                CellRange rangeList = worksheet.Range[j, columnIndex];
                rangeList.DataValidation.DataRange = newWorksheet.Range[2, 1, newWorksheet.Rows.Length, 1];
                rangeList.DataValidation.IsSuppressDropDownArrow = false;
                rangeList.DataValidation.InputMessage = "Choose an item from the list";
            }
        }
    }
}

