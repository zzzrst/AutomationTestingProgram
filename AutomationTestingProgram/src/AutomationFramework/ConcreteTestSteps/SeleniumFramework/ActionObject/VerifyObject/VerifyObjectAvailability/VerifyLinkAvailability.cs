// <copyright file="VerifyLinkAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of a link.
    /// Note that verifying a link requries a FLAG. 
    /// If the FLAG is EXIST, this means that the text is visible.
    /// If the FLAG is DOES NOT EXIST, this means that the text is not visible.
    /// If the FLAG is ENABLED, this means that the text is CLICKABLE and VISIBLE
    /// If the FLAG is DISABLED, this means that the text is NOT CLICKABLE or NOT VISIBLE.
    /// </summary>
    public class VerifyLinkAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Link Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "Links_HTMLTags";
    }
}
