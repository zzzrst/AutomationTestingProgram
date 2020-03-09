// <copyright file="PopulateElementByXPath.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of populating the element identified by the xpath.
    /// </summary>
    public class PopulateElementByXPath : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "PopulateElementByXPath";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.TestStepInfo.Attributes["xPath"].Value;
            string text = this.TestStepInfo.Attributes["text"].Value;
            this.Driver.PopulateElement(xPath, text);
            this.Driver.WaitForLoadingSpinner();
        }
    }
}
