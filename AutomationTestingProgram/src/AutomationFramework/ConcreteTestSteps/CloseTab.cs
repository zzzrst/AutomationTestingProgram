// <copyright file="CloseTab.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of closing the current tab.
    /// </summary>
    public class CloseTab : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "CloseTab";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            int tabIndex = Convert.ToInt32(this.Arguments["value"]);
            InformationObject.TestAutomationDriver.SwitchToTab(tabIndex);
            InformationObject.TestAutomationDriver.CloseBrowser();
        }
    }
}
