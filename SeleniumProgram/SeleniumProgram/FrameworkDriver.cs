// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.TestingDriver;
    using AutomationTestSetFramework;

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
            string browser = "chrome";
            string environment = "test";
            string url = "https://www.google.ca/";
            string respectRunAODAFlag = "False";
            string respectRepeatFor = "False";
            int timeOutThreshold = 5;
            int warningThreshold = 5;
            string csvSaveFileLocation = "C:\\SeleniumPerfXML\\Logs";
            string logSaveFileLocation = "C:\\SeleniumPerfXML\\Logs";
            string screenshotSaveLocation = "C:\\SeleniumPerfXML\\ScreenShots";
            string testingDataDriver = "Selenium";
            string testSetDataType = "XML";
            string testCaseDataType = "XML";
            string testStepDataType = "XML";
            string testSetDataLocation = "C:\\SeleniumPerfXML\\SampleXML.xml";
            string testCaseDataLocation = "C:\\SeleniumPerfXML\\SampleXML.xml";
            string testStepDataLocation = "C:\\SeleniumPerfXML\\SampleXML.xml";

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
                TestSetDataLocation = testSetDataLocation,
                TestCaseDataLocation = testCaseDataLocation,
                TestStepDataLocation = testStepDataLocation,
            };
            TestSet testSet = builder.Build();

            DateTime start = DateTime.UtcNow;

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            DateTime end = DateTime.UtcNow;

            InformationObject.CSVLogger.AddResults($"Total, {Math.Abs((start - end).TotalSeconds)}");
            InformationObject.CSVLogger.WriteOutResults();

            string resultString = testSet.TestSetStatus.RunSuccessful ? "successfull" : "not successful";
            Logger.Info($"SeleniumPerfXML has finished. It was {resultString}");
        }
    }
}
