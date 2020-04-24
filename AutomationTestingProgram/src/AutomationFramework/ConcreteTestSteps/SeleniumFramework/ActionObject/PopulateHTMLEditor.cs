// <copyright file="PopulateHTMLEditor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to populate the html editor.
    /// </summary>
    public class PopulateHTMLEditor : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateHTMLEditor";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string obj = this.Arguments["object"];
            string value = this.Arguments["value"];

            // switch to iframe
            InformationObject.TestAutomationDriver.SwitchToIFrame(obj);

            // use the first p, and change its innertext
            string paragraphXPath = "//body";
            InformationObject.TestAutomationDriver.PopulateElement(paragraphXPath, value);

            // this.BrowserDriver.GetBrowser().ExecuteJS($"arguments[0].innerText = '{this.Value}'", paragraph.GetWebElement());
            InformationObject.TestAutomationDriver.SwitchToIFrame("root");
            this.TestStepStatus.Actual = "Successfully populated HTML Editor.";
        }
    }
}
