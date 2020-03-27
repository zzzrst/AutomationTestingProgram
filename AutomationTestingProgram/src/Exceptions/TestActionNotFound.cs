// <copyright file="TestActionNotFound.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationFramework
{
    using System;

    /// <summary>
    /// Defines the <see cref="TestActionNotFound" />.
    /// </summary>
    public class TestActionNotFound : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "Could not find the test action.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionNotFound"/> class.
        /// </summary>
        public TestActionNotFound()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestActionNotFound"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public TestActionNotFound(string message)
            : base(message)
        {
        }
    }
}
