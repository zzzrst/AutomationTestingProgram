﻿// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.TestingDriver;

    /// <summary>
    /// Main program.
    /// </summary>
    public class FrameworkDriver
    {
        /// <summary>
        /// The Main functionality.
        /// </summary>
        /// <param name="args">Arguments to be passed in.</param>
        public static void Main(string[] args)
        {
            // Parameters to be used for the information object.
            string browser = string.Empty;
            string environment = string.Empty;
            string url = string.Empty;
            string respectRunAODAFlag = string.Empty;
            string respectRepeatFor = string.Empty;
            int timeOutThreshold = 0;
            int warningThreshold = 0;
            string csvSaveFileLocation = string.Empty;
            string logSaveFileLocation = string.Empty;
            string screenshotSaveLocation = string.Empty;
            string testingDataDriver = string.Empty;
            string testSetData = string.Empty;
            string testCaseData = string.Empty;
            string testStepData = string.Empty;
            string testSetDataLocation = string.Empty;
            string testCaseDataLocation = string.Empty;
            string testStepDataLocation = string.Empty;

            InformationObjectBuilder builder = new InformationObjectBuilder()
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
                TestSetData = testSetData,
                TestCaseData = testCaseData,
                TestStepData = testStepData,
                TestSetDataLocation = testSetDataLocation,
                TestCaseDataLocation = testCaseDataLocation,
                TestStepDataLocation = testStepDataLocation,
            };
            builder.Build();
        }
    }
}
