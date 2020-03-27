// <copyright file="DatabaseTestCaseNotFound.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System;

    /// <summary>
    /// Defines the <see cref="DatabaseTestCaseNotFound" />.
    /// </summary>
    public class DatabaseTestCaseNotFound : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "Could not find test case in database. ";

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTestCaseNotFound"/> class.
        /// </summary>
        public DatabaseTestCaseNotFound()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseTestCaseNotFound"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public DatabaseTestCaseNotFound(string message)
            : base(message)
        {
        }
    }
}
