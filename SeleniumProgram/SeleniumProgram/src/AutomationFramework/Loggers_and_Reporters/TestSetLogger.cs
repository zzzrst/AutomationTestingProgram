// <copyright file="TestSetLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters
{
    using System.Collections.Generic;
    using AutomationTestSetFramework;

    /// <summary>
    /// The Implemntation of the TestSetLogger.
    /// </summary>
    public class TestSetLogger : ITestSetLogger
    {
        /// <summary>
        /// Gets or sets the location to save the log to.
        /// </summary>
        public string SaveFileLocation { get; set; } = InformationObject.LogSaveFileLocation + "\\Log.txt";

        /// <inheritdoc/>
        public void Log(ITestSet testSet)
        {
            ITestSetStatus testSetStatus = testSet.TestSetStatus;
            List<string> str = new List<string>();
            str.Add("Name:" + testSet.Name);
            str.Add("RunSuccessful:" + testSetStatus.RunSuccessful.ToString());

            foreach (string line in str)
            {
                Logger.Info(line);
            }
        }
    }
}
