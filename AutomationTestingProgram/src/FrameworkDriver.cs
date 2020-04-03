// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.GeneralData;
    using AutomationTestSetFramework;
    using CommandLine;
    using TestingDriver;
    using static InformationObject;

    /// <summary>
    /// Main program.
    /// </summary>
    public class FrameworkDriver
    {
        /// <summary>
        /// The Main functionality.
        /// </summary>
        /// <param name="args">Arguments to be passed in.</param>
        /// <returns> 0 if no errors were met. </returns>
        public static int Main(string[] args)
        {
            Logger.Info("Checking for updates...");
            if (false && CheckForUpdates(Assembly.GetExecutingAssembly().Location))
            {
                string newArgs = string.Join(" ", args.Select(x => string.Format("\"{0}\"", x)).ToList());
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    FileName = "AutoUpdater.exe",
                    Arguments = newArgs,
                };

                p.StartInfo = startInfo;
                p.Start();

                Thread.Sleep(5000);

                // Closes the current process
                Environment.Exit(0);
            }
            else
            {
                Logger.Info("Program is up to date");
            }

            bool errorParsing;
            int resultCode = 0;

            errorParsing = ParseCommandLine(args);

            if (!errorParsing)
            {
                errorParsing = ParseTestSetParameters();
                SetDefaultParameters();
            }

            if (!errorParsing)
            {
                // Set up all the parts.
                InformationObject.SetUp();

                TestSetBuilder setBuilder = new TestSetBuilder();
                TestSet testSet = setBuilder.Build();

                TestAutomationBuilder automationBuilder = new TestAutomationBuilder();
                automationBuilder.Build();

                Logger.Info($"Running AutomationFramework Version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
                // Run main program.
                DateTime start = DateTime.UtcNow;
                AutomationTestSetDriver.RunTestSet(testSet);
                InformationObject.Reporter.Report();

                RunAODA();

                DateTime end = DateTime.UtcNow;

                InformationObject.CSVLogger.AddResults($"Total, {Math.Abs((start - end).TotalSeconds)}");
                InformationObject.CSVLogger.WriteOutResults();

                string resultString = testSet.TestSetStatus.RunSuccessful ? "successfull" : "not successful";
                Logger.Info($"Automation Testing Program has finished. It was {resultString}");
            }
            else
            {
                resultCode = 1;
            }

            Environment.Exit(resultCode);
            return resultCode;
        }

        /// <summary>
        /// Checks to see if there is any update avalible.
        /// </summary>
        /// <param name="program">Name of the program to check.</param>.
        /// <returns>true if there are updates.</returns>
        public static bool CheckForUpdates(string program)
        {
            Version currentReleaseVersion = new Version(FileVersionInfo.GetVersionInfo(program).ProductVersion);

            // get the release version
            Version latestReleaseVersion = new Version(GetLatestReleaseVersion("https://github.com/zzzrst/AutomationTestingProgram/releases/latest"));

            Logger.Info($"Current Version: {currentReleaseVersion}");

            if (latestReleaseVersion.CompareTo(currentReleaseVersion) > 0)
            {
                Logger.Info($"Program is out of date! Version {latestReleaseVersion} is avaliable.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the AODA result to the file location.
        /// </summary>
        public static void RunAODA()
        {
            if (InformationObject.RespectRunAODAFlag)
            {
                string tempFolder = $"{InformationObject.LogSaveFileLocation}\\temp\\";

                // Delete temp folder if exist and recreate
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }

                Directory.CreateDirectory(tempFolder);

                // Generate AODA Results
                InformationObject.TestAutomationDriver.GenerateAODAResults(tempFolder);

                // Zip all the contents up & Timestamp it
                string zipFileName = $"AODA_Results_{DateTime.Now:MM_dd_yyyy_hh_mm_ss_tt}.zip";
                ZipFile.CreateFromDirectory(tempFolder, $"{InformationObject.LogSaveFileLocation}\\{zipFileName}");

                // Remove all remaining contents.
                Directory.Delete(tempFolder, true);
            }

            InformationObject.TestAutomationDriver.Quit();
        }

        private static string GetLatestReleaseVersion(string url)
        {
            WebClient wc = new WebClient();
            string result = wc.DownloadString(url);
            Regex rx = new Regex("v[0-9]*[.][0-9]*[.][0-9]*");
            return rx.Match(result).Value.Substring(1);
        }

        private static void SetDefaultParameters()
        {
            if (GetEnvironmentVariable(EnvVar.LogSaveFileLocation) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.LogSaveFileLocation, GetEnvironmentVariable(EnvVar.CsvSaveFileLocation));
            }

            if (GetEnvironmentVariable(EnvVar.ReportSaveFileLocation) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.ReportSaveFileLocation, GetEnvironmentVariable(EnvVar.CsvSaveFileLocation));
            }

            if (GetEnvironmentVariable(EnvVar.ScreenshotSaveLocation) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.ScreenshotSaveLocation, GetEnvironmentVariable(EnvVar.CsvSaveFileLocation));
            }

            if (GetEnvironmentVariable(EnvVar.RespectRepeatFor) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.RespectRepeatFor, "false");
            }

            if (GetEnvironmentVariable(EnvVar.RespectRunAODAFlag) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.RespectRunAODAFlag, "false");
            }
        }

        private static bool ParseTestSetParameters()
        {
            bool errorParsing = false;
            Dictionary<EnvVar, string> parameters;

            SetEnvironmentVariable(EnvVar.LoadingSpinner, string.Empty);
            SetEnvironmentVariable(EnvVar.ErrorContainer, string.Empty);
            var l = GetEnvironmentVariable(EnvVar.TestSetDataType);
            ITestGeneralData dataInformation = ReflectiveGetter.GetImplementationOfType<ITestGeneralData>()
                .Find(x => x.Name.Equals(GetEnvironmentVariable(EnvVar.TestSetDataType)));
            if (dataInformation != null)
            {
                if (dataInformation.Verify(GetEnvironmentVariable(EnvVar.TestSetDataArgs)))
                {
                    parameters = dataInformation.ParseParameters(GetEnvironmentVariable(EnvVar.TestSetDataArgs), GetEnvironmentVariable(EnvVar.DataFile));
                    foreach (EnvVar paramName in parameters.Keys)
                    {
                        // If it's not filled in already, fill it in.
                        if (GetEnvironmentVariable(paramName) == string.Empty
                            || GetEnvironmentVariable(paramName) == "0")
                        {
                            SetEnvironmentVariable(paramName, parameters[paramName]);
                        }
                    }
                }
                else
                {
                    Logger.Error("Error verifying test set data.");
                    errorParsing = true;
                }
            }
            else
            {
                Logger.Error($"General data implementation does not exist for {GetEnvironmentVariable(EnvVar.TestSetDataType)}");
                errorParsing = true;
            }

            return errorParsing;
        }

        private static bool ParseCommandLine(string[] args)
        {
            bool errorParsing = false;
            Parser.Default.ParseArguments<FrameworkOptions>(args)
               .WithParsed(o =>
               {
                   SetEnvironmentVariable(EnvVar.Browser, o.Browser ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.Environment, o.Environment ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.URL, o.URL ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.RespectRepeatFor, o.RespectRepeatFor ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.RespectRunAODAFlag, o.RespectRunAodaFlag ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.TimeOutThreshold, o.TimeOutThreshold.ToString());
                   SetEnvironmentVariable(EnvVar.WarningThreshold, o.WarningThreshold.ToString());
                   SetEnvironmentVariable(EnvVar.DataFile, o.DataFile ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.CsvSaveFileLocation, o.CSVSaveFileLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.LogSaveFileLocation, o.LogSaveLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.ReportSaveFileLocation, o.ReportSaveLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.ScreenshotSaveLocation, o.ScreenShotSaveLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.TestAutomationDriver, o.AutomationProgram ?? "selenium");
                   SetEnvironmentVariable(EnvVar.TestSetDataType, o.TestSetDataType);
                   SetEnvironmentVariable(EnvVar.TestCaseDataType, o.TestCaseDataType ?? GetEnvironmentVariable(EnvVar.TestSetDataType));
                   SetEnvironmentVariable(EnvVar.TestStepDataType, o.TestStepDataType ?? GetEnvironmentVariable(EnvVar.TestCaseDataType));
                   SetEnvironmentVariable(EnvVar.TestSetDataArgs, o.TestSetDataArgs);
                   SetEnvironmentVariable(EnvVar.TestCaseDataArgs, o.TestCaseDataArgs ?? GetEnvironmentVariable(EnvVar.TestSetDataArgs));
                   SetEnvironmentVariable(EnvVar.TestStepDataArgs, o.TestStepDataArgs ?? GetEnvironmentVariable(EnvVar.TestCaseDataArgs));
               })
               .WithNotParsed(errs =>
               {
                   Logger.Error(errs);
                   if (errs != null)
                   {
                       errorParsing = true;
                   }
               });
            return errorParsing;
        }
    }
}
