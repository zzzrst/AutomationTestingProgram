// <copyright file="TestSetXml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestSetFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using System;
    using System.Collections.Generic;
    using System.Xml;

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
            get => this.TestCaseFlow.ChildNodes.Count;
            set => this.TotalTestCases = this.TestCaseFlow.ChildNodes.Count;
        }

        /// <inheritdoc/>
        public ITestSetStatus TestSetStatus { get; set; }

        /// <inheritdoc/>
        public int CurrTestCaseNumber { get; set; } = -1;

        /// <inheritdoc/>
        public IMethodBoundaryAspect.FlowBehavior OnExceptionFlowBehavior { get; set; }

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            return this.testStack.Count > 0;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            TestCase testCase = null;

            testCase = this.IfRunTestCaseLayer();

            if (testCase == null)
            {
                throw new Exception("Missing Test case");
            }

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

            this.Reporter.AddTestSetStatus(this.TestSetStatus);

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

            this.Reporter.AddTestCaseStatus(testCaseStatus);
        }
    }
}
