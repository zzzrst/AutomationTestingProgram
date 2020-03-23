// <copyright file="OpenBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class OpenBrowser : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "OpenBrowser";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string url = this.Arguments.ContainsKey("url") ? this.Arguments["url"] : string.Empty;
            InformationObject.TestAutomationDriver.NavigateToURL(url);
        }
    }
}
