// <copyright file="PopulateTextBox.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class PopulateTextBox : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateTextBox";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string text = this.Arguments["value"];
            Logger.Warn("This test step only does xpaths and html ids for now.");

            try
            {
                InformationObject.TestAutomationDriver.PopulateElement(this.XPath, text);
                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
                this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails
                this.TestStepStatus.Actual = "Successfully populated text box"; // clear to default
            }
            catch (Exception ex)
            {
                Logger.Info("Trying to Populate Text Box");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Populating TextBox using xpath: " + this.XPath;
                this.HandleException(ex);
            }
        }
    }
}
