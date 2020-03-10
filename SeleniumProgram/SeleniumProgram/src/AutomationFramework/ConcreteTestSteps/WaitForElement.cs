// <copyright file="WaitForElement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestingProgram.TestAutomationDriver;

    /// <summary>
    /// This class executes the action of waiting for an elment's status.
    /// </summary>
    public class WaitForElement : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "WaitForElement";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["xPath"];
            bool invisible = bool.Parse(this.Arguments["invisible"]);

            ITestAutomationDriver.ElementState state = invisible ? ITestAutomationDriver.ElementState.Invisible : ITestAutomationDriver.ElementState.Visible;

            InformationObject.TestAutomationDriver.WaitForElementState(xPath, state);
        }
    }
}
