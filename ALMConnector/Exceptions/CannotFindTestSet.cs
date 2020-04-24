// <copyright file="CannotFindTestSet.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;

    /// <summary>
    /// Defines the <see cref="CannotFindTestSet" />.
    /// </summary>
    public class CannotFindTestSet : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "ALM could not find the test set: ";

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotFindTestSet"/> class.
        /// </summary>
        public CannotFindTestSet()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotFindTestSet"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public CannotFindTestSet(string message)
            : base(message)
        {
        }
    }
}
