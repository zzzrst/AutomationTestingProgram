// <copyright file="TestCase.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestSetFramework;
    using static AutomationTestingProgram.InformationObject;
    using static AutomationTestSetFramework.IMethodBoundaryAspect;

    /// <summary>
    /// Implementation of the testCase class.
    /// </summary>
    public class TestCase : ITestCase
    {
        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public string Name { get; set; } = "Test Case";

        /// <inheritdoc/>
        public int TestCaseNumber { get; set; }

        /// <inheritdoc/>
        public int TotalTestSteps
        {
            get; // => this.TestCaseInfo.ChildNodes.Count;
            set; // => this.TotalTestSteps = this.TestCaseInfo.ChildNodes.Count;
        }

        /// <inheritdoc/>
        public ITestCaseStatus TestCaseStatus { get; set; }

        /// <inheritdoc/>
        public int CurrTestStepNumber { get; set; } = 0;

        /// <inheritdoc/>
        public FlowBehavior OnExceptionFlowBehavior { get; set; }// = FlowBehavior.Return;

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            return InformationObject.TestCaseData.ExistNextTestStep();
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            TestStep testStep;
            testStep = (TestStep)InformationObject.TestCaseData.GetNextTestStep();

            if (testStep != null)
            {
                testStep.Name += $" {this.CurrTestStepNumber} ";
                testStep.TestStepNumber += this.CurrTestStepNumber;
                testStep.ShouldExecuteVariable = testStep.ShouldExecuteVariable && this.ShouldExecuteVariable;
                this.CurrTestStepNumber++;
            }

            return testStep;
        }

        /// <inheritdoc/>
        public void HandleException(Exception e)
        {
            this.TestCaseStatus.ErrorStack = e.StackTrace;
            this.TestCaseStatus.FriendlyErrorMessage = e.Message;
            this.TestCaseStatus.RunSuccessful = false;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (this.TestCaseStatus == null)
            {
                var timeUTC = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, easternZone);

                this.TestCaseStatus = new TestCaseStatus()
                {
                    Name = this.Name,
                    StartTime = easternTime,
                    TestCaseNumber = this.TestCaseNumber,
                };

                // later: fix for specifying db
                // if (GetEnvironmentVariable(EnvVar.TestSetDataType).ToLower() == "excel")
                InformationObject.Reporter.CreateAzureTestCase(this.Name, "This is a test description");
            }
        }

        /// <inheritdoc/>
        public bool ShouldExecute()
        {
            // if block test set
            if (InformationObject.BlockTestSet)
            {
                this.TestCaseStatus.RunSuccessful = false;
                this.TestCaseStatus.Actual = "Blocked";
                this.TestCaseStatus.Description = "Test Case Blocked";

                ITestStepStatus testStepStatus = new TestStepStatus();
                testStepStatus.RunSuccessful = false;

                InformationObject.Reporter.AddTestStepStatusToTestCase(testStepStatus, this.TestCaseStatus);

                return false;
            }

            return this.ShouldExecuteVariable && InformationObject.ShouldExecute;
        }

        /// <inheritdoc/>
        public void TearDown()
        {
            var timeUTC = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, easternZone);

            this.TestCaseStatus.EndTime = easternTime;
            if (!this.ShouldExecuteVariable)
            {
                this.TestCaseStatus.Actual = "Did not run.";
            }

            ITestCaseLogger log = new TestCaseLogger();
            log.Log(this);
        }

        /// <inheritdoc/>
        public void UpdateTestCaseStatus(ITestStepStatus testStepStatus)
        {
            string result = this.GetTotalElapsedTime(testStepStatus).ToString();
            if (testStepStatus.RunSuccessful == false)
            {
                result = "F";
                if (!((TestStepStatus)testStepStatus).Optional)
                {
                    this.TestCaseStatus.RunSuccessful = false;
                    this.TestCaseStatus.FriendlyErrorMessage = "Something went wrong with a MANDATORY test step";
                    Logger.Warn(this.TestCaseStatus.FriendlyErrorMessage);
                    this.ShouldExecuteVariable = false;
                }
                else if (((TestStepStatus)testStepStatus).ContinueOnError)
                {
                    // optional and important
                    this.TestCaseStatus.RunSuccessful = false;
                    this.TestCaseStatus.FriendlyErrorMessage = "Something went wrong with an IMPORTANT test step";
                    Logger.Warn(this.TestCaseStatus.FriendlyErrorMessage);
                }
                else
                {
                    // we won't affect the test case status if it's successful or not. Failed test case will remain failed, success will remain success
                    // this.TestCaseStatus.RunSuccessful = true; // assume that run was successful

                    this.TestCaseStatus.FriendlyErrorMessage = "Something went wrong with an OPTIONAL test step";
                    Logger.Warn(this.TestCaseStatus.FriendlyErrorMessage);
                }
            }

            if (testStepStatus.Actual != "No Log")
            {
                // here we can replace all the commas inside the test friendly error message
                string errorMessage = testStepStatus.FriendlyErrorMessage.Replace(",", string.Empty);
                string errorStack = testStepStatus.ErrorStack.Replace(",", string.Empty);

                InformationObject.CSVLogger.AddResults($"{testStepStatus.Name}, {testStepStatus.Description}, {testStepStatus.Expected}, {testStepStatus.RunSuccessful}, {testStepStatus.TestStepNumber},{result}, {errorMessage}, {errorStack}");
            }

            //if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ReportToDevOps"]))
            //{
            InformationObject.Reporter.AddTestStepStatusToTestCase(testStepStatus, this.TestCaseStatus);
            //}
        }

        private double GetTotalElapsedTime(ITestStepStatus testStepStatus)
        {
            return Math.Abs((testStepStatus.StartTime - testStepStatus.EndTime).TotalSeconds);
        }
    }
}
