// <copyright file="ClickObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Threading;

    /// <summary>
    /// This test step to click check box.
    /// </summary>
    public class ClickObject : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ClickObject";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            try
            {
                // parse provided flag
                string flag = this.Arguments["value"].ToUpper();

                // for only checkbox (move in the future)
                switch (flag)
                {
                    case "ON": // enabled
                        InformationObject.TestAutomationDriver.Check(this.XPath, true, this.JsCommand);
                        Logger.Info("Checked for whether button is ON");
                        this.TestStepStatus.Actual = "Successfully checked: " + this.XPath;
                        this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails
                        return;
                    case "OFF": // disabled
                        InformationObject.TestAutomationDriver.Uncheck(this.XPath, true, this.JsCommand);
                        Logger.Info("Checked for whether button is OFF");
                        this.TestStepStatus.Actual = "Successfully unchecked: " + this.XPath;
                        this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails
                        return;
                }

                // sometimes we will click elements using JS, so we should check whether or not we are clicking using JS
                if (this.JsCommand != string.Empty)
                {
                    // Logger.Info("launching JS command");
                    // this.XPath = "(//input | //button)"; // added by Victor (this should be in the whitelist)
                    InformationObject.TestAutomationDriver.ClickElement(this.XPath, true, this.JsCommand);
                    this.TestStepStatus.Actual = "Successfully clicked object js: " + this.JsCommand;
                }
                else
                {
                    // Logger.Info("launching xpath command");
                    InformationObject.TestAutomationDriver.ClickElement(this.XPath, false, this.JsCommand);
                    this.TestStepStatus.Actual = "Successfully clicked object xpath: " + this.XPath;
                }

                this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails
            }
            catch (Exception e)
            {
                Logger.Info($"Click Object failed in ClickObject.cs using xpath {this.XPath} and js {this.JsCommand}");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Clicking Object";
                this.HandleException(e);
            }

            Thread.Sleep(1 * 1000); // sleep for 1 seconds after clicking for it to register, can be changed for testing purposes
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
