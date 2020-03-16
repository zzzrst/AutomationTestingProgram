// <copyright file="InformationObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.TestAutomationDriver;
    using AutomationTestingProgram.TestingData;

    /// <summary>
    /// An information class that contains information needed by other objects/methods.
    /// </summary>
    public static class InformationObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether to respect the repeat for value.
        /// </summary>
        public static bool RespectRepeatFor { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to run AODA.
        /// </summary>
        public static bool RespectRunAODAFlag { get; set; } = false;

        /// <summary>
        /// Gets or sets the location to save the log.
        /// </summary>
        public static string LogSaveFileLocation { get; set; }

        /// <summary>
        /// Gets or sets the location to save the screenshot.
        /// </summary>
        public static string ScreenshotSaveLocation { get; set; }

        /// <summary>
        /// Gets or sets the location to save the CSV file.
        /// </summary>
        public static CSVLogger CSVLogger { get; set; }

        /// <summary>
        /// Gets or sets the object to get the test set data from.
        /// </summary>
        public static ITestSetData TestSetData { get; set; }

        /// <summary>
        /// Gets or sets the object to get the test case data from.
        /// </summary>
        public static ITestCaseData TestCaseData { get; set; }

        /// <summary>
        /// Gets or sets the object to get the test step data from.
        /// </summary>
        public static ITestStepData TestStepData { get; set; }

        /// <summary>
        /// Gets or sets the testing driver to run the testing program on.
        /// </summary>
        public static ITestAutomationDriver TestAutomationDriver { get; set; }

        /// <summary>
        /// Gets or sets the reporter object.
        /// </summary>
        public static Reporter Reporter { get; set; }

        /// <summary>
        /// Sets up all the variables and paths.
        /// </summary>
        public static void SetUp()
        {
            string csvSaveLocation = Environment.GetEnvironmentVariable("csvSaveFileLocation");
            string logSaveLocation = Environment.GetEnvironmentVariable("logSaveFileLocation");
            string reportSaveLocation = Environment.GetEnvironmentVariable("reportSaveFileLocation");
            string screenshotSaveLocation = Environment.GetEnvironmentVariable("screenshotSaveLocation");

            string xmlFile = Environment.GetEnvironmentVariable("testSetDataArgs");
            string xmlFileName = xmlFile.Substring(xmlFile.LastIndexOf("\\") + 1);

            CSVLogger = new CSVLogger(csvSaveLocation + "\\" + $"{xmlFileName}.csv");
            LogSaveFileLocation = logSaveLocation;
            ScreenshotSaveLocation = screenshotSaveLocation;

            Reporter = new Reporter()
            {
                SaveFileLocation = Path.Combine(reportSaveLocation, "Report.txt"),
            };

            RespectRepeatFor = bool.Parse(Environment.GetEnvironmentVariable("respectRepeatFor"));
            RespectRunAODAFlag = bool.Parse(Environment.GetEnvironmentVariable("respectRunAODAFlag"));

            Directory.CreateDirectory(csvSaveLocation);
            Directory.CreateDirectory(logSaveLocation);
            Directory.CreateDirectory(reportSaveLocation);
            Directory.CreateDirectory(screenshotSaveLocation);
        }
    }
}
