// <copyright file="PopulateTextBoxDDL.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of populating the element identified by the xpath.
    /// </summary>
    public class PopulateTextBoxDDL : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateTextBoxDDL";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string text = this.Arguments["value"];

            try
            {
                InformationObject.TestAutomationDriver.PopulateElement(this.XPath, text, this.JsCommand);
                this.TestStepStatus.Actual = "Successfully Populated TextBoxDDL";
                this.TestStepStatus.RunSuccessful = true;
            }
            catch (Exception ex)
            {
                Logger.Info("Populate TextBoxDDL failed");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Populating Web Element";
                this.HandleException(ex);
            }

            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
