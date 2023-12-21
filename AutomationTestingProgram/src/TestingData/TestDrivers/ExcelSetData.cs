// <copyright file="ExcelSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.AccessControl;
    using AutomationTestingProgram.Exceptions;
    using AutomationTestSetFramework;
    using log4net.Filter;
    using NPOI.HPSF;
    using NPOI.SS.UserModel;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public class ExcelSetData : ExcelData, ITestSetData
    {
        private const int TESTCASEINDEX = 0;

        private string testStepDescription = "";

        private bool reportToDevOps = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSetData"/> class.
        /// </summary>
        /// <param name="args">The arguments to be passed in.</param>
        public ExcelSetData(string args)
            : base(args)
        {
            // if we don't want to Report to DevOps
            this.reportToDevOps = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ReportToDevOps"]);
        }

        // changed from 3 to 4 since Victor added a type question. Should ask devs about this.
        // changed from 4 to 6 since Victor added two more questions question. Should ask devs about this.
        // TODO: further investigation is required. 
        // The ColIndex indicates the column for the TestCaseName, which is currently the second column

        // indicates the test case name we're currently on
        private string CurrTestCase { get; set; } = string.Empty;

        /// <summary>
        /// Gets The next test case.
        /// </summary>
        /// <returns>The next test case to run. If the current test case is the same, then we should be ignoring the need to run.</returns>
        public ITestCase GetNextTestCase()
        {
            ITestCase testCase = null;

            if (this.CurrTestCase != string.Empty)
            {
                while (this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(TESTCASEINDEX).ToString() == this.CurrTestCase)
                {
                    ExcelData.RowIndex++;
                }
            }

            string newUsr = this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(TESTCASEINDEX).ToString();
            testCase = InformationObject.TestCaseData.SetUpTestCase(newUsr);
            Logger.Info("Created Test Case: " + newUsr);
            this.CurrTestCase = newUsr;

            return testCase;
        }

        /// <summary>
        /// Sees if there exist another test case.
        /// </summary>
        /// <returns>Returns true if there is another test case.</returns>
        public bool ExistNextTestCase()
        {
            bool another_test_case;

            int tempCount = ExcelData.RowIndex;

            if (this.CurrTestCase != string.Empty)
            {
                while (this.TestSetSheet.GetRow(tempCount)?.GetCell(TESTCASEINDEX)?.ToString() == this.CurrTestCase)
                {
                    tempCount++;
                }
            }

            // set another test case to either null or to string value
            // Victor: if another test case is an empty string, we will also not continue.
            another_test_case = this.TestSetSheet.GetRow(tempCount)?.GetCell(TESTCASEINDEX) != null &&
                                this.TestSetSheet.GetRow(tempCount)?.GetCell(TESTCASEINDEX).ToString().Trim() != string.Empty; ;

            Logger.Info("Another test case is: " + another_test_case);
            Logger.Info("Row number for temp: " + tempCount);
            Logger.Info("Row number for ExcelData: " + ExcelData.RowIndex);
            return another_test_case;
        }

        /// <summary>
        /// Sets up the test set by calculating the number of test cases in it.
        /// Notice that a test set can have numerous test cases with the same name, so we have to account for that.
        /// </summary>
        public void SetUpTestSet()
        {
            if (!this.reportToDevOps || !InformationObject.ShouldExecute)
            {
                return;
            }

            // send test set out
            List<string> testCases = new List<string>();

            int rowId = 1;

            ICell nextTestCase;
            string lastTestCaseName = string.Empty;

            // for each step details
            //string testStepExpected;
            //string testStepDescription;
            string testStepExpected;
            string testStepValue;
            string testStepObject;
            string testStepType;
            string testStepAction;
            string testStepCondType;
            string testStepSkip;

            string currTestCaseId = string.Empty;

            // keep on adding next test cases until null reached
            while ((nextTestCase = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.TESTCASENAME)) != null)
            {
                string nextTestCaseVal = nextTestCase.ToString().Trim();

                // if the last test case is not the same as the next test case
                if (lastTestCaseName != nextTestCaseVal)
                {
                    // save test case details
                    if (currTestCaseId != string.Empty)
                    {
                        Console.WriteLine("Next test case " + nextTestCaseVal);
                        // save last test case test steps
                        InformationObject.Reporter.SaveTestSteps(currTestCaseId);
                    }
                    else
                    {
                        // we will also assign currTestCaseId
                    }

                    // create new test case
                    currTestCaseId = InformationObject.Reporter.CreateInitialTestCase(nextTestCase.ToString());

                    testCases.Add(nextTestCaseVal);
                    lastTestCaseName = nextTestCaseVal;
                }

                testStepType = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.TYPECOL)?.ToString() ?? string.Empty;
                testStepObject = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.OBJECTCOL)?.ToString() ?? string.Empty;
                testStepValue = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.VALUECOL)?.ToString() ?? string.Empty;
                testStepAction = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.ACTIONCOL)?.ToString() ?? string.Empty;

                // description is the expected test result, used for not only this but also for attachments
                //this.testStepDescription = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.DESC)?.ToString() ?? string.Empty;

                // expect test result is the description
                this.testStepDescription = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.DESC)?.ToString() ?? string.Empty;

                // create the value of the test step name to be published to DevOps
                testStepExpected = $"{testStepAction}";

                if (testStepType != string.Empty)
                {
                    testStepExpected += $" using {testStepType}";
                }

                // include value if it is not empty
                if (testStepValue != string.Empty)
                {
                    testStepExpected += $" and VALUE {testStepValue}";
                }

                // include object if it is not empty
                if (testStepObject != string.Empty)
                {
                    testStepExpected += $" and OBJECT {testStepObject}";
                }

                // include test step condition if it exists
                testStepCondType = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.TESTSTEPTYPE)?.ToString() ?? string.Empty;
                if (testStepCondType != string.Empty)
                {
                    testStepExpected += $" and TYPE {testStepCondType}";

                    if (testStepCondType.Length > 1)
                    {
                        Logger.Error("Error with parsing GoToStep, length > 1");
                    }

                    // if Type 4, then include the GoToStep value
                    if (int.Parse(testStepCondType) == 4) {
                        string goToStep = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.GOTOSTEP)?.ToString() ?? string.Empty;
                        testStepExpected += $" and GOTOSTEP {goToStep}";
                    }
                }

                // adding test step name as going to be skipped
                testStepSkip = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.CONTROL)?.ToString() ?? string.Empty;
                if (testStepSkip != string.Empty)
                {
                    testStepExpected += $" and CONTROL {testStepSkip}";
                }

                // create the azure test step
                InformationObject.Reporter.CreateAzureTestStep(this.testStepDescription, testStepExpected, this.testStepDescription);

                rowId++;

                // if the next row is null itself, then break
                if (this.TestSetSheet.GetRow(rowId)?.GetCell(ExcelCaseData.TESTCASENAME) == null  || 
                    this.TestSetSheet.GetRow(rowId)?.GetCell(ExcelCaseData.TESTCASENAME)?.ToString()?.Trim() == string.Empty)
                {
                    InformationObject.Reporter.SaveTestSteps(currTestCaseId);
                    break;
                }
            }

            InformationObject.Reporter.SetTestCaseList(testCases.ToList());
        }

        /// <summary>
        /// Adds an attachment to the result.
        /// </summary>
        /// <param name="attachment">the attachment to attach.</param>
        public void AddAttachment(string attachment)
        {
            string fileName = attachment.Substring(attachment.LastIndexOf("\\") + 1);
            //fileName = fileName.Substring(0, fileName.IndexOf("."));
            Console.WriteLine("FileName: " + fileName);

            //InformationObject.Reporter.AddTestRunAttachment("Test run attachment", attachment, fileName);
            InformationObject.Reporter.AddTestAttachment(fileName, attachment, $"{this.testStepDescription}");
        }

        /// <summary>
        /// Adds AODA Report log.
        /// </summary>
        /// <param name="attachment">the attachment to attach.</param>
        public void AddAODAReport(string attachment)
        {
            string aodaFileName = attachment.Substring(attachment.LastIndexOf("\\") + 1);
            //aodaFileName = aodaFileName.Substring(0, aodaFileName.IndexOf("."));
            InformationObject.Reporter.AddTestRunAttachment("AODA Report attachment", attachment, aodaFileName);
        }

        /// <summary>
        /// Adds error screenshot log.
        /// </summary>
        /// <param name="attachment">the attachment to attach.</param>
        public void AddErrorScreenshot(string attachment)
        {
        }

        private bool ParseSetParameters()
        {
            ISheet configSheet = this.ExcelFile.GetSheet("Test Set Config");

            if (configSheet == null)
            {
                Logger.Warn("Test Set Config sheet does not exist");
                return false;
            }

            // iterate through column A and column B to populate the InformationObject with the execution variables.

            return true;

        }
    }
}
