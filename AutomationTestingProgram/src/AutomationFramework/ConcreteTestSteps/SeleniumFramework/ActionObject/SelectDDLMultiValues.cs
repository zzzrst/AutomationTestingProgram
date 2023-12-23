// <copyright file="SelectDDLMultiValues.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of selecting multiple values from the specified dropdownlist.
    /// </summary>
    public class SelectDDLMultiValues : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Select DDL Multi Values";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string values = this.Arguments["value"];

            try
            {
                foreach (string selection in values.Split(","))
                {
                    InformationObject.TestAutomationDriver.SelectValueInElement(this.XPath, selection);
                    InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Failure in selecting multiple DDL values.");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in selecting multiple DDL values:  " + values;
                this.HandleException(ex);
            }
        }
    }
}
