// <copyright file="VerifyObjectAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class VerifyObjectAttribute : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyObjectAttribute";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            bool passed = false;
            string message = string.Empty;
            string value = this.Arguments["value"];

            // find browser object
            Browser_Object browserObject = this.CreateBrowserObject(this.Timeout);

            // check if browser object is found
            if (!browserObject.IsFound)
            {
                message = $"Cannot find {browserObject.ToString()}.";
            }
            else if (browserObject is SeleniumAbstractionLayer.BrowserRelatedObjects.CheckBox checkBox)
            {
                // attempt to verify status of check box
                if (passed = checkBox.VerifyAttribute(Browser_Object.STATUS, value))
                {
                    message = $"Found and verified {checkBox.ToString()}, status is '{value}' as expected.";
                }
                else
                {
                    message = $"Found {checkBox.ToString()}, status was expected to be '{value}' ";
                    message += $"but found '{checkBox.GetValue(Browser_Object.STATUS)}'.";
                }
            }
            else
            {
                // attempt to verify content of browser object
                if (passed = browserObject.VerifyAttribute(Browser_Object.CONTENT, value))
                {
                    message = $"Found and verified {browserObject.ToString()}, content is '{value}' as expected.";
                }
                else
                {
                    message = $"Found {browserObject.ToString()}, content was expected to be '{value}' ";
                    message += $"but found '{browserObject.GetValue(Browser_Object.CONTENT)}'.";
                }
            }

            // create and return test status
            this.TestStatus = new Test_Status()
            {
                Message = message,
                Pass = passed,
            };
            PrintLog.PrintTestActionLog(this.Log());
            return this.TestStatus;
        }
    }
}
