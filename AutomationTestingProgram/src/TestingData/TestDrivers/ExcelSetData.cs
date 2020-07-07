// <copyright file="ExcelSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public class ExcelSetData : ExcelData, ITestSetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSetData"/> class.
        /// </summary>
        /// <param name="args">The arguments to be passed in.</param>
        public ExcelSetData(string args)
            : base(args)
        {
        }

        private int ColIndex { get; set; } = 3;

        /// <summary>
        /// Gets The next test case.
        /// </summary>
        /// <returns>The next test case to run.</returns>
        public ITestCase GetNextTestCase()
        {
            this.ColIndex++;
            InformationObject.TestAutomationDriver.Quit();
            return InformationObject.TestCaseData.SetUpTestCase((this.ColIndex - 1).ToString());
        }

        /// <summary>
        /// Sees if there exist another test case.
        /// </summary>
        /// <returns>Returns true if there is another test case.</returns>
        public bool ExistNextTestCase()
        {
            return this.TestSetSheet.GetRow(0)?.GetCell(this.ColIndex) != null;
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
