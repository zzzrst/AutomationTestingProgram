// <copyright file="PopulateElementByXPath.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of populating the element identified by the xpath.
    /// </summary>
    public class PopulateElementByXPath : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateElementByXPath";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["object"];
            string text = this.Arguments["value"];
            InformationObject.TestAutomationDriver.PopulateElement(xPath, text);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
