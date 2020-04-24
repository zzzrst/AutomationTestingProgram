// <copyright file="CannotAddTestCaseAttachment.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;

    /// <summary>
    /// Defines the <see cref="CannotAddTestCaseAttachment" />.
    /// </summary>
    public class CannotAddTestCaseAttachment : Exception
    {
        /// <summary>
        /// Defines the ErrorMsg.
        /// </summary>
        public const string ErrorMsg = "ALM cannot add test case attachment:";

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAddTestCaseAttachment"/> class.
        /// </summary>
        public CannotAddTestCaseAttachment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CannotAddTestCaseAttachment"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public CannotAddTestCaseAttachment(string message)
            : base(message)
        {
        }
    }
}
