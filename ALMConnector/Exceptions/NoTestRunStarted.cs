// <copyright file="NoTestRunStarted.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;

    /// <summary>
    /// Defines the <see cref="NoTestRunStarted" />.
    /// </summary>
    public class NoTestRunStarted : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "User did not start test run first.";

        /// <summary>
        /// Initializes a new instance of the <see cref="NoTestRunStarted"/> class.
        /// </summary>
        public NoTestRunStarted()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoTestRunStarted"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public NoTestRunStarted(string message)
            : base(message)
        {
        }
    }
}
