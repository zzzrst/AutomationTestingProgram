// <copyright file="RuleInformation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    using System.Collections.Generic;

    /// <summary>
    /// Class to represent the information for each rule.
    /// </summary>
    public class RuleInformation
    {
        /// <summary>
        /// Gets or sets the description of the rule.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the help text for this rule.
        /// </summary>
        public string Help { get; set; }

        /// <summary>
        /// Gets or sets the help url associated with this rule.
        /// </summary>
        public string HelpUrl { get; set; }

        /// <summary>
        /// Gets or sets the impact associated with this rule.
        /// </summary>
        public string Impact { get; set; }

        /// <summary>
        /// Gets or sets the rule tag associated with this rule.
        /// </summary>
        public List<string> RuleTag { get; set; }
    }
}
