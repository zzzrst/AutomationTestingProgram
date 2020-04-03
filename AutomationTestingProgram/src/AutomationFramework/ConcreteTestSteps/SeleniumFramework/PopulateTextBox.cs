// <copyright file="PopulateTextBox.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class PopulateTextBox : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateTextBox";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["object"];
            string text = this.Arguments["value"];
            Logger.Warn("This test step only does xpaths for now.");
            InformationObject.TestAutomationDriver.PopulateElement(xPath, text);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
