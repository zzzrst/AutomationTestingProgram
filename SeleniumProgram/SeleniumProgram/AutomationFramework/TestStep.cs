// <copyright file="TestStep.cs" company="PlaceholderCompany">
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
    /// An Implementation of the ITestStep class.
    /// </summary>
    public class TestStep : ITestStep
    {
        /// <inheritdoc/>
        public virtual string Name { get; set; } = "Test Step";

        /// <summary>
        /// Gets or sets arguments used for the test Step.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public int TestStepNumber { get; set; }

        /// <inheritdoc/>
        public ITestStepStatus TestStepStatus { get; set; }

        /// <inheritdoc/>
        public FlowBehavior OnExceptionFlowBehavior { get; set; } = FlowBehavior.Return;

        /// <summary>
        /// Gets or sets a value indicating whether to run AODA.
        /// </summary>
        public bool RunAODA { get; set; } = false;

        /// <summary>
        /// Gets or sets the AODA page name.
        /// </summary>
        public string RunAODAPageName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether test step should be logged.
        /// </summary>
        public bool ShouldLog { get; set; } = true;

        /// <inheritdoc/>
        public virtual void Execute()
        {
            this.ShouldExecuteVariable = false;
        }

        /// <inheritdoc/>
        public virtual void HandleException(Exception e)
        {
            this.TestStepStatus.ErrorStack = e.StackTrace;
            this.TestStepStatus.FriendlyErrorMessage = e.Message;
            this.TestStepStatus.RunSuccessful = false;

            if (this.ShouldLog)
            {
                InformationObject.CSVLogger.AddResults($"\"{this.Name}\",\"F\"");
            }

            InformationObject.TestingDriver.CheckErrorContainer();
            InformationObject.TestingDriver.TakeScreenShot();
        }

        /// <inheritdoc/>
        public virtual void SetUp()
        {
            if (this.TestStepStatus == null)
            {
                this.TestStepStatus = new TestStepStatus()
                {
                    StartTime = DateTime.UtcNow,
                    TestStepNumber = this.TestStepNumber,
                };
            }

            if (!this.ShouldExecuteVariable)
            {
                this.TestStepStatus.Actual = "N/A";
                InformationObject.CSVLogger.AddResults($"\"{this.Name}\",\"N/A\"");
            }
        }

        /// <inheritdoc/>
        public virtual bool ShouldExecute()
        {
            return this.ShouldExecuteVariable;
        }

        /// <inheritdoc/>
        public virtual void TearDown()
        {
            this.TestStepStatus.EndTime = DateTime.UtcNow;
            if (this.RunAODA)
            {
                InformationObject.TestingDriver.RunAODA(this.RunAODAPageName);
            }

            double totalTime = this.GetTotalElapsedTime();

            if (this.ShouldLog)
            {
                ITestStepLogger log = new TestStepLogger();
                log.Log(this);
                InformationObject.CSVLogger.AddResults($"\"{this.Name}\",\"{totalTime.ToString()}\"");
            }
        }

        private double GetTotalElapsedTime()
        {
            return Math.Abs((this.TestStepStatus.StartTime - this.TestStepStatus.EndTime).TotalSeconds);
        }
    }
}
