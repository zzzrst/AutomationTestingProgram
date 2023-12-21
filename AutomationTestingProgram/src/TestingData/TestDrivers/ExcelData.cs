// <copyright file="ExcelData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutomationTestSetFramework;
    using NPOI.OpenXmlFormats.Spreadsheet;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using TDAPIOLELib;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public class ExcelData : ITestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelData"/> class.
        /// </summary>
        /// <param name="args">The path to the excel file.</param>
        public ExcelData(string args)
        {
            this.TestArgs = args;
            try
            {
                string[] argument = args.Split(";");
                this.TestArgs = argument[0];

                // this.User = argument[1];
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Info("Not excel");

                // Logger.Info("One Argument Found, Will run all Test Cases in Excel (use ';" +
                //    " to add another argument");
            }
        }

        /// <inheritdoc/>
        /// The path to the excel document
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Excel";

        public static int RowIndex { get; set; } = 1;

        public static int TestCaseStartIndex { get; set; } = 1;

        /// <summary>
        /// Gets or sets the Excel File to read from.
        /// </summary>
        protected IWorkbook ExcelFile { get; set; }

        /// <summary>
        /// Gets or sets this sheet contains the test set info and data.
        /// </summary>
        protected ISheet TestSetSheet { get; set; }

        //protected int ColIndex { get; set; }

        /// <summary>
        /// Gets or sets which user to check the Test Case against.
        /// </summary>
        protected string User { get; set; } = string.Empty;

        /// <inheritdoc/>
        public void SetUp()
        {
            try
            {
                using (FileStream templateFS = new FileStream(this.TestArgs, FileMode.Open, FileAccess.Read))
                {
                    this.ExcelFile = new XSSFWorkbook(templateFS);
                }

                // move into the try block
                this.TestSetSheet = this.ExcelFile.GetSheetAt(0);
            }
            catch (Exception)
            {
                Logger.Error("Excel file is currently in use. Please close. OR does not exist");
                InformationObject.ShouldExecute = false; // set to false for should execute
            }
        }

        //public List<string> GetUniqueTestCases()
        //{
        //    HashSet<string> testCases = new HashSet<string>();

        //    int rowId = 1;
        //    ICell nextTestCase;
        //    // keep on adding next test cases until null reached
        //    while ((nextTestCase = this.TestSetSheet.GetRow(rowId).GetCell(ExcelCaseData.TESTCASENAME)) != null)
        //    {
        //        Console.WriteLine("Added test case: " + nextTestCase.ToString());
        //        testCases.Add(nextTestCase.ToString());
        //        rowId++;
        //    }

        //    return testCases.ToList();
        //}
    }
}
