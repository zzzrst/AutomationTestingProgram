// <copyright file="DismissAlert.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to dissmiss alerts.
    /// </summary>
    public class DismissAlert : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Dismiss Alert";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            InformationObject.TestAutomationDriver.DismissAlert();
            this.TestStepStatus.Actual = "Alert was successfuly dismissed.";
        }
    }
}
