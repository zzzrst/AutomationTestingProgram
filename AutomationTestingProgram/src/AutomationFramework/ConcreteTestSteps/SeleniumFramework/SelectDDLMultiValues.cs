// <copyright file="SelectDDLMultiValues.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of selecting a value from the specified dropdownlist.
    /// </summary>
    public class SelectDDLMultiValues : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SelectDDLMultiValues";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["object"];
            string values = this.Arguments["value"];
            foreach (string selection in values.Split(","))
            {
                InformationObject.TestAutomationDriver.SelectValueInElement(xPath, selection);
                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
            }
        }
    }
}
