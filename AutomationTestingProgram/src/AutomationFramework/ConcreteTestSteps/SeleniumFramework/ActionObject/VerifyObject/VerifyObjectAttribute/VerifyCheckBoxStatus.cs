// <copyright file="VerifyCheckBoxStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the status of a check box.
    /// </summary>
    public class VerifyCheckBoxStatus : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Checkbox Status";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebCheckBox_HTMLTags";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string expectedValue = this.Arguments["value"].ToUpper();
            bool state = InformationObject.TestAutomationDriver.VerifyElementSelected(this.XPath, this.JsCommand);

            this.TestStepStatus.RunSuccessful = (expectedValue == "ON" && state) || (expectedValue == "OFF" && !state);
        }
    }
}
