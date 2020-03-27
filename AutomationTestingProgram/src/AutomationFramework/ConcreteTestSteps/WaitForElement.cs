// <copyright file="WaitForElement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using TestingDriver;

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
            string xPath = this.Arguments["object"];
            bool invisible = bool.Parse(this.Arguments["value"]);

            ITestingDriver.ElementState state = invisible ? ITestingDriver.ElementState.Invisible : ITestingDriver.ElementState.Visible;

            InformationObject.TestAutomationDriver.WaitForElementState(xPath, state);
        }
    }
}
