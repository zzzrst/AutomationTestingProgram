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
            /*WebElement widgetSpan = new WebElement(this.BrowserDriver.GetBrowser(), widgetSpanXPath, 10);
            if (widgetSpan.IsFound)
            {
                widgetSpan.Click();

                this.BrowserDriver.WaitForSpinner();

                // click link based on value
                string link = $"//div[contains(text(),'{this.Value}')]";
                WebElement lookupLink = new WebElement(this.BrowserDriver.GetBrowser(), link, 10);

                if (lookupLink.IsFound)
                {
                    lookupLink.Click();
                    this.TestStepStatus.RunSuccessful = true;
                    this.TestStepStatus.Actual = $"Successfully clicked on {value} found in {widgetSpanXPath}";
                }
                else
                {
                    this.TestStepStatus.Actual = $"Could not find the link that contains the text {value}";
                }
            }
            else
            {
                this.TestStepStatus.Actual = $"Could not find drop down list using {widgetSpanXPath}";
            }*/
        }
    }
}
