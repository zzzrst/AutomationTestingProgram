// <copyright file="VerifyWebTableAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of a textbox.
    /// </summary>
    public class VerifyWebTableAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify WebTable Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebTable_HTMLTag";
    }
}
