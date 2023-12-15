﻿// <copyright file="TestStep.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
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
        public Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public int TestStepNumber { get; set; } = 0;

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

        /// <summary>
        /// Gets or sets number of attempts before a fail is thrown.
        /// </summary>
        public int MaxAttempts { get; set; } = 1;

        /// <summary>
        /// Gets or sets the description of the test step.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether determines whether the test step is optional or not.
        /// </summary>
        public bool Optional { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the test step should pass when it passes.
        /// </summary>
        public bool PassCondition { get; set; } = true;

        private int Attempts { get; set; } = 0;

        /// <inheritdoc/>
        public virtual void Execute()
        {
            this.Attempts++;
        }

        /// <inheritdoc/>
        public virtual void HandleException(Exception e)
        {
            this.TestStepStatus.Name += "Attempt 1/" + this.MaxAttempts;
            this.TestStepStatus.Description += "Error while trying to query the database.";
            this.TestStepStatus.Expected += "Execute " + this.Description + " successfully";
            this.TestStepStatus.Actual += "Failure in executing " + this.Description + "!\n" + e.ToString();
            this.TestStepStatus.ErrorStack += e.StackTrace;
            this.TestStepStatus.FriendlyErrorMessage += e.Message;
            this.TestStepStatus.RunSuccessful = false;

            Logger.Error(e.Message);

            InformationObject.TestAutomationDriver.CheckErrorContainer();
            InformationObject.TestAutomationDriver.TakeScreenShot();
        }

        /// <inheritdoc/>
        public virtual void SetUp()
        {
            if (this.TestStepStatus == null)
            {
                this.TestStepStatus = new TestStepStatus()
                {
                    Name = this.Name,
                    StartTime = DateTime.UtcNow,
                    TestStepNumber = this.TestStepNumber,
                    Description = this.Description,
                    Expected = this.Description,
                    Optional = this.Optional,
                };
            }

            InformationObject.TestStepData.SetArguments(this);

            if (!this.ShouldExecuteVariable)
            {
                this.TestStepStatus.Actual = "N/A";
            }
        }

        /// <inheritdoc/>
        public virtual bool ShouldExecute()
        {
            return this.ShouldExecuteVariable && this.Attempts < this.MaxAttempts;
        }

        /// <inheritdoc/>
        public virtual void TearDown()
        {
            this.TestStepStatus.EndTime = DateTime.UtcNow;
            if (this.RunAODA)
            {
                InformationObject.TestAutomationDriver.RunAODA(this.RunAODAPageName);
            }

            this.TestStepStatus.RunSuccessful = !this.ShouldExecuteVariable || (this.TestStepStatus.RunSuccessful == this.PassCondition);

            if (this.ShouldLog)
            {
                ITestStepLogger log = new TestStepLogger();
                log.Log(this);

                if (this.TestStepStatus.Actual == string.Empty)
                {
                    this.TestStepStatus.Actual = this.TestStepStatus.Expected;
                }
            }
            else
            {
                this.TestStepStatus.Actual = "No Log";
            }
        }
    }
}
