// <copyright file="SwitchToTab.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    using System;

    /// <summary>
    /// This class executes the action of switching to tab x.
    /// </summary>
    public class SwitchToTab : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SwitchToTab";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            int tabIndex = Convert.ToInt32(this.TestStepInfo.Attributes["tabIndex"].Value);
            this.Driver.SwitchToTab(tabIndex);
        }
    }
}
