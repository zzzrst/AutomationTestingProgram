// <copyright file="TestStepLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations.Loggers_and_Reporters
{
    using AutomationTestSetFramework;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The Implemntation of the TestSetLogger.
    /// </summary>
    public class TestStepLogger : ITestStepLogger
    {
        /// <summary>
        /// Gets or sets the location to save the log to.
        /// </summary>
        public string SaveFileLocation { get; set; } = XMLInformation.LogSaveFileLocation + "\\Log.txt";

        /// <inheritdoc/>
        public void Log(ITestStep testStep)
        {
            ITestStepStatus testStepStatus = testStep.TestStepStatus;
            List<string> str = new List<string>();
            str.Add(this.Tab(2) + "Name:" + testStep.Name);
            str.Add(this.Tab(2) + "RunSuccessful:" + testStepStatus.RunSuccessful.ToString());
            str.Add(this.Tab(2) + "----------------------------");

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
