// <copyright file="ClickElementByXPath.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class ClickElementByXPath : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ClickElementByXPath";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["xPath"];
            bool useJS = false;
            if (this.Arguments.ContainsKey("useJS"))
            {
                useJS = bool.Parse(this.Arguments["useJS"]);
            }

            InformationObject.TestAutomationDriver.ClickElement(xPath, useJS);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
