// <copyright file="ClickObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Threading;

    /// <summary>
    /// This test step allows us to save a value into a parameter and recall it in the future.
    /// </summary>
    public class GetWebElementText : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Get WebElement Text";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // this is the parameters value
            string parameter = this.Arguments["value"];

            try
            {
                // Logger.Info("launching xpath command");
                string text = InformationObject.TestAutomationDriver.GetElementText(this.XPath, this.JsCommand);
                this.TestStepStatus.Actual = "Got element text: " + text + "using xpath: " + this.XPath;

                InformationObject.RunParameters.Add(parameter, text);
                Logger.Info("Added parameter: " + parameter + " with value " + text);

                this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails
            }
            catch (Exception e)
            {
                Logger.Info($"Get Element Text using {this.XPath} and js {this.JsCommand} failed");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Getting Element Text";
                this.HandleException(e);
            }

            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
