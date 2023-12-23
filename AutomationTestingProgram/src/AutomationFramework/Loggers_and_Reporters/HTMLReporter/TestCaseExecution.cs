// <copyright file="TestCaseExecution.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class to store information related to a test case execution.
    /// </summary>
    public class TestCaseHTML
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseHTML"/> class.
        /// </summary>
        public TestCaseHTML()
        {
        }

        /// <summary>
        /// Gets or sets a list of test steps in the test case and their results.
        /// </summary>
        public List<List<string>> TestSteps { get; set; }

        /// <summary>
        /// Gets or sets the Attachments for this test case execution.
        /// </summary>
        public object[] Attachments { get; set; }

        /// <summary>
        /// Gets or sets the Execution Date and Time for this test case execution.
        /// </summary>
        public DateTime ExecDateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there are screenshots included in this test case execution.
        /// </summary>
        public bool HasScreenShot { get; set; }

        /// <summary>
        /// Gets or sets the number of test steps in the test case.
        /// </summary>
        public int NumSteps { get; set; }

        /// <summary>
        /// Gets or sets the unique test case run ID.
        /// </summary>
        public string RunID { get; set; }

        /// <summary>
        /// Gets or sets the unique test case run name for this test case execution.
        /// </summary>
        public string RunName { get; set; }

        /// <summary>
        /// Gets or sets the status of this test case execution.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the name of this test case. Default is the empty string.
        /// </summary>
        public string TestCaseName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the tester running this test case.
        /// </summary>
        public string TesterName { get; set; }
    }
}
