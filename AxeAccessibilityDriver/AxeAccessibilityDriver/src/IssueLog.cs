// <copyright file="IssueLog.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// An object of the issue log's row.
    /// </summary>
    public class IssueLog
    {
        /// <summary>
        /// Gets or sets url of the issue.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the success criterion.
        /// </summary>
        public string Criterion { get; set; }

        /// <summary>
        /// Gets or sets description of the problem.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the impact of the problem.
        /// </summary>
        public string Impact { get; set; }
    }
}
