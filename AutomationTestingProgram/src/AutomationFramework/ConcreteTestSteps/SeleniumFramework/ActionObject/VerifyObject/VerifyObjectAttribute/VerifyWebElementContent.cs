// <copyright file="VerifyWebElementContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
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

            this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyElementText(expected, this.XPath, this.JsCommand);
        }
    }
}
