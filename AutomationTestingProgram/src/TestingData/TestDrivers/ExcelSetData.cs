// <copyright file="ExcelSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public class ExcelSetData : ExcelData
    {
        private int ColIndex { get; set; } = 0;

        /// <summary>
        /// Gets The next test case.
        /// </summary>
        /// <returns>The next test case to run.</returns>
        public ITestCase GetNextTestCase()
        {
            return InformationObject.TestCaseData.SetUpTestCase(this.ColIndex.ToString());
        }

        /// <summary>
        /// Sees if there exist another test case.
        /// </summary>
        /// <returns>Returns true if there is another test case.</returns>
        public bool ExistNextTestCase()
        {
            return this.TestSetSheet.GetRow(0).GetCell(this.ColIndex).ToString() != null;
        }

        /// <summary>
        /// Sets up the test set and returns a new test step before it runs.
        /// </summary>
        public void SetUpTestSet()
        {
        }

        /// <summary>
        /// Adds an attachment to the result.
        /// </summary>
        /// <param name="attachment">the attachment to attach.</param>
        public void AddAttachment(string attachment)
        {
        }
    }
}
