// <copyright file="VerifyObjectAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step an abstract class to verify an object's avalibility.
    /// </summary>
    public class VerifyObjectAvailability : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Object Availability";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // Wait for loading spinner before verifying
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();

            // added by Victor
            // InformationObject.TestAutomationDriver.ClickElement(this.XPath, false, this.JsCommand);
            // InformationObject.TestAutomationDriver.WaitForLoadingSpinner();

            // find browser object
            string resultMsgEnd = this.Description.ToString().Replace("with", "using its");

            // parse provided flag, trim the value of the flag
            string flag = this.Arguments["value"].ToUpper().Trim();

            string resultMsg = $"Provided flag is {flag}. ";
            Logger.Info("Verifying Object " + resultMsg);

            TestingDriver.ITestingDriver.ElementState elementState;

            switch (flag)
            {
                case "0": case "ENABLED":
                    elementState = TestingDriver.ITestingDriver.ElementState.Clickable;
                    break;
                case "1": case "DISABLED":
                    elementState = TestingDriver.ITestingDriver.ElementState.Disabled;
                    break;
                case "2": case "EXIST":
                    elementState = TestingDriver.ITestingDriver.ElementState.Visible;
                    break;
                case "3": case "DOES NOT EXIST":
                    elementState = TestingDriver.ITestingDriver.ElementState.Invisible;
                    break;
                default:
                    throw new Exception($"Provided availability flag is '{flag}' which is not in [Enabled, Disabled, Exist, Does not exist].");
            }

            // added by Victor, needs to be tested
            try
            {
                bool passed = this.VerifyElementState(elementState);

                resultMsg += passed ? $"{resultMsgEnd} is {flag}." : $"{resultMsgEnd} was not {flag}.";

                this.TestStepStatus.Actual = resultMsg;
                this.TestStepStatus.RunSuccessful = passed;

                if (!passed)
                {
                    throw new Exception(resultMsg);
                }
            }
            catch (Exception e)
            {
                Logger.Info("Verify Object Availability Failed");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Verifying Object Availability";

                this.HandleException(e);

                int waitAmt = 10000; // 10 seconds between each verification attempt
                Thread.Sleep(waitAmt); // sleep for wait amt milliseconds
            }
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
