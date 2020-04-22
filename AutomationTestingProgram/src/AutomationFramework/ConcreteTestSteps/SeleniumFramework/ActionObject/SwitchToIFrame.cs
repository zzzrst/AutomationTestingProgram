// <copyright file="SwitchToIFrame.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to switch into an iframe.
    /// </summary>
    public class SwitchToIFrame : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SwitchToIFrame";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["comment"];
            InformationObject.TestAutomationDriver.SwitchToIFrame(xPath);
        }
    }
}
