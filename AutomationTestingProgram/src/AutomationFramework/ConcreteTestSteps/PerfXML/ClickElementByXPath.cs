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
            string xPath = this.Arguments["object"];
            bool useJS = false;
            if (this.Arguments.ContainsKey("comment"))
            {
                bool.TryParse(this.Arguments["comment"], out useJS);
            }

            InformationObject.TestAutomationDriver.ClickElement(xPath, useJS);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
