// <copyright file="CloseBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of closing the browser.
    /// </summary>
    public class CloseBrowser : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Close Browser";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            InformationObject.TestAutomationDriver.CloseBrowser(true);
        }
    }
}
