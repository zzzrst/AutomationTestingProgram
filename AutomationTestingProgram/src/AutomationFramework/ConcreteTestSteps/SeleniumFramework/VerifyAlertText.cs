// <copyright file="VerifyAlertText.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step is  to verify the text of an alert.
    /// </summary>
    public class VerifyAlertText : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Alert Text";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            bool passed = false;
            string message = string.Empty;
            string value = this.Arguments["value"];
            string alertText = InformationObject.TestAutomationDriver.GetAlertText();

            if (alertText == value)
            {
                passed = true;
                message = $"Text found in the alert was same as expected: {value}";
            }
            else
            {
                message = $"Expected text to be {value} but found {alertText}";
            }

            this.TestStepStatus.Actual = message;
            this.TestStepStatus.RunSuccessful = passed;
        }
    }
}
