// <copyright file="SelectLookup.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step is to select a lookup.
    /// </summary>
    public class SelectLookup : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Select Lookup";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string obj = this.Arguments["object"];
            string value = this.Arguments["value"];
            string attribute = this.Arguments["comment"];

            // click on the span to have lookup drop down
            string widgetSpanXPath = $"//div[@{attribute}='{obj}']";
            if (attribute.ToLower().Trim() == "xpath")
            {
                widgetSpanXPath = obj;
            }

            // widgetSpanXPath += $"//span[@arial-label='Select box activate']";
            InformationObject.TestAutomationDriver.ClickElement(widgetSpanXPath);

            // click link based on value
            string link = $"//div[contains(text(),'{this.Arguments["value"]}')]";
            InformationObject.TestAutomationDriver.ClickElement(link);

            this.TestStepStatus.Actual = $"Successfully clicked on {value} found in {widgetSpanXPath}";
        }
    }
}
