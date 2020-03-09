﻿// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
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
                SetSaveFileParameters();
            }

            if (!errorParsing)
            {
                TestSetBuilder setBuilder = new TestSetBuilder();
                TestSet testSet = setBuilder.Build();

                TestAutomationBuilder automationBuilder = new TestAutomationBuilder();
                automationBuilder.Build();

                DateTime start = DateTime.UtcNow;

                AutomationTestSetDriver.RunTestSet(testSet);
                InformationObject.Reporter.Report();

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

        private static void SetSaveFileParameters()
        {
            if (Environment.GetEnvironmentVariable("logSaveFileLocation") == string.Empty)
            {
                Environment.SetEnvironmentVariable("logSaveFileLocation", Environment.GetEnvironmentVariable("csvSaveFileLocation"));
            }

            if (Environment.GetEnvironmentVariable("reportSaveFileLocation") == string.Empty)
            {
                Environment.SetEnvironmentVariable("reportSaveFileLocation", Environment.GetEnvironmentVariable("csvSaveFileLocation"));
            }

            if (Environment.GetEnvironmentVariable("screenshotSaveLocation") == string.Empty)
            {
                Environment.SetEnvironmentVariable("screenshotSaveLocation", Environment.GetEnvironmentVariable("csvSaveFileLocation"));
            }
        }

        private static bool ParseTestSetParameters()
        {
            bool errorParsing = false;
            Dictionary<string, string> parameters;

            ITestGeneralData dataInformation = ReflectiveGetter.GetImplementationOfType<ITestGeneralData>()
                .Find(x => x.Name.Equals(Environment.GetEnvironmentVariable("testSetDataArgs")));

            if (dataInformation.Verify(Environment.GetEnvironmentVariable("testSetDataArgs")))
            {
                parameters = dataInformation.ParseParameters(Environment.GetEnvironmentVariable("testSetDataArgs"), Environment.GetEnvironmentVariable("dataFile"));
                foreach (string paramName in parameters.Keys)
                {
                    // If it's not filled in already, fill it in.
                    if (Environment.GetEnvironmentVariable(paramName) == string.Empty)
                    {
                        Environment.SetEnvironmentVariable(paramName, parameters[paramName]);
                    }
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
                   Environment.SetEnvironmentVariable("testingDataDriver", o.AutomationProgram ?? "selenium");
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
