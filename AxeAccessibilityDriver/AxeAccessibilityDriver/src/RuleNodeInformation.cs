// <copyright file="RuleNodeInformation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    using System.Collections.Generic;

    /// <summary>
    /// Class to represent the information on a rule node.
    /// </summary>
    public class RuleNodeInformation
    {
        /// <summary>
        /// Gets or sets the HTML element this rule pertains to.
        /// </summary>
        public string HTML { get; set; }

        /// <summary>
        /// Gets or sets the list of target HTML elements / information this rule has.
        /// </summary>
        public string Target { get; set; }
    }
}
