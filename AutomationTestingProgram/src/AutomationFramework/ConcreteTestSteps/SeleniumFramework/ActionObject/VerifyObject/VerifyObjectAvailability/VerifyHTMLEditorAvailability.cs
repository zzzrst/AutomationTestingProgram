// <copyright file="VerifyHTMLEditorAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Linq;
    using OpenQA.Selenium;
    using TestingDriver;

    /// <summary>
    /// This test step to verify the availabilty of an html editor.
    /// </summary>
    public class VerifyHTMLEditorAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify HTML Editor Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "";

        /// <inheritdoc/>
        public override void Execute()
        {
            InformationObject.TestAutomationDriver.SwitchToIFrame(this.Arguments["object"]);

            base.Execute();

            InformationObject.TestAutomationDriver.SwitchToIFrame("root");
        }

        /// <inheritdoc/>
        protected override bool VerifyElementState(ITestingDriver.ElementState elementState)
        {
            // either its //body or //html
            return InformationObject.TestAutomationDriver.CheckForElementState("//body", elementState);
        }
    }
}
