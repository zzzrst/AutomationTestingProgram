// <copyright file="VerifyTextBoxAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of a textbox.
    /// </summary>
    public class VerifyTextBoxAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify Textbox Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebEdit_HTMLTags";
    }
}
