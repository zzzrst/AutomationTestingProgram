// <copyright file="VerifyWebElementContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to verify the content of an image.
    /// </summary>
    public class VerifyWebElementContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify WebElement Content";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebElement_HTMLTag";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expected = this.Arguments["value"];

            try
            {
                Logger.Info("Verify WebElement Content using xpath: " + this.XPath);
                Logger.Info("Verify WebElement Content using jsCommand: " + this.JsCommand);
                Logger.Info("Expected: " + expected);

                bool result = InformationObject.TestAutomationDriver.VerifyElementText(expected, this.XPath, this.JsCommand);

                // if the verification of the element text validation is false, then we will verify the field value instead. 
                // essentially doing field value validation in addition to verify element text. ie verify textbox content
                if (!result)
                {
                    Logger.Info("Failed verifying using VerifyElementText, now attempting VerifyElementValue");
                    result = InformationObject.TestAutomationDriver.VerifyFieldValue(expected, this.XPath, this.JsCommand);
                }

                this.TestStepStatus.RunSuccessful = result; // rest of try clause is skipped if it fails
                if (result)
                {
                    this.TestStepStatus.Actual = "Successfully verified web element content xpath: " + this.XPath;
                }
                else
                {
                    this.TestStepStatus.Actual = "Failure in Verifying Web Content";

                    throw new Exception(this.TestStepStatus.Actual);
                }
            }
            catch (Exception e)
            {
                Logger.Info("Verify Web Element failed due to exception");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Verifying Web Content, exception caught";
                Logger.Info("Exception caught in Verify WebElement Content: " + e.Message);
                this.HandleException(e);
            }
        }
    }
}
