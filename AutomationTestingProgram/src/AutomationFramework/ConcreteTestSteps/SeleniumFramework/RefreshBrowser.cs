// <copyright file="RefreshBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to refresh browser.
    /// </summary>
    public class RefreshBrowser : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Refresh Browser";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            InformationObject.TestAutomationDriver.RefreshWebPage();
        }
    }
}
