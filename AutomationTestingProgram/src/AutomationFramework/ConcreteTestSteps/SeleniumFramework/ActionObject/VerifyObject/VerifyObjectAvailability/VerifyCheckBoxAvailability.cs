// <copyright file="VerifyCheckBoxAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of a check box.
    /// </summary>
    public class VerifyCheckBoxAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Checkbox Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebCheckBox_HTMLTags";
    }
}
