// <copyright file="CloseTab.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    using System;

    /// <summary>
    /// This class executes the action of closing the current tab.
    /// </summary>
    public class CloseTab : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "CloseTab";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            int tabIndex = Convert.ToInt32(this.TestStepInfo.Attributes["tabIndex"].Value);
            this.Driver.SwitchToTab(tabIndex);
            this.Driver.CloseBrowser();
        }
    }
}
