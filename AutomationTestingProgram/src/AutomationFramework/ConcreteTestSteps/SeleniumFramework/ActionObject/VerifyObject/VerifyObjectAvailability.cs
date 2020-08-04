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
    public class VerifyObjectAvailability : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyObjectAvailability";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // find browser object
            string resultMsgEnd = this.Description.ToString().Replace("with", "using its");

            // parse provided flag
            string flag = this.Arguments["value"].ToUpper();

            string resultMsg = $"Provided flag is {flag}. ";

            TestingDriver.ITestingDriver.ElementState elementState;

            switch (flag)
            {
                case "0": case "ENABLED":
                    elementState = TestingDriver.ITestingDriver.ElementState.Visible;
                    break;
                case "1": case "DISABLED":
                    elementState = TestingDriver.ITestingDriver.ElementState.Disabled;
                    break;
                case "2": case "EXIST":
                    elementState = TestingDriver.ITestingDriver.ElementState.Clickable;
                    break;
                case "3": case "DOES NOT EXIST":
                    elementState = TestingDriver.ITestingDriver.ElementState.Invisible;
                    break;
                default:
                    throw new Exception($"Provided availability flag is '{flag}' which is not in [Enabled, Disabled, Exist, Does not exist].");
            }

            bool passed = this.VerifyElementState(elementState);
            resultMsg += passed ? $"{resultMsgEnd} is {flag}." : $"{resultMsgEnd} was not {flag}.";

            this.TestStepStatus.Expected = this.Arguments["value"];
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
