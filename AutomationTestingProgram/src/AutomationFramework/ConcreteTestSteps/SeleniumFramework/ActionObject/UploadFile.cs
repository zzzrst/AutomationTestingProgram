﻿// <copyright file="UploadFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using AutomationTestinProgram.Helper;
    using AutomationTestSetFramework;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class UploadFile : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Upload File";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string uploadBtn;

            // add implementation to read html ids -- added by Victor
            if (this.Arguments["comment"] == "html id")
            {
                // Ideally we get: //*[@id='Name']
                uploadBtn = $"//*[@id='{this.Arguments["object"]}']";
                Logger.Info("xpath from HTML ID: " + uploadBtn);
            }
            else
            {
                // original
                uploadBtn = this.Arguments["object"];
            }

            Logger.Warn("This test step only does xpaths and html ids for now.");

            string filePath = this.Arguments["value"];
            filePath = FilePathResolver.Resolve(filePath); // resolve the file path in case it's as K folder

            Logger.Info("Attempting to upload file to: " + filePath + " with upload button:  " + uploadBtn);

            // check if this is input with type=file
            if (InformationObject.TestAutomationDriver.VerifyAttribute("type", "file", uploadBtn))
            {
                // we can just send the path in
                ((TestingDriver.SeleniumDriver)InformationObject.TestAutomationDriver).GetWebElement(uploadBtn).SendKeys(filePath);
                this.TestStepStatus.RunSuccessful = true;
                this.TestStepStatus.Actual = "Successfully inputted the value into upload file.";
                return;
            }
            else
            {
                IWebDriver driver = ((TestingDriver.SeleniumDriver)InformationObject.TestAutomationDriver).WebDriver;

                // check how many input with type=file
                var elements = driver.FindElements(By.XPath("//input[@type='file']"));
                if (elements.Count < 0)
                {
                    // none were found
                    this.TestStepStatus.RunSuccessful = false;
                    this.TestStepStatus.Actual = $"We could not find the input with type='file'";
                }
                else if (elements.Count == 1)
                {
                    // we just have to interact with this one
                    // we can just send the path in
                    elements[0].SendKeys(filePath);
                    this.TestStepStatus.RunSuccessful = true;
                    this.TestStepStatus.Actual = "Successfully inputted the value into upload file.";

                    // add test run attachment for upload file
                    InformationObject.TestSetData.AddAttachment(filePath);
                }
                else
                {
                    // we have to do something else.
                    // get the all the elements that have the same accept, but are not input, with type= file
                    string acceptPattern = InformationObject.TestAutomationDriver.GetElementAttribute("accept", this.XPath, this.JsCommand);

                    var uploadElements = driver.FindElements(By.XPath($"//*[@accept='{acceptPattern}' and not(@type='file')]"));

                    int index = uploadElements.IndexOf(((TestingDriver.SeleniumDriver)InformationObject.TestAutomationDriver).GetWebElement(this.XPath, this.JsCommand));
                    var inputFileElement = driver.FindElements(By.XPath($"//*[@accept='{acceptPattern}' and @type='file']"))[index];
                    inputFileElement.SendKeys(filePath);
                    this.TestStepStatus.RunSuccessful = true;
                    this.TestStepStatus.Actual = "Successfully inputted the value into upload file.";

                    // add test run attachment for upload file
                    InformationObject.TestSetData.AddAttachment(filePath);

                }
            }
        }
    }
}
