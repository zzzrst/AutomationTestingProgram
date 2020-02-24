// <copyright file="ClickElementByXPath.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class ClickElementByXPath : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ClickElementByXPath";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.TestStepInfo.Attributes["xPath"].Value;
            bool useJS = false;
            if (this.TestStepInfo.Attributes["useJS"] != null)
            {
                useJS = bool.Parse(this.TestStepInfo.Attributes["useJS"].Value);
            }

            InformationObject.TestingDriver.ClickElement(xPath, useJS);
            InformationObject.TestingDriver.WaitForLoadingSpinner();
        }
    }
}
