// <copyright file="DDLSelectByXPath.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of selecting a value from the specified dropdownlist.
    /// </summary>
    public class DDLSelectByXPath : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "DDLSelectByXPath";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["xPath"];
            string selection = this.Arguments["selection"];
            InformationObject.TestAutomationDriver.SelectValueInElement(xPath, selection);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
