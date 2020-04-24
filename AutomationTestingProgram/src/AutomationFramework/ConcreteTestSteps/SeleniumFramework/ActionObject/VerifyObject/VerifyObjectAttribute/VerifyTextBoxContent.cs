// <copyright file="VerifyTextBoxContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to verify the content of an image.
    /// </summary>
    public class VerifyTextBoxContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Textbox Content";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expected = this.Arguments["value"];

            this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyElementText(expected, this.XPath, this.JsCommand);
        }

        /// <inheritdoc/>
        public override void HandleException(Exception e)
        {
            base.HandleException(e);
            Logger.Error($"Could not find '{this.Arguments["value"]}' at {this.XPath} using {this.JsCommand}.");
        }
    }
}
