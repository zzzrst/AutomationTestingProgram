// <copyright file="ClickButton.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Configuration;
    using System.Threading;

    /// <summary>
    /// This test step to click a button.
    /// </summary>
    public class ClickButton : ClickObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Click Button";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebButton_HTMLTags";

        /// <inheritdoc/>
        public override void Execute()
        {
            int sleepAmt = int.Parse(ConfigurationManager.AppSettings["CLICK_BUTTON_WAIT_TIME"]);
            Logger.Info($"About to sleep {sleepAmt} seconds before executing");
            Thread.Sleep(sleepAmt);

            // base dot execute will execute the code for ClickObject
            base.Execute();
        }
    }
}
