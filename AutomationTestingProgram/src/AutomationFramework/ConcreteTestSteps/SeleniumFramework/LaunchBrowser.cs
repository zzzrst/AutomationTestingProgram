// <copyright file="LaunchBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class LaunchBrowser : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Launch Browser";

        /// <inheritdoc/>
        public override void Execute()
        {
            Logger.Info($"Launching");
            base.Execute();
            string url = this.Arguments.ContainsKey("value") ? this.Arguments["value"] : string.Empty;
            InformationObject.TestAutomationDriver.NavigateToURL(url);
            Logger.Info($"Launch browser to url {url}");
        }
    }
}
