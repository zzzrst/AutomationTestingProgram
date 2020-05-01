// <copyright file="Reporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomationTestSetFramework;

    /// <summary>
    /// The implementation of the reporter class.
    /// </summary>
    public class Reporter : IReporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reporter"/> class.
        /// </summary>
        /// <param name="saveLocation">The location to save the file to.</param>
        public Reporter(string saveLocation)
        {
            this.SaveFileLocation = saveLocation;
            this.TestSetStatuses = new List<ITestSetStatus>();
            this.TestCaseStatuses = new List<ITestCaseStatus>();
            this.TestCaseToTestSteps = new Dictionary<ITestCaseStatus, List<ITestStepStatus>>();
        }

        /// <summary>
        /// Gets or sets the location to save the report to.
        /// </summary>
        public string SaveFileLocation { get; set; }

        /// <summary>
        /// Gets or sets the list of test set statuses.
        /// </summary>
        public List<ITestSetStatus> TestSetStatuses { get; set; }

        /// <summary>
        /// Gets or sets the list of test case statuses.
        /// </summary>
        public List<ITestCaseStatus> TestCaseStatuses { get; set; }

        /// <summary>
        /// Gets or sets the dictonary with key as Test Cases status and value as a list of Test step statuses.
        /// </summary>
        public Dictionary<ITestCaseStatus, List<ITestStepStatus>> TestCaseToTestSteps { get; set; }

        /// <inheritdoc/>
        public virtual void AddTestCaseStatus(ITestCaseStatus testCaseStatus)
        {
            this.TestCaseStatuses.Add(testCaseStatus);
        }

        /// <inheritdoc/>
        public virtual void AddTestSetStatus(ITestSetStatus testSetStatus)
        {
            this.TestSetStatuses.Add(testSetStatus);
        }

        /// <inheritdoc/>
        public virtual void AddTestStepStatusToTestCase(ITestStepStatus testStepStatus, ITestCaseStatus testCaseStatus)
        {
            if (!this.TestCaseToTestSteps.ContainsKey(testCaseStatus))
            {
                this.TestCaseToTestSteps.Add(testCaseStatus, new List<ITestStepStatus>());
            }

            this.TestCaseToTestSteps[testCaseStatus].Add(testStepStatus);
        }

        /// <inheritdoc/>
        public virtual void Report()
        {
            List<string> str = new List<string>();
            str.Add($"Running SeleniumPerfXML Version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            foreach (InformationObject.EnvVar var in Enum.GetValues(typeof(InformationObject.EnvVar)))
            {
                str.Add($"{var} = {InformationObject.GetEnvironmentVariable(var)}");
            }

            str.Add("-----------------------");
            foreach (ITestSetStatus testSetStatus in this.TestSetStatuses)
            {
                str.Add("Name: " + testSetStatus.Name);
                str.Add("RunSuccessful:" + testSetStatus.RunSuccessful.ToString());
                str.Add("StartTime:" + testSetStatus.StartTime.ToString());
                str.Add("EndTime:" + testSetStatus.EndTime.ToString());
                str.Add("Description:" + testSetStatus.Description);
                str.Add("ErrorStack:" + testSetStatus.ErrorStack);
                str.Add("FriendlyErrorMessage:" + testSetStatus.FriendlyErrorMessage);
                str.Add("Expected:" + testSetStatus.Expected);
                str.Add("Actual:" + testSetStatus.Actual);
            }

            foreach (ITestCaseStatus testCaseStatus in this.TestCaseStatuses)
            {
                str.Add(this.Tab(1) + "Name:" + testCaseStatus.Name);
                str.Add(this.Tab(1) + "TestCaseNumber:" + testCaseStatus.TestCaseNumber.ToString());
                str.Add(this.Tab(1) + "RunSuccessful:" + testCaseStatus.RunSuccessful.ToString());
                str.Add(this.Tab(1) + "StartTime:" + testCaseStatus.StartTime.ToString());
                str.Add(this.Tab(1) + "EndTime:" + testCaseStatus.EndTime.ToString());
                str.Add(this.Tab(1) + "Description:" + testCaseStatus.Description);
                str.Add(this.Tab(1) + "ErrorStack:" + testCaseStatus.ErrorStack);
                str.Add(this.Tab(1) + "FriendlyErrorMessage:" + testCaseStatus.FriendlyErrorMessage);
                str.Add(this.Tab(1) + "Expected:" + testCaseStatus.Expected);
                str.Add(this.Tab(1) + "Actual:" + testCaseStatus.Actual);

                if (this.TestCaseToTestSteps.ContainsKey(testCaseStatus))
                {
                    // log the test steps.
                    foreach (ITestStepStatus testStepStatus in this.TestCaseToTestSteps[testCaseStatus])
                    {
                        str.Add(this.Tab(2) + "Name:" + testStepStatus.Name);
                        str.Add(this.Tab(2) + "TestStepNumber:" + testStepStatus.TestStepNumber.ToString());
                        str.Add(this.Tab(2) + "RunSuccessful:" + testStepStatus.RunSuccessful.ToString());
                        str.Add(this.Tab(2) + "StartTime:" + testStepStatus.StartTime.ToString());
                        str.Add(this.Tab(2) + "EndTime:" + testStepStatus.EndTime.ToString());
                        str.Add(this.Tab(2) + "Description:" + testStepStatus.Description);
                        str.Add(this.Tab(2) + "ErrorStack:" + testStepStatus.ErrorStack);
                        str.Add(this.Tab(2) + "FriendlyErrorMessage:" + testStepStatus.FriendlyErrorMessage);
                        str.Add(this.Tab(2) + "Expected:" + testStepStatus.Expected);
                        str.Add(this.Tab(2) + "Actual:" + testStepStatus.Actual);
                        str.Add(this.Tab(2) + "-----------------------");
                    }
                }
            }

            str.Add(string.Empty);
            using (StreamWriter file =
            new StreamWriter($"{this.SaveFileLocation}", true))
            {
                foreach (string line in str)
                {
                    file.WriteLine(line);
                }
            }
        }

        private string Tab(int indents = 1)
        {
            return string.Concat(Enumerable.Repeat("    ", indents));
        }
    }
}
