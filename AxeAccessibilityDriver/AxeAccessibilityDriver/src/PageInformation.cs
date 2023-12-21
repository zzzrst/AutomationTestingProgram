// <copyright file="PageInformation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    /// <summary>
    /// Class to represent information on a page.
    /// </summary>
    public class PageInformation
    {
        /// <summary>
        /// Gets or sets the title found on the page the browser is on..
        /// </summary>
        public string BrowserPageTitle { get; set; }

        /// <summary>
        /// Gets or sets the title provided by the user for the page.
        /// </summary>
        public string ProvidedPageTitle { get; set; }
    }
}
