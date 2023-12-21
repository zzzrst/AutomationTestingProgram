// <copyright file="CannotPostTestRun.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;

    /// <summary>
    /// Defines the <see cref="CannotPostTestRun" />.
    /// </summary>
    public class CannotPostTestRun : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "ALM could not post test run: ";

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotPostTestRun"/> class.
        /// </summary>
        public CannotPostTestRun()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotPostTestRun"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public CannotPostTestRun(string message)
            : base(message)
        {
        }
    }
}
