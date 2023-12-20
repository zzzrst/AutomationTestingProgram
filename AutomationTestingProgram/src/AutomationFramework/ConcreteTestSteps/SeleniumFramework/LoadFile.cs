// <copyright file="LoadFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Linq;
    using AutomationTestinProgram.Helper;
    using AutomationTestSetFramework;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class LoadFile : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Load File";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string locator;
            string filePath = this.Arguments["value"];
            string property = this.Arguments["comment"];

            // add implementation to read html ids. See Load File examples
            if (property == "html id" || property == string.Empty)
            {
                // Ideally we get: //*[@id='Name']
                locator = $"//*[@id='{this.Arguments["object"]}']";
                Logger.Info("xpath from HTML ID: " + locator);
            }
            else
            {
                // original
                Logger.Warn("Likely error in implementation of Load File");
                locator = this.Arguments["object"];
            }

            filePath = FilePathResolver.Resolve(filePath); // resolve the file path in case it's as K folder

            Logger.Info("Attempting to upload file to: " + filePath + " with button:  " + locator);

            if (InformationObject.TestAutomationDriver.VerifyAttribute("type", "file", locator))
            {
                ((TestingDriver.SeleniumDriver)InformationObject.TestAutomationDriver).GetWebElement(locator).SendKeys(filePath);
                this.TestStepStatus.Actual = "Successfully inputted the value into Load File.";
            }
        }
    }
}
