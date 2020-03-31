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
                this.TestCaseStatus = new TestCaseStatus()
                {
                    Name = this.Name,
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
            string actual = testStepStatus.Actual;
            if (testStepStatus.RunSuccessful == false)
            {
                this.TestCaseStatus.RunSuccessful = false;
                this.TestCaseStatus.FriendlyErrorMessage = "Something went wrong with a test step";
                actual = "F";
            }

            if (testStepStatus.Actual != "No Log")
            {
                InformationObject.CSVLogger.AddResults($"\"{testStepStatus.Name}\",\"{actual}\"");
            }

            InformationObject.Reporter.AddTestStepStatusToTestCase(testStepStatus, this.TestCaseStatus);
        }
    }
}
