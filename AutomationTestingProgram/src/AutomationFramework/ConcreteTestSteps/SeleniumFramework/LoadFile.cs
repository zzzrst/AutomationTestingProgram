// <copyright file="LoadFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
/*
using System;

namespace AutomationTestingProgram.AutomationFramework
{
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
            bool passed = false;
            string obj = this.Arguments["object"];
            string value = this.Arguments["value"];
            string property = this.TestObject.Attribute;
            string message = $"Web File with attribute '{property}' and value of '{obj}' ";
            Test_Object objFileUpload = new Test_Object("webFile", property, obj);
            objFileUpload.AddAttribute("tag", "input");

            lock (AutoItDriver.Obj)
            {
                WebElement webFileUpload = new WebElement(this.BrowserDriver.GetBrowser(), objFileUpload, this.Timeout);

                // check if web file upload is found
                if (!webFileUpload.IsFound)
                {
                    message += "could not be found.";
                }
                else
                {
                    // attempt to upload to web file
                    try
                    {
                        while (AutoItDriver.DialogBoxExist(this.Alm.AlmBrowser))
                        {
                        }

                        if (passed = webFileUpload.Click(opensModal: true))
                        {
                            string uploadDialogName = AutoItDriver.UploadDialogBoxTitle(this.Alm.AlmBrowser);
                            if (passed = AutoItDriver.PopulateWinDialogBox(this.Alm.AlmBrowser, uploadDialogName, this.Timeout, value))
                            {
                                message += $"successfully chose file '{value}'.";
                            }
                            else
                            {
                                message += $"could not choose file '{value}'.";
                            }
                        }
                        else
                        {
                            message += $"could not select input to choose file '{value}'.";
                        }
                    }
                    catch (Exception ex)
                    {
                        message += $"did not choose file '{value}', but found something wrong: '{ex.Message}'.";
                    }
                }

                // create and return test status
                this.TestStatus = new Test_Status()
                {
                    Message = message,
                    Pass = passed,
                };
            }
    }
}
*/