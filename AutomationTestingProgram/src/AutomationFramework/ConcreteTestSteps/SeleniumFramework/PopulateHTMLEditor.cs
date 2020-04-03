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

            bool passed = false;
            string resultMsg = string.Empty;
            string attribute;
            string obj = this.Arguments["object"];
            string value = this.Arguments["value"];
            /*
            // switch to iframe
            bool cont = InformationObject.TestAutomationDriver.SwitchToIFrame(this.TestObject.Attribute, obj);

            if (!cont)
            {
                resultMsg = $"Could not find the iframe using {obj} {this.TestObject.Attribute}";
            }
            else
            {
                // use the first p, and change its innertext
                string paragraphXPath = "//body";
                WebElement paragraph = new WebElement(this.BrowserDriver.GetBrowser(), paragraphXPath, this.Timeout);
                paragraph.GetWebElement().Clear();
                paragraph.SendKeys(value);

                // this.BrowserDriver.GetBrowser().ExecuteJS($"arguments[0].innerText = '{this.Value}'", paragraph.GetWebElement());
                InformationObject.TestAutomationDriver.SwitchToIFrame("root");
                resultMsg = "Successfully populated HTML Editor.";
                passed = true;
            }*/
        }
    }
}
