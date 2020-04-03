// <copyright file="CheckBox.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to click check box.
    /// </summary>
    public class CheckBox : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Check Box";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            Logger.Warn("This method is not fully implented. it is currently acting like click element");
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
