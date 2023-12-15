// <copyright file="FileNotFound.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Exceptions
{
    using System;

    /// <summary>
    /// Defines the <see cref="FileNotFound" />.
    /// </summary>
    public class FileNotFound : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "Could not find the file provided at the following path: ";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNotFound"/> class.
        /// </summary>
        public FileNotFound()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileNotFound"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public FileNotFound(string message)
            : base(message)
        {
        }
    }
}
