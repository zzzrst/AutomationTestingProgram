// <copyright file="CheckBox.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step to click check box.
    /// </summary>
    public class CheckBox : ClickObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Check Box";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebCheckBox_HTMLTags";
    }
}