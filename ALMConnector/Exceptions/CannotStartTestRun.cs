// <copyright file="CannotStartTestRun.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;

    /// <summary>
    /// Defines the <see cref="CannotStartTestRun" />.
    /// </summary>
    public class CannotStartTestRun : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "ALM cannot start test run:";

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotStartTestRun"/> class.
        /// </summary>
        public CannotStartTestRun()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotStartTestRun"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public CannotStartTestRun(string message)
            : base(message)
        {
        }
    }
}
