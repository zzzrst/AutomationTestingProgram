// <copyright file="ExcelData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using AutomationTestSetFramework;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using System.IO;
    using TDAPIOLELib;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public class ExcelData : ITestData
    {
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

        /// <inheritdoc/>
        public void SetUp()
        {
            using (FileStream templateFS = new FileStream(this.TestArgs, FileMode.Open, FileAccess.Read))
            {
                this.ExcelFile = new XSSFWorkbook(templateFS);
            }

            this.TestSetSheet = this.ExcelFile.GetSheetAt(0);
        }
    }
}
