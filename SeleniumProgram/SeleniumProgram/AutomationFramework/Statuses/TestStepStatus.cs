// <copyright file="TestStepStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestSetFramework;
    using System;

    /// <summary>
    /// Implementation of ITestStepStatus class.
    /// </summary>
    public class TestStepStatus : ITestStepStatus
    {
        /// <inheritdoc/>
        public int TestStepNumber { get; set; } = -1;

        /// <inheritdoc/>
        public bool RunSuccessful { get; set; } = true;

        /// <inheritdoc/>
        public string ErrorStack { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string FriendlyErrorMessage { get; set; } = string.Empty;

        /// <inheritdoc/>
        public DateTime StartTime { get; set; }

        /// <inheritdoc/>
        public DateTime EndTime { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Expected { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Actual { get; set; } = string.Empty;
    }
}
