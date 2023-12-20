// <copyright file="VerifyImageContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to verify the content of an image.
    /// </summary>
    public class VerifyImageContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Image Content";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "Image_HTMLTags";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expected = this.Arguments["value"];

            this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyAttribute("outerHTML", expected, this.XPath, this.JsCommand);

            if (this.TestStepStatus.RunSuccessful)
            {
                this.TestStepStatus.Actual = "Successfully verified image content to be " + expected;
            }
            else
            {
                this.TestStepStatus.Actual = "Failure in Verifying Image Content";

                throw new Exception(this.TestStepStatus.Actual);
            }
        }
    }
}
