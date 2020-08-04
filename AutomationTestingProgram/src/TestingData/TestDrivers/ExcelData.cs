// <copyright file="ExcelData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.IO;
    using AutomationTestSetFramework;
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
        }

        /// <inheritdoc/>
        /// The path to the excel document
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Excel";

        /// <summary>
        /// Gets or sets the Excel File to read from.
        /// </summary>
        protected IWorkbook ExcelFile { get; set; }

        /// <summary>
        /// Gets or sets this sheet contains the test set info and data.
        /// </summary>
        protected ISheet TestSetSheet { get; set; }

        /// <summary>
        /// Gets or sets which user to check the Test Case against.
        /// </summary>
        protected string User { get; set; } = string.Empty;

        /// <inheritdoc/>
        public void SetUp()
        {
            try
            {
                string[] argument = this.TestArgs.Split(";");
                this.TestArgs = argument[0];
                this.User = argument[1];
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Info("One Argument Found, Will run all Test Cases in Excel (use ';" +
                    " to choose a test Case)");
            }

            using (FileStream templateFS = new FileStream(this.TestArgs, FileMode.Open, FileAccess.Read))
            {
                this.ExcelFile = new XSSFWorkbook(templateFS);
            }

            this.TestSetSheet = this.ExcelFile.GetSheetAt(0);
        }
    }
}
