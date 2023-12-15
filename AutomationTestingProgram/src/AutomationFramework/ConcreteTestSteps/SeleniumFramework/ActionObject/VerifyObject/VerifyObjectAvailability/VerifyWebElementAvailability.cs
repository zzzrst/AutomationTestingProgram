﻿// <copyright file="VerifyWebElementAvailability.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to verify the availability of a textbox.
    /// </summary>
    public class VerifyWebElementAvailability : VerifyObjectAvailability
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Verify WebElement Availability";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebElement_HTMLTag";
    }
}
