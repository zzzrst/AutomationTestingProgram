// <copyright file="TestAutomationBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Builders
{
    using System;
    using AutomationTestingProgram.TestAutomationDriver;
    using static AutomationTestingProgram.TestAutomationDriver.ITestAutomationDriver;

    /// <summary>
    /// Builds a new selenium Driver based on the given variables.
    /// </summary>
    public class TestAutomationBuilder
    {
        /// <summary>
        /// Gets or sets the browser to use.
        /// </summary>
        public Browser Browser { get; set; }

        /// <summary>
        /// Gets or sets the time out Threshold.
        /// </summary>
        public TimeSpan TimeOutThreshold { get; set; }

        /// <summary>
        /// Gets or sets the Enviornment to use.
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the url to go to.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets the location of the Screenshot.
        /// </summary>
        public string ScreenshotSaveLocation { get; set; }

        /// <summary>
        /// Gets or sets the error container location.
        /// </summary>
        public string ErrorContainer { get; set; }

        /// <summary>
        /// Gets or sets the loading spinner location.
        /// </summary>
        public string LoadingSpinner { get; set; }

        /// <summary>
        /// Returns the seleniumDriver based on the given paramenters.
        /// </summary>
        /// <returns>A new SeleniumDriver.</returns>
        public SeleniumDriver BuildSeleniumDriver()
        {
            SeleniumDriver driver;
            driver = new SeleniumDriver(this.Browser, this.TimeOutThreshold, this.Environment, this.URL, this.ScreenshotSaveLocation)
            {
                ErrorContainer = this.ErrorContainer,
                LoadingSpinner = this.LoadingSpinner,
            };
            return driver;
        }
    }
}
