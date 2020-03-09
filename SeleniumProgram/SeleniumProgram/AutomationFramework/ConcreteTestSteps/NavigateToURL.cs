// <copyright file="NavigateToURL.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class NavigateToURL : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "NavigateToURL";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string url = this.Arguments["url"];
            InformationObject.TestAutomationDriver.NavigateToURL(url, false);
        }
    }
}
