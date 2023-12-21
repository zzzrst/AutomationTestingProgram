// <copyright file="TestAutomationBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
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
            Logger.Info("Testing driver is: " + testingDriver);
            string acTimeout = ConfigurationManager.AppSettings["ActualTimeOut"];
            Logger.Info("Actual timeout is: " + acTimeout);

            string exType = ConfigurationManager.AppSettings["ExecutionType"];
            List<string> exTypesList = ConfigurationManager.AppSettings["ListTypeOfExecutions"].Split(",").ToList();

            if (exTypesList.Contains(exType))
            {
                switch (exType)
                {
                    case "mobile":
                        //windowSize.Width = windowSize.Width / 3;
                        break;
                    case "tablet":
                        //windowSize.Width = windowSize.Width / 2;
                        break;
                    case "desktop":
                        //windowSize.Width = 1024;
                        //windowSize.Height = 768;
                        break;
                    case "extended-desktop":
                        break;
                    case "max":
                        // maximize to the max size of the window, which should already be done
                        break;
                    default:
                        Logger.Warn("Not implemented error for size");
                        break;
                }
            }
            else
            {
                Logger.Info("Exeuction type list does not contain size " + exType);
            }

            bool headless = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["HEADLESS_MODE"]);
            bool incogMode = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["INCOGMODE"]);

            ITestingDriver automationDriver = new SeleniumDriver(
                GetEnvironmentVariable(EnvVar.Browser),
                int.Parse(GetEnvironmentVariable(EnvVar.TimeOutThreshold)),
                GetEnvironmentVariable(EnvVar.Environment),
                GetEnvironmentVariable(EnvVar.URL),
                GetEnvironmentVariable(EnvVar.ScreenshotSaveLocation),
                int.Parse(ConfigurationManager.AppSettings["ActualTimeOut"]),
                GetEnvironmentVariable(EnvVar.LoadingSpinner),
                GetEnvironmentVariable(EnvVar.ErrorContainer),
                string.Empty, headless, incogMode, null, exType);

            if (automationDriver == null)
            {
                Logger.Error($"Sorry we do not currently support the testing application: {testingDriver}");
            }

            TestAutomationDriver = automationDriver;
        }
    }
}
