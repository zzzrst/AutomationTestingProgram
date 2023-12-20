// <copyright file="SelectRadioButton.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This class executes the action of selecting a value from the specified dropdownlist.
    /// </summary>
    public class SelectRadioButton : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SelectWebRadioGroup";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebRadioGroup_HTMLTags";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string selection = this.Arguments["value"];
            try
            {
                Logger.Info("Select Web Radio Group in process (added by Victor)" + this.XPath + " " + selection);

                InformationObject.TestAutomationDriver.SelectValueInElement(this.XPath, selection);

                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
            }
            catch (Exception ex)
            {
                Logger.Info($"Failur ni selecting web radio button.");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in selecting web radio button " + selection;
                this.HandleException(ex);
            }
        }
    }
}
