// <copyright file="SwitchIntoIFrame.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of switching context into IFrame.
    /// </summary>
    public class SwitchIntoIFrame : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SwitchIntoIFrame";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["xPath"];
            InformationObject.TestAutomationDriver.SwitchToIFrame(xPath);
        }
    }
}
