// <copyright file="ClickButton.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to click a button.
    /// </summary>
    public class ClickButton : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Click Button";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.Arguments["object"];
            bool useJS = false;
            if (this.Arguments.ContainsKey("comment"))
            {
                useJS = bool.Parse(this.Arguments["comment"]);
            }

            InformationObject.TestAutomationDriver.ClickElement(xPath, useJS);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
