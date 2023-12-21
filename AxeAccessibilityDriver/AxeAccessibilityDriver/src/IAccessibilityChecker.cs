// <copyright file="IAccessibilityChecker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AxeAccessibilityDriver
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using OpenQA.Selenium;

    /// <summary>
    /// The Accessibility interface.
    /// </summary>
    public interface IAccessibilityChecker
    {
        /// <summary>
        /// Gets or sets webDriver to use.
        /// </summary>
        IWebDriver Driver { get; set; }

        /// <summary>
        /// This captures the AODA result for this webpage.
        /// </summary>
        /// <param name="providedPageTitle"> Title of the page. </param>
        void CaptureResult(string providedPageTitle);

        /// <summary>
        /// Logs the result for this file.
        /// </summary>
        /// <param name="folderLocation">Location to save all the results.</param>
        void LogResults(string folderLocation);
    }
}
