// <copyright file="VerifyImageContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the content of an image.
    /// </summary>
    public class VerifyImageContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Image Content";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expected = this.Arguments["value"];

            this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyAttribute("outerHTML", expected, this.XPath, this.JsCommand);
        }
    }
}
