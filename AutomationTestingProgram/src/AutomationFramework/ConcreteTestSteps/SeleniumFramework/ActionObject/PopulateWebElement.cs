// <copyright file="PopulateWebElement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of populating the element identified by the xpath.
    /// </summary>
    public class PopulateWebElement : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateWebElement";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            Logger.Warn("This test step only uses xpath for now.");
            string xPath = this.Arguments["object"];
            string text = this.Arguments["value"];
            InformationObject.TestAutomationDriver.PopulateElement(xPath, text);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
