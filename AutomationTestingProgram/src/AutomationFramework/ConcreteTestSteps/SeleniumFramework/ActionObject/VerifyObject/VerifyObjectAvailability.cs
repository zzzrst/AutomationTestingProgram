// <copyright file="VerifyObjectAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step an abstract class to verify an object's avalibility.
    /// </summary>
    public abstract class VerifyObjectAvailability : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Object Availability";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            bool passed = false;

            // find browser object
            string resultMsgEnd = this.Description.ToString().Replace("with", "using its");

            string value = this.Arguments["value"];

            // parse provided flag
            string flag = value.ToUpper();

            string resultMsg = $"Provided flag is {flag}. ";

            TestingDriver.ITestingDriver.ElementState elementState;

            if (flag == "0" || flag == "ENABLED")
            {
                elementState = TestingDriver.ITestingDriver.ElementState.Visible;
            }
            else if (flag == "1" || flag == "DISABLED")
            {
                elementState = TestingDriver.ITestingDriver.ElementState.Invisible;
            }
            else if (flag == "2" || flag == "EXIST")
            {
                elementState = TestingDriver.ITestingDriver.ElementState.Clickable;
            }
            else if (flag == "3" || flag == "DOES NOT EXIST")
            {
                elementState = TestingDriver.ITestingDriver.ElementState.Invisible;
            }
            else
            {
                throw new Exception($"Provided availability flag is '{flag}' which is not in [Enabled, Disabled, Exist, Does not exist].");
            }

            passed = this.VerifyElementState(elementState);
            resultMsg += passed ? $"{resultMsgEnd} is {flag}." : $"{resultMsgEnd} was not {flag}.";

            this.TestStepStatus.Actual = resultMsg;
            this.TestStepStatus.RunSuccessful = passed;
        }

        /// <summary>
        /// Verifys if the given element state holds true.
        /// </summary>
        /// <param name="elementState">The state of the element.</param>
        /// <returns>returns true if the state is correct.</returns>
        protected virtual bool VerifyElementState(TestingDriver.ITestingDriver.ElementState elementState)
        {
            return InformationObject.TestAutomationDriver.CheckForElementState(this.XPath, elementState, this.JsCommand);
        }
    }
}
