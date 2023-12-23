// <copyright file="Login.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestinProgram.Helper;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step to log in.
    /// </summary>
    public class UploadDataFile : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Upload Data File";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            try
            {
                // we firstly assume that we've clicked into the collection and are now trying to upload the file. 
                // at the end, we click out of the data file upload
                // we must be provisioned Ministry Administrator role to do this.

                // click the menu tool kit drop down
                InformationObject.TestAutomationDriver.ClickElement("//a[@id='MenuAdminToolkit']");

                // click into the FileUpload button
                InformationObject.TestAutomationDriver.ClickElement("//a[@id='MenuDataFileUpload']");

                // click submit for processing
                string uploadBtn = "(//button[normalize-space()='Browse'])";

                string filePath = this.Arguments["value"];
                filePath = FilePathResolver.Resolve(filePath); // resolve the file path in case it's as K folder

                Logger.Info("Attempting to upload file to: " + filePath + " with upload button:  " + uploadBtn);

                // check if this is input with type=file
                if (InformationObject.TestAutomationDriver.VerifyAttribute("type", "file", uploadBtn))
                {
                    // we can just send the path in
                    ((TestingDriver.SeleniumDriver)InformationObject.TestAutomationDriver).GetWebElement(uploadBtn).SendKeys(filePath);

                    // then click the upload button
                    InformationObject.TestAutomationDriver.ClickElement("//button[@id='btnUpload']");

                    this.TestStepStatus.RunSuccessful = true;
                    this.TestStepStatus.Actual = "Successfully inputted the value into upload file.";
                    //return;
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
                        this.TestStepStatus.Actual = "Successfully inputted the value into upload file with element id 1";

                        InformationObject.TestAutomationDriver.ClickElement("//button[@id='btnUpload']");

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

                // print out the actual results
                Logger.Info("Actual results: " + this.TestStepStatus.Actual);

                // here, we wait for 10 seconds
                InformationObject.TestAutomationDriver.Wait(10);

                // click the btn Refresh buttons
                InformationObject.TestAutomationDriver.ClickElement("//button[@id='btnRefresh']");

                bool state;
                for (int i = 0; i < 3; i++)
                {
                    // verify that the page says Processsed is visible
                    state = InformationObject.TestAutomationDriver.CheckForElementState("//*[normalize-space()='Processed']", TestingDriver.ITestingDriver.ElementState.Visible);

                    // if the state is still false for verified as processed
                    if (state == false)
                    {
                        // refresh the page
                        InformationObject.TestAutomationDriver.ClickElement("//button[@id='btnRefresh']");

                        // wait 20 seconds
                        InformationObject.TestAutomationDriver.Wait(20);

                        Logger.Info("Unsuccessfully verified as processed");
                    }
                    else
                    {
                        Logger.Info("Successfully verified as processed");
                        break;
                    }
                }

                // Click back home
                InformationObject.TestAutomationDriver.ClickElement("//*[@id ='MenuHome']");
            }
            catch (Exception ex)
            {
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = ex.Message;
                Logger.Info(ex);
            }
            }
    }
}
