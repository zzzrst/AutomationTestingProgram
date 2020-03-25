// <copyright file="TestAutomationBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Builders
{
    using System;
    using System.Configuration;
    using TestingDriver;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// Builds a new selenium Driver based on the given variables.
    /// </summary>
    public class TestAutomationBuilder
    {
        /// <summary>
        /// The usable testing applications.
        /// </summary>
        public enum TestingDriverType
        {
            /// <summary>
            /// Selenium program.
            /// </summary>
            Selenium,
        }

        /// <summary>
        /// Builds a new test automation driver.
        /// </summary>
        public void Build()
        {
            string testingDriver = GetEnvironmentVariable(EnvVar.TestAutomationDriver);
            SeleniumDriver driver = new SeleniumDriver();
            ITestingDriver automationDriver = new SeleniumDriver(
                GetEnvironmentVariable(EnvVar.Browser),
                int.Parse(GetEnvironmentVariable(EnvVar.TimeOutThreshold)),
                GetEnvironmentVariable(EnvVar.Environment),
                GetEnvironmentVariable(EnvVar.URL),
                GetEnvironmentVariable(EnvVar.ScreenshotSaveLocation),
                int.Parse(ConfigurationManager.AppSettings["ActualTimeOut"]),
                GetEnvironmentVariable(EnvVar.LoadingSpinner),
                GetEnvironmentVariable(EnvVar.ErrorContainer),
                string.Empty);
            if (automationDriver == null)
            {
                Logger.Error($"Sorry we do not currently support the testing application: {testingDriver}");
            }

            TestAutomationDriver = automationDriver;
        }
    }
}
