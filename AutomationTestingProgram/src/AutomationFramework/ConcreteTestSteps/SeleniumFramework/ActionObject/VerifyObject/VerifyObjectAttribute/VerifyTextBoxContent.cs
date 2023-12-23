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
        protected override string HTMLWhiteListTag { get; set; } = "WebEdit_HTMLTags";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expected = this.Arguments["value"];

            try
            {
                // TextArea stores its text in "value" attribute instead of innerText or textContent
                this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyFieldValue(expected, this.XPath, this.JsCommand);

                // this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails

                if (this.TestStepStatus.RunSuccessful)
                {
                    this.TestStepStatus.Actual = "Successfully verified web element content xpath: " + this.XPath;
                }
                else
                {
                    this.TestStepStatus.Actual = "Failure in Verifying Web TextBox content";

                    throw new Exception(this.TestStepStatus.Actual);
                }

                this.TestStepStatus.Actual = "Successfully verified web element content xpath: " + this.XPath;
            }
            catch (Exception ex)
            {
                Logger.Info($"Could not find '{this.Arguments["value"]}' at {this.XPath} using {this.JsCommand}.");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Verifying Web Content";
                Logger.Info("Run was unsuccessful in attempt to Verify Textbox content");
                this.HandleException(ex);
            }
        }
    }
}
