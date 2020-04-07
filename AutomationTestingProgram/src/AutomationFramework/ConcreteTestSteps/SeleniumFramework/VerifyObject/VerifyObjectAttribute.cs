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
    public class VerifyObjectAttribute : VerifyObject
    {
        /// <summary>
        /// Verification attribute string to check status.
        /// </summary>
        private readonly string status = "status";

        /// <summary>
        /// Verification attribute string to check content.
        /// </summary>
        private readonly string content = "content";

        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyObjectAttribute";

         /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

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
            this.TestStepStatus.RunSuccessful = passed;
        }

        /// <summary>
        /// Returns the status of the check box.
        /// </summary>
        /// <param name="attribute">Verification attribute string.</param>
        /// <returns>The status of the check box.</returns>
        private string GetValue(string attribute)
        {
            if (attribute == this.status)
            {
                return this.Element.Selected ? "ON" : "OFF";
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns whether or not the actual attribute value of the check box matches with the expected value,
        /// given a verification attribute string to check.
        /// </summary>
        /// <param name="attribute">Verification attribute string to check.</param>
        /// <param name="expectedValue">Expected value to compare with.</param>
        /// <returns><code>true</code> if actual attribute value matches with the expected value.</returns>
        private bool VerifyAttribute(string attribute, string expectedValue)
        {
            attribute = attribute.ToLower();
            if (attribute.Contains(this.status))
            {
                expectedValue = expectedValue.ToUpper();
                return (expectedValue == "ON" && this.Element.Selected) || (expectedValue == "OFF" && !this.Element.Selected);
            }

            return false;
        }
    }
}
