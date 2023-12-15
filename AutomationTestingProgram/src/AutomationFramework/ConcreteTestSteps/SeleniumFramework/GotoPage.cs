// <copyright file="GotoPage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to navigate to a url.
    /// </summary>
    public class GotoPage : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Goto Page";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string url = this.Arguments["value"];
            InformationObject.TestAutomationDriver.NavigateToURL(url, false);
        }
    }
}
