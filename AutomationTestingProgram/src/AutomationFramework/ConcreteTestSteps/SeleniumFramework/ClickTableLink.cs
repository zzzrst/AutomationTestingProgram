// <copyright file="ClickTableLink.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using OpenQA.Selenium;
using TestingDriver;

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// .
    /// </summary>
    public class ClickTableLink : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Click Table Link";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // find the link browser object
            IWebElement webtable = ((SeleniumDriver)InformationObject.TestAutomationDriver).GetWebElement(this.XPath, this.JsCommand);
            this.TestStepStatus.Actual = webtable.ToString() + " ";
            if (webtable == null)
            {
                this.TestStepStatus.Actual += "could not be found.";
            }
            else
            {
                string value = this.Arguments["value"];
                string targetRowInnerText = value.Split(';')[0];
                string targetLinkInnerText = value.Split(';')[1];
                this.ClickLink(targetRowInnerText, targetLinkInnerText);
            }
        }

        private void ClickLink(string targetRowInnerText, string targetLinkInnerText)
        {
            // using the baseXPath, we add //tr//td[contains(text(), "targetRowInnerText")]//..
            // Ex. //table[@id='indexTableManageSurveys']//tr//td[contains(text(), "asdf")]//..
            string getRowXPath = this.XPath + $"//tr//td[contains(text(), \"{targetRowInnerText}\")]//..";

            // using the getRowXPath, we add //td//*[contains(text(), "targetLinkInnerText")]
            // Ex. //table[@id='indexTableManageSurveys']//tr//td[contains(text(), "asdf")]//..//td//*[contains(text(), "sad")]
            string getLinkXPath = getRowXPath + $"//td/*[contains(text(), \"{targetLinkInnerText}\")]";

            InformationObject.TestAutomationDriver.ClickElement(getLinkXPath);
        }
    }
}
