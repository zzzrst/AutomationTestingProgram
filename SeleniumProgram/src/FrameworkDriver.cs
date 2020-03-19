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
    using System.Reflection;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.GeneralData;
    using AutomationTestingProgram.TestAutomationDriver;
    using AutomationTestSetFramework;
    using CommandLine;

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
                Logger.Info($"SeleniumPerfXML has finished. It was {resultString}");
            }
            else
            {
                resultCode = 1;
            }

            Environment.Exit(resultCode);
            return resultCode;
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

        private static void SetDefaultParameters()
        {
            if (Environment.GetEnvironmentVariable("logSaveFileLocation") == null)
            {
                Environment.SetEnvironmentVariable("logSaveFileLocation", Environment.GetEnvironmentVariable("csvSaveFileLocation"));
            }

            if (Environment.GetEnvironmentVariable("reportSaveFileLocation") == null)
            {
                Environment.SetEnvironmentVariable("reportSaveFileLocation", Environment.GetEnvironmentVariable("csvSaveFileLocation"));
            }

            if (Environment.GetEnvironmentVariable("screenshotSaveLocation") == null)
            {
                Environment.SetEnvironmentVariable("screenshotSaveLocation", Environment.GetEnvironmentVariable("csvSaveFileLocation"));
            }

            if (Environment.GetEnvironmentVariable("respectRepeatFor") == null)
            {
                Environment.SetEnvironmentVariable("respectRepeatFor", "false");
            }

            if (Environment.GetEnvironmentVariable("respectRunAODAFlag") == null)
            {
                Environment.SetEnvironmentVariable("respectRunAODAFlag", "false");
            }
        }

        private static bool ParseTestSetParameters()
        {
            bool errorParsing = false;
            Dictionary<string, string> parameters;

            Environment.SetEnvironmentVariable("loadingSpinner", string.Empty);
            Environment.SetEnvironmentVariable("errorContainer", string.Empty);

            ITestGeneralData dataInformation = ReflectiveGetter.GetImplementationOfType<ITestGeneralData>()
                .Find(x => x.Name.Equals(Environment.GetEnvironmentVariable("testSetDataType")));

            if (dataInformation.Verify(Environment.GetEnvironmentVariable("testSetDataArgs")))
            {
                parameters = dataInformation.ParseParameters(Environment.GetEnvironmentVariable("testSetDataArgs"), Environment.GetEnvironmentVariable("dataFile"));
                foreach (string paramName in parameters.Keys)
                {
                    // If it's not filled in already, fill it in.
                    if (Environment.GetEnvironmentVariable(paramName) == null || Environment.GetEnvironmentVariable(paramName) == string.Empty
                        || Environment.GetEnvironmentVariable(paramName) == "0")
                    {
                        Environment.SetEnvironmentVariable(paramName, parameters[paramName]);
                    }
                }
            }
            else
            {
                Logger.Error("Error verifying test set data.");
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
                   Environment.SetEnvironmentVariable("browser", o.Browser ?? string.Empty);
                   Environment.SetEnvironmentVariable("environment", o.Environment ?? string.Empty);
                   Environment.SetEnvironmentVariable("url", o.URL ?? string.Empty);
                   Environment.SetEnvironmentVariable("respectRepeatFor", o.RespectRepeatFor ?? string.Empty);
                   Environment.SetEnvironmentVariable("respectRunAODAFlag", o.RespectRunAodaFlag ?? string.Empty);
                   Environment.SetEnvironmentVariable("timeOutThreshold", o.TimeOutThreshold.ToString());
                   Environment.SetEnvironmentVariable("warningThreshold", o.WarningThreshold.ToString());
                   Environment.SetEnvironmentVariable("dataFile", o.DataFile ?? string.Empty);
                   Environment.SetEnvironmentVariable("csvSaveFileLocation", o.CSVSaveFileLocation ?? string.Empty);
                   Environment.SetEnvironmentVariable("logSaveFileLocation", o.LogSaveLocation ?? string.Empty);
                   Environment.SetEnvironmentVariable("reportSaveFileLocation", o.ReportSaveLocation ?? string.Empty);
                   Environment.SetEnvironmentVariable("screenshotSaveLocation", o.ScreenShotSaveLocation ?? string.Empty);
                   Environment.SetEnvironmentVariable("testAutomationDriver", o.AutomationProgram ?? "selenium");
                   Environment.SetEnvironmentVariable("testSetDataType", o.TestSetDataType);
                   Environment.SetEnvironmentVariable("testCaseDataType", o.TestCaseDataType ?? Environment.GetEnvironmentVariable("testSetDataType"));
                   Environment.SetEnvironmentVariable("testStepDataType", o.TestStepDataType ?? Environment.GetEnvironmentVariable("testCaseDataType"));
                   Environment.SetEnvironmentVariable("testSetDataArgs", o.TestSetDataArgs);
                   Environment.SetEnvironmentVariable("testCaseDataArgs", o.TestCaseDataArgs ?? Environment.GetEnvironmentVariable("testSetDataArgs"));
                   Environment.SetEnvironmentVariable("testStepDataArgs", o.TestStepDataArgs ?? Environment.GetEnvironmentVariable("testCaseDataArgs"));
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
