// <copyright file="VerifyDDLAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of a drop down list.
    /// </summary>
    public class VerifyDDLAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify DDL Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebList_HTMLTag";
    }
}
