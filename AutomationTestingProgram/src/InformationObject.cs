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
    using AutomationTestingProgram.TestingData;
    using TestingDriver;

    /// <summary>
    /// An information class that contains information needed by other objects/methods.
    /// Also contains the information about enviornemnt variables created by the program.
    /// </summary>
    public static class InformationObject
    {
        /// <summary>
        /// Enviornement variables used by the program.
        /// </summary>
        public enum EnvVar
        {
            /// <summary>
            /// The attempts to try before timing out.
            /// </summary>
            Attempts,

            /// <summary>
            /// The Browser To use.
            /// browser type.
            /// </summary>
            Browser,

            /// <summary>
            /// The Environment you are running the test in.
            /// string.
            /// </summary>
            Environment,

            /// <summary>
            /// URL For the test website.
            /// string.
            /// </summary>
            URL,

            /// <summary>
            /// Whether or not the program should respect repeat for flags.
            /// bool.
            /// </summary>
            RespectRepeatFor,

            /// <summary>
            /// Wether or not the program should respect AODA flags.
            /// bool.
            /// </summary>
            RespectRunAODAFlag,

            /// <summary>
            /// Time out threshold.
            /// int.
            /// </summary>
            TimeOutThreshold,

            /// <summary>
            /// Warning Threshold.
            /// int.
            /// </summary>
            WarningThreshold,

            /// <summary>
            /// Location of the data file.
            /// string.
            /// </summary>
            DataFile,

            /// <summary>
            /// Location to save the csv file.
            /// string.
            /// </summary>
            CsvSaveFileLocation,

            /// <summary>
            /// Location to save the log.
            /// string,
            /// </summary>
            LogSaveFileLocation,

            /// <summary>
            /// Location to save the report.
            /// string.
            /// </summary>
            ReportSaveFileLocation,

            /// <summary>
            /// Location to save the screenshot.
            /// string.
            /// </summary>
            ScreenshotSaveLocation,

            /// <summary>
            /// What type of automation driver to use.
            /// </summary>
            TestAutomationDriver,

            /// <summary>
            /// The test set data file type.
            /// </summary>
            TestSetDataType,

            /// <summary>
            /// The test case data file type.
            /// </summary>
            TestCaseDataType,

            /// <summary>
            /// The test step data file type.
            /// </summary>
            TestStepDataType,

            /// <summary>
            /// Test Set args, usualy the location of the data file.
            /// </summary>
            TestSetDataArgs,

            /// <summary>
            /// Test Case args, usualy the location of the data file.
            /// </summary>
            TestCaseDataArgs,

            /// <summary>
            /// Test Step args, usualy the location of the data file.
            /// </summary>
            TestStepDataArgs,

            /// <summary>
            /// Loading spinner.
            /// </summary>
            LoadingSpinner,

            /// <summary>
            /// Error Container.
            /// </summary>
            ErrorContainer,
        }

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
        public static ITestingDriver TestAutomationDriver { get; set; }

        /// <summary>
        /// Gets or sets the reporter object.
        /// </summary>
        public static Reporter Reporter { get; set; }

        /// <summary>
        /// Sets up all the variables and paths.
        /// </summary>
        public static void SetUp()
        {
            string csvSaveLocation = GetEnvironmentVariable(EnvVar.CsvSaveFileLocation);
            string logSaveLocation = GetEnvironmentVariable(EnvVar.LogSaveFileLocation);
            string reportSaveLocation = GetEnvironmentVariable(EnvVar.ReportSaveFileLocation);
            string screenshotSaveLocation = GetEnvironmentVariable(EnvVar.ScreenshotSaveLocation);

            string testSetFile = GetEnvironmentVariable(EnvVar.TestSetDataArgs);
            string csvFileName = testSetFile.Substring(testSetFile.LastIndexOf("\\") + 1);
            csvFileName = csvFileName.Substring(0, csvFileName.Length - 4);

            CSVLogger = new CSVLogger(csvSaveLocation + "\\" + $"{csvFileName}.csv");
            CSVLogger.AddResults($"Transaction, {DateTime.Now:G}");
            CSVLogger.AddResults($"Environment URL, {GetEnvironmentVariable(EnvVar.URL)}");

            LogSaveFileLocation = logSaveLocation;
            ScreenshotSaveLocation = screenshotSaveLocation;

            RespectRepeatFor = bool.Parse(GetEnvironmentVariable(EnvVar.RespectRepeatFor));
            RespectRunAODAFlag = bool.Parse(GetEnvironmentVariable(EnvVar.RespectRunAODAFlag));

            Directory.CreateDirectory(csvSaveLocation);
            Directory.CreateDirectory(logSaveLocation);
            Directory.CreateDirectory(reportSaveLocation);
            Directory.CreateDirectory(screenshotSaveLocation);

            SetReporter(reportSaveLocation);
        }

        /// <summary>
        /// Sets the given enviornemnt variable with the given value.
        /// </summary>
        /// <param name="variable">The Environment Variable to set.</param>
        /// <param name="value">What Value to set it.</param>
        public static void SetEnvironmentVariable(EnvVar variable, string value)
        {
            Environment.SetEnvironmentVariable(variable.ToString(), value);
        }

        /// <summary>
        /// Gets the given envirnment variable.
        /// </summary>
        /// <param name="variable">Variable to return.</param>
        /// <returns>The value of the variable.</returns>
        public static string GetEnvironmentVariable(EnvVar variable)
        {
            string value = Environment.GetEnvironmentVariable(variable.ToString());
            if (value == null)
            {
                value = string.Empty;
            }

            return value;
        }

        private static void SetReporter(string reportSaveLocation)
        {
            switch (GetEnvironmentVariable(EnvVar.TestSetDataType).ToLower())
            {
                case "alm":
                    Reporter = new ALMReporter(string.Empty);
                    break;
                default:
                    Reporter = new Reporter(Path.Combine(reportSaveLocation, "Report.txt"));
                    break;
            }
        }
    }
}
