// <copyright file="ClickLink.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to Click a link.
    /// </summary>
    public class ClickLink : ClickObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Click Link";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "Links_HTMLTags";
    }
}
