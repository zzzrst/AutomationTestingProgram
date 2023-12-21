// <copyright file="InformationObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.TestingData;
    using Spire.Pdf.Exporting.XPS.Schema;
    using TestingDriver;
    using Path = System.IO.Path;

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
            /// Build Number of the run.
            /// string.
            /// </summary>
            BuildNumber,

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

            /// <summary>
            /// PAT for the Azure DvvOps exeuction.
            /// </summary>
            AzurePAT,

            /// <summary>
            /// Secrets from the pipeline execution on DevOps.
            /// </summary>
            SecretInformation,

            /// <summary>
            /// Maximum failures before entire execution fails.
            /// </summary>
            MaxFailures,
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
        /// Gets or sets a run parameter.
        /// </summary>
        public static Dictionary<string, string> RunParameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the test set name.
        /// </summary>
        public static string TestSetName { get; set; }

        /// <summary>
        /// Gets or sets a value whether or not this program should execute.
        /// </summary>
        public static bool ShouldExecute { get; set; }

        /// <summary>
        /// Gets or sets a value whether or not this program should execute.
        /// </summary>
        public static bool BlockTestSet { get; set; }

        /// <summary>
        /// Gets or sets a list indicating the people to notify after failure.
        /// </summary>
        public static List<string> NotifyEmails { get; set; }

        /// <summary>
        /// Gets or sets a list indicating the people to notify after failure.
        /// </summary>
        public static string OriginalTestSetDirectoryName { get; set; }

        /// <summary>
        /// Gets or sets the URL corresponding to the DevOps execution.
        /// </summary>
        public static string ExecutionURL { get; set; }

        /// <summary>
        /// Gets or sets the test plan name corresponding to the execution.
        /// </summary>
        public static string TestPlanName { get; set; }

        /// <summary>
        /// Gets or sets the test suite folder structure for the execution when reporting to DevOps.
        /// </summary>
        public static string FolderStructure { get; set; }

        /// <summary>
        /// Gets or sets the test plan name corresponding to the execution.
        /// </summary>
        public static string TestProjectName { get; set; }
        /// <summary>

        /// Gets or sets the tester email for the corresponding test set the execution.
        /// </summary>
        public static string TesterEmail { get; set; }

        /// Gets or sets the tester Name for the corresponding test set the execution.
        /// </summary>
        public static string TesterName { get; set; }

        /// Gets or sets the release uri and environment uri for the corresponding test set and its execution. 
        /// Must be from DevOps, and must come together. Comma separated. 
        /// </summary>
        public static string ReleaseEnvUri { get; set; }

        /// <summary>
        /// Sets up all the variables and paths. 
        /// </summary>
        public static void SetUp()
        {
            InformationObject.ShouldExecute = true;
            InformationObject.BlockTestSet = false;

            Logger.Info("Beginning SetUp");
            string csvSaveLocation = GetEnvironmentVariable(EnvVar.CsvSaveFileLocation);
            string logSaveLocation = GetEnvironmentVariable(EnvVar.LogSaveFileLocation);
            string reportSaveLocation = GetEnvironmentVariable(EnvVar.ReportSaveFileLocation);
            string screenshotSaveLocation = GetEnvironmentVariable(EnvVar.ScreenshotSaveLocation);

            string csvFileName = InformationObject.TestSetName;
            Logger.Info("CSV file name is: " + csvSaveLocation + "\\" + csvFileName);

            // add CSV results execution log file
            CSVLogger = new CSVLogger(csvSaveLocation + "\\" + $"{csvFileName}_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.csv");
            CSVLogger.AddResults($"Transaction, {DateTime.Now:G}");
            CSVLogger.AddResults($"Environment, {GetEnvironmentVariable(EnvVar.Environment)}");
            CSVLogger.AddResults(","); // create line separator
            CSVLogger.AddResults("Test Step Name, Description, Expected, Run Successful, Step Number, Speed, Error Trace, Friendly Error Message");

            LogSaveFileLocation = logSaveLocation;
            ScreenshotSaveLocation = screenshotSaveLocation;

            RespectRepeatFor = bool.Parse(GetEnvironmentVariable(EnvVar.RespectRepeatFor));
            RespectRunAODAFlag = bool.Parse(GetEnvironmentVariable(EnvVar.RespectRunAODAFlag));

            try
            {
                Directory.CreateDirectory(csvSaveLocation);
                Directory.CreateDirectory(logSaveLocation);
                Directory.CreateDirectory(reportSaveLocation);
                Directory.CreateDirectory(screenshotSaveLocation);
                Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"]));
            }
            catch (ArgumentException)
            {
                throw new Exception("Missing file path parameters. On one or more of these Directories. " +
                    "csv,log,report,screenshot");
            }

            string now = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");

            RunParameters.Add("UNIQUE_IDENTIFIER", now);

            Console.WriteLine("Added UNIQUE IDENTIFIER with value: " + now);

            SetReporter(reportSaveLocation);
        }

        /// <summary>
        /// Sets the given enviornemnt variable with the given value.
        /// </summary>
        /// <param name="variable">The Environment Variable to set.</param>
        /// <param name="value">What Value to set it.</param>
        public static void SetEnvironmentVariable(EnvVar variable, string value)
        {
            Logger.Info("set environment variable: " + variable.ToString() + " to: " + value);
            Environment.SetEnvironmentVariable(variable.ToString(), value);
        }

        /// <summary>
        /// Gets the given environment variable.
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
                    // Reporter = new Reporter(Path.Combine(reportSaveLocation, "Report.txt"));
                    Reporter = new ALMReporter(string.Empty);
                    break;
                default:
                    string testSetFile = GetEnvironmentVariable(EnvVar.TestSetDataArgs);
                    //string excelName = testSetFile.Substring(testSetFile.LastIndexOf("\\") + 1);
                    //excelName = excelName.Substring(0, excelName.IndexOf("."));

                    Reporter = new Reporter(Path.Combine(reportSaveLocation, $"Report_{InformationObject.TestSetName}_{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.txt"));
                    break;
            }
        }
    }
}
