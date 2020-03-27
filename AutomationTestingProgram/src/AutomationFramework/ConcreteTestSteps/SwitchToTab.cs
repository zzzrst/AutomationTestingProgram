// <copyright file="SwitchToTab.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of switching to tab x.
    /// </summary>
    public class SwitchToTab : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SwitchToTab";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            int tabIndex = Convert.ToInt32(this.Arguments["value"]);
            InformationObject.TestAutomationDriver.SwitchToTab(tabIndex);
        }
    }
}
