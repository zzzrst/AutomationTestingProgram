// <copyright file="TestCaseLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations.Loggers_and_Reporters
{
    using AutomationTestSetFramework;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The Implemntation of the TestCaseLogger.
    /// </summary>
    public class TestCaseLogger : ITestCaseLogger
    {
        /// <summary>
        /// Gets or sets the location to save the log to.
        /// </summary>
        public string SaveFileLocation { get; set; } = XMLInformation.LogSaveFileLocation + "\\Log.txt";

        /// <inheritdoc/>
        public void Log(ITestCase testCase)
        {
            ITestCaseStatus testCaseStatus = testCase.TestCaseStatus;
            List<string> str = new List<string>();
            str.Add(this.Tab(1) + "Name:" + testCase.Name);
            str.Add(this.Tab(1) + "RunSuccessful:" + testCaseStatus.RunSuccessful.ToString());

            foreach (string line in str)
            {
                Logger.Info(line);
            }
        }

        private string Tab(int indents = 1)
        {
            return string.Concat(Enumerable.Repeat("    ", indents));
        }
    }
}
