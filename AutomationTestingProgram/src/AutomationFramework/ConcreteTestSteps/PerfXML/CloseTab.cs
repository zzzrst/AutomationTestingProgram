// <copyright file="CloseTab.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of closing the current tab.
    /// </summary>
    public class CloseTab : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Close Tab";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string stringVal = this.Arguments["value"];

            // if stringVal is null, then log as error
            if (stringVal == string.Empty)
            {
                Logger.Error("Not a valid use of Close Tab, it is string.Empty");
                this.TestStepStatus.RunSuccessful = false;
                return;
            }

            int tabIndex = Convert.ToInt32(stringVal);

            try
            {
                // here we indicate the index of the tab that we are wanting to close
                InformationObject.TestAutomationDriver.SwitchToTab(tabIndex);
                InformationObject.TestAutomationDriver.CloseBrowser(false);
            }
            catch (Exception ex)
            {
                this.TestStepStatus.RunSuccessful = false;
                Logger.Error(ex);
            }
        }
    }
}
