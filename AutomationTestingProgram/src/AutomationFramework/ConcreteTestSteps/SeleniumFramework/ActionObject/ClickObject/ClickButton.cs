// <copyright file="ClickButton.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

//using Microsoft.Extensions.Configuration;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace AutomationTestingProgram.AutomationFramework
{
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
            base.Execute();

            // sleep an additional 3.5 seconds after clicking
            int sleepAmt = int.Parse(ConfigurationManager.AppSettings["CLICK_BUTTON_WAIT_TIME"]);
            Logger.Info($"About to sleep an additional {sleepAmt} seconds");
            Thread.Sleep(sleepAmt);
        }
    }
}
