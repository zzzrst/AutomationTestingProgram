// <copyright file="ClickObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to click check box.
    /// </summary>
    public class ClickObject : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ClickObject";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            InformationObject.TestAutomationDriver.ClickElement(this.XPath, false, this.JsCommand);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
