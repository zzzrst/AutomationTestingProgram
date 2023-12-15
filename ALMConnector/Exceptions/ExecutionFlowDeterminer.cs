// <copyright file="ExecutionFlowDeterminer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;

    /// <summary>
    /// Defines the <see cref="ExecutionFlowDeterminer" />.
    /// </summary>
    public class ExecutionFlowDeterminer : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "ALM Execution Flow cannot be determined. May be non-linear. ";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionFlowDeterminer"/> class.
        /// </summary>
        public ExecutionFlowDeterminer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionFlowDeterminer"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public ExecutionFlowDeterminer(string message)
            : base(message)
        {
        }
    }
}
