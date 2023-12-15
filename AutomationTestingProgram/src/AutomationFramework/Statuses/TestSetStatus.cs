﻿// <copyright file="TestSetStatus.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using AutomationTestSetFramework;

    /// <summary>
    /// An Implementation of the testSetStatus class.
    /// </summary>
    public class TestSetStatus : ITestSetStatus
    {
        /// <inheritdoc/>
        public string Name { get; set; } = "test set";

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
