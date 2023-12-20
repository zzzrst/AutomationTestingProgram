// <copyright file="VerifyHTMLEditorContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Linq;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step to verify the content of a HTML editor.
    /// </summary>
    public class VerifyHTMLEditorContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify HTML Editor Content";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expected = this.Arguments["value"];

            InformationObject.TestAutomationDriver.SwitchToIFrame(this.Arguments["object"]);

            this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyElementText(expected, "//body", this.JsCommand);

            InformationObject.TestAutomationDriver.SwitchToIFrame("root");

            if (this.TestStepStatus.RunSuccessful)
            {
                this.TestStepStatus.Actual = "Successfully verified web HTML Editor Content xpath: " + this.XPath;
            }
            else
            {
                this.TestStepStatus.Actual = "Failure in Verifying HTML Editor Content";

                throw new Exception(this.TestStepStatus.Actual);
            }

        }
    }
}
