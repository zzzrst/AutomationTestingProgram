// <copyright file="CannotAddTestStep.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;

    /// <summary>
    /// Defines the <see cref="CannotAddTestStep" />.
    /// </summary>
    public class CannotAddTestStep : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "ALM cannot add test step:";

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAddTestStep"/> class.
        /// </summary>
        public CannotAddTestStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAddTestStep"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public CannotAddTestStep(string message)
            : base(message)
        {
        }
    }
}
