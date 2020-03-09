// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
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
        // Parameters to be used for the to build the test.
        private static Dictionary<string, string> Parameters = new Dictionary<string, string>()
        {
        browser = string.Empty;
        environment = string.Empty;
        private static string url = string.Empty;
        private static string respectRunAODAFlag = string.Empty;
        private static string respectRepeatFor = string.Empty;
        private static int timeOutThreshold = 0;
        private static int warningThreshold = 0;
        private static string dataFile = string.Empty;
        private static string csvSaveFileLocation = string.Empty;
        private static string logSaveFileLocation = string.Empty;
        private static string reportSaveFileLocation = string.Empty;
        private static string screenshotSaveLocation = string.Empty;
        private static string testingDataDriver = string.Empty;
        private static string testSetDataType = string.Empty;
        private static string testCaseDataType = string.Empty;
        private static string testStepDataType = string.Empty;
        private static string testSetDataArgs = string.Empty;
        private static string testCaseDataArgs = string.Empty;
        private static string testStepDataArgs = string.Empty;
    };


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
            }

            if (!errorParsing)
            {
                TestSetBuilder builder = new TestSetBuilder()
                {
                    Browser = browser,
                    Environment = environment,
                    URL = url,
                    PassedInRespectRepeatFor = respectRepeatFor,
                    PassedInRespectRunAODAFlag = respectRunAODAFlag,
                    TimeOutThreshold = timeOutThreshold,
                    WarningThreshold = warningThreshold,
                    CsvSaveFileLocation = csvSaveFileLocation,
                    LogSaveFileLocation = logSaveFileLocation,
                    ScreenshotSaveLocation = screenshotSaveLocation,
                    ReportSaveFileLocation = csvSaveFileLocation,
                    TestingDataDriver = testingDataDriver,
                    TestSetDataType = testSetDataType,
                    TestCaseDataType = testCaseDataType,
                    TestStepDataType = testStepDataType,
                    TestSetDataArgs = testSetDataArgs,
                    TestCaseDataArgs = testCaseDataArgs,
                    TestStepDataArgs = testStepDataArgs,
                };
                TestSet testSet = builder.Build();
            Environment.GetEnvironmentVariables
                DateTime start = DateTime.UtcNow;

                AutomationTestSetDriver.RunTestSet(testSet);
                InformationObject.Reporter.Report();

                DateTime end = DateTime.UtcNow;

                InformationObject.CSVLogger.AddResults($"Total, {Math.Abs((start - end).TotalSeconds)}");
                //InformationObject.CSVLogger.WriteOutResults();

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

        private static bool ParseTestSetParameters()
        {
            bool errorParsing = false;
            Dictionary<string, string> parameters;

            ITestGeneralData dataInformation = ReflectiveGetter.GetImplementationOfType<ITestGeneralData>()
                .Find(x => x.Name.Equals(testSetDataType));

            if (dataInformation.Verify(testSetDataArgs))
            {
                parameters = dataInformation.ParseParameters(testSetDataArgs, dataFile);
                foreach (string value in parameters.Keys)
                {
                    nameof
                }
            }
            else
            {
                errorParsing = true;
            }

            return errorParsing;
        }

        private static bool ParseCommandLine(string[] args)
        {
            bool errorParsing = false;
            Parser.Default.ParseArguments<FrameworkOptions>(args)
               .WithParsed<FrameworkOptions>(o =>
               {
                   browser = o.Browser ?? string.Empty;
                   environment = o.Environment ?? string.Empty;
                   url = o.URL ?? string.Empty;
                   respectRepeatFor = o.RespectRepeatFor ?? string.Empty;
                   respectRunAODAFlag = o.RespectRunAodaFlag ?? string.Empty;
                   timeOutThreshold = o.TimeOutThreshold;
                   warningThreshold = o.WarningThreshold;
                   dataFile = o.DataFile ?? string.Empty;
                   csvSaveFileLocation = o.CSVSaveFileLocation ?? string.Empty;
                   logSaveFileLocation = o.LogSaveLocation ?? string.Empty;
                   reportSaveFileLocation = o.ReportSaveLocation ?? string.Empty;
                   screenshotSaveLocation = o.ScreenShotSaveLocation ?? string.Empty;
                   testingDataDriver = o.AutomationProgram ?? "selenium";
                   testSetDataType = o.TestSetDataType;
                   testCaseDataType = o.TestCaseDataType ?? testStepDataType;
                   testStepDataType = o.TestStepDataType ?? testCaseDataType;
                   testSetDataArgs = o.TestSetDataArgs;
                   testCaseDataArgs = o.TestCaseDataArgs ?? testSetDataArgs;
                   testStepDataArgs = o.TestStepDataArgs ?? testCaseDataArgs;
               })
               .WithNotParsed<FrameworkOptions>(errs =>
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
