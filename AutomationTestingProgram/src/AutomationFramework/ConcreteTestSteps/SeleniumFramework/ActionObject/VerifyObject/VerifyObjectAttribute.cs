// <copyright file="VerifyObjectAttribute.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step an abstract class to verify an object's attribute.
    /// </summary>
    public abstract class VerifyObjectAttribute : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyObjectAttribute";

         /// <inheritdoc/>
        public override void Execute()
        {
           /* base.Execute();

            bool passed = false;
            string message = string.Empty;
            string value = this.Arguments["value"];

            // check if browser object is found
            if (!this.IsFound)
            {
                message = $"Cannot find {this.Name.Replace("Verify", string.Empty)}.";
            }
            else
            {
                string toCheck = this.content;
                if (this.Name.Contains("Check"))
                {
                    // attempt to verify status of check box
                    toCheck = this.status;
                }

                if (passed = this.VerifyAttribute(toCheck, value))
                {
                    message = $"Found and {this.Name}, {toCheck} is '{value}' as expected.";
                }
                else
                {
                    message = $"Found {this.Name.Replace("Verify", string.Empty)}, {toCheck} was expected to be '{value}' ";
                    message += $"but found '{this.GetValue(toCheck)}'.";
                }
            }

            this.TestStepStatus.Actual = message;
            this.TestStepStatus.RunSuccessful = passed;*/
        }
    }
}
