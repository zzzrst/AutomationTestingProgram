// <copyright file="VerifyObjectAvailability.cs" company="PlaceholderCompany">
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
    /// This test step an abstract class to verify an object's avalibility.
    /// </summary>
    public class VerifyObjectAvailability : VerifyObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Object Availability";

        /// <summary>
        /// Gets a value indicating whether IsEnabled
        /// Whether or not this object is enabled (clickable or writable).
        /// </summary>
        private bool IsEnabled
        {
            get
            {
                try
                {
                    bool isReadOnly = bool.Parse(this.Element.GetAttribute("readonly") ?? "false");
                    return this.Element != null && this.Element.Enabled && !isReadOnly;
                }
                catch (StaleElementReferenceException)
                {
                    // this exception is thrown when a reference to an element is no longer valid.
                    // Either the page was reloaded / element is no longer on the page.
                    return false;
                }
            }
        }

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            bool passed = false;
            string resultMsg = string.Empty;

            // find browser object
            string resultMsgEnd = this.Description.ToString().Replace("with", "using its");
            string value = this.Arguments["value"];

            // parse provided flag
            string flag = value.ToUpper();
            if (flag == "0" || flag == "ENABLED")
            {
                resultMsg = "Provided flag is 'Enabled'. ";
                if (!this.IsFound)
                {
                    resultMsg += "Could not find " + resultMsgEnd;
                }
                else
                {
                    passed = this.VerifyAvailability(flag);
                    resultMsg += passed ? $"Found {resultMsgEnd} which was enabled." : $"Found {resultMsg} which wasn't enabled.";
                }
            }
            else if (flag == "1" || flag == "DISABLED")
            {
                resultMsg = "Provided flag is 'Disabled'. ";
                if (!this.IsFound)
                {
                    resultMsg += "Could not find " + resultMsgEnd;
                }
                else
                {
                    passed = this.VerifyAvailability(flag);
                    resultMsg += passed ? $"Found {resultMsgEnd} which was disabled." : $"Found {resultMsgEnd} which wasn't disabled.";
                }
            }
            else if (flag == "2" || flag == "EXIST")
            {
                resultMsg = "Provided flag is 'Exist'. ";
                passed = this.VerifyAvailability(flag);
                resultMsg += passed ? $"Successfully found {resultMsgEnd}." : $"However, cannot find {resultMsgEnd}.";
            }
            else if (flag == "3" || flag == "DOES NOT EXIST")
            {
                resultMsg = "Provided flag is 'Does not exist'. ";
                passed = this.VerifyAvailability(flag);
                resultMsg += passed ? $"Cannot find {resultMsgEnd} as wanted." : $"However, was able to find {resultMsgEnd}.";
            }
            else
            {
                resultMsg = $"Provided availability flag is '{flag}' which is not in [Enabled, Disabled, Exist, Does not exist].";
            }

            this.TestStepStatus.Actual = resultMsg;
            this.TestStepStatus.RunSuccessful = passed;
        }

        /// <summary>
        /// Verifies the availability of this browser object given the flag.
        /// </summary>
        /// <param name="flag">Given availability flag, must be in ["Enabled", "Disabled", "Exist", "Does not exist"].</param>
        /// <returns><code>true</code> if given availability flag holds.</returns>
        private bool VerifyAvailability(string flag)
        {
            flag = flag.ToLower();
            if (flag == "0" || flag == "enabled")
            {
                return this.IsEnabled;
            }
            else if (flag == "1" || flag == "disabled")
            {
                return !this.IsEnabled;
            }
            else if (flag == "2" || flag == "exist")
            {
                return this.IsFound;
            }
            else if (flag == "3" || flag == "does not exist")
            {
                return !this.IsFound;
            }
            else
            {
                return false;
            }
        }
    }
}
