// <copyright file="PopulateTextBoxDDL.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
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
            InformationObject.TestAutomationDriver.PopulateElement(this.XPath, text, this.JsCommand);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
