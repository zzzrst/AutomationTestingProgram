// <copyright file="VerifyLinkAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of an image.
    /// </summary>
    public class VerifyLinkAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Link Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "Links_HTMLTags";
    }
}
