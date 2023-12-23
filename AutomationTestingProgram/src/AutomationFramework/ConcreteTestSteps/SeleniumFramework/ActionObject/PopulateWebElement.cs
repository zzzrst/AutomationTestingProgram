// <copyright file="PopulateWebElement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of populating the element identified by the xpath.
    /// </summary>
    public class PopulateWebElement : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateWebElement";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            Logger.Warn("This test step only uses xpath for now.");

            // string xPath = this.Arguments["object"];
            string text = this.Arguments["value"];

            // added by Victor
            try
            {
                InformationObject.TestAutomationDriver.PopulateElement(this.XPath, text);
                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
                this.TestStepStatus.Actual = "Successfully Populated WebElement";
                this.TestStepStatus.RunSuccessful = true;
            }
            catch (Exception ex)
            {
                Logger.Info("Populate WebElement failed");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Populating Web Element";
                this.HandleException(ex);
            }
        }
    }
}
