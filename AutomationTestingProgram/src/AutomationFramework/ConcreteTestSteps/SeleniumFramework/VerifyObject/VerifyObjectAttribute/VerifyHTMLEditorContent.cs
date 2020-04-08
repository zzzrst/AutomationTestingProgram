// <copyright file="VerifyHTMLEditorContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Linq;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step to verify the content of a HTML editor.
    /// </summary>
    public class VerifyHTMLEditorContent : VerifyObjectAttribute
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify HTML Editor Content";

        /// <summary>
        /// Finds the web element by the given XPath.
        /// </summary>
        /// <returns>The matching web element on the current browser window.</returns>
        protected override IWebElement FindElement()
        {
            InformationObject.TestAutomationDriver.SwitchToIFrame(this.Arguments["object"]);

            string paragraphXPath = "//body";

            // really bad workaround
            var allElements = InformationObject.TestAutomationDriver.WebDriver.FindElements(By.XPath(paragraphXPath));
            try
            {
                return allElements.ToList().Find(e => e.Displayed);
            }
            catch (ArgumentNullException)
            {
                return allElements[0];
            }
        }
    }
}
