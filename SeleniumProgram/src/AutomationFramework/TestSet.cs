// <copyright file="TestSet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestSetFramework;

    /// <summary>
    /// Implementation of the ITestSet Class.
    /// </summary>
    public class TestSet : ITestSet
    {
        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public string Name { get; set; } = "Test Set";

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
                throw new Exception("Missing Test case");
            }

            testCase.TestCaseNumber = this.CurrTestCaseNumber;
            this.CurrTestCaseNumber += 1;

            return testCase;
        }

        /// <inheritdoc/>
        public void HandleException(Exception e)
        {
            this.TestSetStatus.ErrorStack = e.StackTrace;
            this.TestSetStatus.FriendlyErrorMessage = e.Message;
            this.TestSetStatus.RunSuccessful = false;
            this.ShouldExecuteVariable = false;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (this.TestSetStatus == null)
            {
                this.TestSetStatus = new TestSetStatus()
                {
                    Name = this.Name,
                    StartTime = DateTime.UtcNow,
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
            this.TestSetStatus.EndTime = DateTime.UtcNow;

            InformationObject.Reporter.AddTestSetStatus(this.TestSetStatus);

            ITestSetLogger log = new TestSetLogger();
            log.Log(this);
        }

        /// <inheritdoc/>
        public void UpdateTestSetStatus(ITestCaseStatus testCaseStatus)
        {
            if (testCaseStatus.RunSuccessful == false)
            {
                this.TestSetStatus.RunSuccessful = false;
            }

            InformationObject.Reporter.AddTestCaseStatus(testCaseStatus);
        }
    }
}
