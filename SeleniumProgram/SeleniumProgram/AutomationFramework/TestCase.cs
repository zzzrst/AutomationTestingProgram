// <copyright file="TestCase.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using AutomationTestSetFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;

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
            get => this.TestCaseInfo.ChildNodes.Count;
            set => this.TotalTestSteps = this.TestCaseInfo.ChildNodes.Count;
        }

        /// <inheritdoc/>
        public ITestCaseStatus TestCaseStatus { get; set; }

        /// <inheritdoc/>
        public int CurrTestStepNumber { get; set; } = 0;

        /// <inheritdoc/>
        public IMethodBoundaryAspect.FlowBehavior OnExceptionFlowBehavior { get; set; }

        /// <summary>
        /// Gets or sets the ammount of times this should be ran.
        /// </summary>
        public int ShouldExecuteAmountOfTimes { get; set; } = 1;

        /// <summary>
        /// Gets or sets the ammount of times the test case has ran.
        /// </summary>
        public int ExecuteCount { get; set; } = 0;

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            return this.testStack.Count > 0 || this.ShouldExecuteAmountOfTimes > this.ExecuteCount + 1;
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            ITestStep testStep = null;

            // reached end of loop, check if should loop again.
            if (this.testStack.Count == 0 && this.ShouldExecuteAmountOfTimes > this.ExecuteCount)
            {
                this.AddNodesToStack(this.TestCaseInfo);
                this.ExecuteCount += 1;
            }

            testStep = this.IfRunTestStepLayer();

            return testStep;
        }

        /// <inheritdoc/>
        public void HandleException(Exception e)
        {
            this.ExecuteCount -= 1;
            this.TestCaseStatus.ErrorStack = e.StackTrace;
            this.TestCaseStatus.FriendlyErrorMessage = e.Message;
            this.TestCaseStatus.RunSuccessful = false;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (this.TestCaseStatus == null)
            {
                this.TestCaseStatus = new TestCaseStatus()
                {
                    StartTime = DateTime.UtcNow,
                    TestCaseNumber = this.TestCaseNumber,
                };
            }
        }

        /// <inheritdoc/>
        public bool ShouldExecute()
        {
            return this.ShouldExecuteVariable;
        }

        /// <inheritdoc/>
        public void TearDown()
        {
            this.TestCaseStatus.EndTime = DateTime.UtcNow;
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
            if (testStepStatus.RunSuccessful == false)
            {
                this.TestCaseStatus.RunSuccessful = false;
                this.TestCaseStatus.FriendlyErrorMessage = "Something went wrong with a test step";
            }

            InformationObject.Reporter.AddTestStepStatusToTestCase(testStepStatus, this.TestCaseStatus);
        }
    }
}
