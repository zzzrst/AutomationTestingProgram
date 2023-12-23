// <copyright file="TestSet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Xml;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestSetFramework;
    //using TDAPIOLELib;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// Implementation of the ITestSet Class.
    /// </summary>
    public class TestSet : ITestSet
    {
        /// <summary>
        /// Gets or sets a value indicating we are going to block the test cases in the test set.
        /// </summary>
        public bool BlockTestSet { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public string Name { get; set; } = "Test Set Execution";

        /// <inheritdoc/>
        public int TotalTestCases
        {
            get; // => this.TestCaseFlow.ChildNodes.Count;
            set; // => this.TotalTestCases = this.TestCaseFlow.ChildNodes.Count;
        }

        /// <inheritdoc/>
        public ITestSetStatus TestSetStatus { get; set; }

        /// <inheritdoc/>
        public int CurrTestCaseNumber { get; set; } = 0;

        /// <inheritdoc/>
        public IMethodBoundaryAspect.FlowBehavior OnExceptionFlowBehavior { get; set; }

        // number of consecutive test case failures before failing a test set
        private int NumConsecutiveTestFailures { get; set; } = 0;

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            return InformationObject.TestSetData.ExistNextTestCase();
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            ITestCase testCase = null;

            testCase = InformationObject.TestSetData.GetNextTestCase();

            if (testCase == null)
            {
                Logger.Error("Error with GetNextTestCase");
                throw new Exception("Missing Test case");
            }

            testCase.TestCaseNumber = this.CurrTestCaseNumber;
            this.CurrTestCaseNumber += 1;

            return testCase;
        }

        /// <inheritdoc/>
        public void HandleException(Exception e)
        {
            this.TestSetStatus.ErrorStack += e.StackTrace;
            this.TestSetStatus.FriendlyErrorMessage += e.Message;
            this.TestSetStatus.RunSuccessful = false;
            this.ShouldExecuteVariable = false;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (this.TestSetStatus == null)
            {
                var timeUTC = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, easternZone);
                this.TestSetStatus = new TestSetStatus()
                {
                    Description = "Set Up Test Set",
                    Expected = "Expected to pass",
                    Name = this.Name,
                    StartTime = easternTime,
                };
            }
        }

        /// <inheritdoc/>
        public bool ShouldExecute()
        {
            return this.ShouldExecuteVariable && InformationObject.ShouldExecute;
        }

        /// <inheritdoc/>
        public void TearDown()
        {
            var timeUTC = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, easternZone);
            this.TestSetStatus.EndTime = easternTime;
            this.TestSetStatus.Name = this.Name;
            this.TestSetStatus.Description = "Test Set Execution";
            this.TestSetStatus.Expected = "Expected to pass";
            this.TestSetStatus.Actual = $"Passed is: {this.TestSetStatus.RunSuccessful}";

            if (GetEnvironmentVariable(EnvVar.TestSetDataType).ToLower() == "excel")
            {
                // record the test set status
                InformationObject.Reporter.AddTestSetStatus(this.TestSetStatus);
            }

            InformationObject.CSVLogger.AddResults($"Test Set Run Successful, {this.TestSetStatus.RunSuccessful}");

            ITestSetLogger log = new TestSetLogger();
            log.Log(this);
        }

        /// <inheritdoc/>
        public void UpdateTestSetStatus(ITestCaseStatus testCaseStatus)
        {
            if (testCaseStatus.RunSuccessful == false)
            {
                this.TestSetStatus.RunSuccessful = false;

                // increase the test case failures by one
                this.NumConsecutiveTestFailures += 1;
                Logger.Info("Failed consecutively counter: " + this.NumConsecutiveTestFailures);
            }
            else
            {
                this.NumConsecutiveTestFailures = 0;
            }

            // add test case result to both csv and devops or alm
            InformationObject.CSVLogger.AddResults($"Test Case Run Successful, {testCaseStatus.Name}, {testCaseStatus.RunSuccessful}");
            InformationObject.Reporter.AddTestCaseStatus(testCaseStatus);

            // int maxConsecutiveFailures = int.Parse(ConfigurationManager.AppSettings["MaxConsecutiveFailedTestCases"]);

            // Here we are setting the number of consecutive failures of the test case before failing the test case
            int maxConsecutiveFailures = int.Parse(InformationObject.GetEnvironmentVariable(EnvVar.MaxFailures));

            // if we reach max failures, print out reached MAX failures, and end.
            if (this.NumConsecutiveTestFailures >= maxConsecutiveFailures)
            {
                CSVLogger.AddResults($"Max Consecutive failures, {maxConsecutiveFailures} reached");
                Logger.Info("MAX failures reached, ending execution");

                // here we should report the rest of the test cases as blocked
                Logger.Info("Populating all remaining test cases as blocked");

                // we block the rest of the executions
                InformationObject.BlockTestSet = true;
            }
        }
    }
}