// <copyright file="TestCaseCreationFailed.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Exceptions
{
    using System;

    /// <summary>
    /// Defines the <see cref="TestCaseCreationFailed" />.
    /// </summary>
    public class TestCaseCreationFailed : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "Something went wrong when creating test case.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseCreationFailed"/> class.
        /// </summary>
        public TestCaseCreationFailed()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseCreationFailed"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public TestCaseCreationFailed(string message)
            : base(message)
        {
        }
    }
}
