// <copyright file="TestSetBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.TestAutomationDriver;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;

    /// <summary>
    /// Creates a new Information Object and returns it.
    /// </summary>
    public class TestSetBuilder
    {
        /// <summary>
        /// The usable places to get data.
        /// </summary>
        public enum TestingDataType
        {
            /// <summary>
            /// Application life cycle managment.
            /// </summary>
            Alm,

            /// <summary>
            /// From a SQL database.
            /// </summary>
            Sql,

            /// <summary>
            /// From an XML File.
            /// </summary>
            XML,

            /// <summary>
            /// From a txt file.
            /// </summary>
            Text,
        }

        /// <summary>
        /// Gets or sets the browser to use in this test.
        /// </summary>
        public string Browser { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the environment to go to.
        /// </summary>
        public string Environment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL the browser should land on first.
        /// </summary>
        public string URL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the passed in respectRunAODAFlag parameter.
        /// </summary>
        public string PassedInRespectRunAODAFlag { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the passed in respectRepeatFor parameter.
        /// </summary>
        public string PassedInRespectRepeatFor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timeout threshold.
        /// </summary>
        public int TimeOutThreshold { get; set; } = 120;

        /// <summary>
        /// Gets or sets the warning threshold. Note that the warning threshold should be less than the timeout threshold.
        /// </summary>
        public int WarningThreshold { get; set; } = 0;

        /// <summary>
        /// Gets or sets the csv save file location.
        /// </summary>
        public string CsvSaveFileLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the log save file location.
        /// </summary>
        public string LogSaveFileLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the report save file location.
        /// </summary>
        public string ReportSaveFileLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the screenshot save location.
        /// </summary>
        public string ScreenshotSaveLocation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets which TestingDataDriver to use.
        /// </summary>
        public string TestingDataDriver { get; set; }

        /// <summary>
        /// Gets or sets where to get the test set data.
        /// </summary>
        public string TestSetDataType { get; set; }

        /// <summary>
        /// Gets or sets where to get the test case data.
        /// </summary>
        public string TestCaseDataType { get; set; }

        /// <summary>
        /// Gets or sets where to get the test step.
        /// </summary>
        public string TestStepDataType { get; set; }

        /// <summary>
        /// Gets or sets the args of the test set data. Most often the location.
        /// </summary>
        public string TestSetDataArgs { get; set; }

        /// <summary>
        /// Gets or sets the args of the test case data. Most often the location.
        /// </summary>
        public string TestCaseDataArgs { get; set; }

        /// <summary>
        /// Gets or sets the args of the test step. Most often the location.
        /// </summary>
        public string TestStepDataArgs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to respectRunAODAFlag or not.
        /// </summary>
        private bool RespectRunAODAFlag { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to respectRepeatFor or not.
        /// </summary>
        private bool RespectRepeatFor { get; set; } = false;

        private string LoadingSpinner { get; set; }

        private string ErrorContainer { get; set; }

        /// <summary>
        /// Sets the values for the informaton object.
        /// </summary>
        /// <returns>The test Set to run.</returns>
        public TestSet Build()
        {
            TestSet testSet;

            this.InsantiateTestingDriver();
            this.InsantiateTestSetData();
            this.InsantiateTestCaseData();
            this.InsantiateTestStepData();

            InformationObject.CSVLogger = new CSVLogger(this.CsvSaveFileLocation);
            InformationObject.LogSaveFileLocation = this.LogSaveFileLocation;
            InformationObject.Reporter = new Reporter()
            {
                SaveFileLocation = this.ReportSaveFileLocation + "\\Report.txt",
            };
            InformationObject.RespectRepeatFor = this.RespectRepeatFor;
            InformationObject.RespectRunAODAFlag = this.RespectRunAODAFlag;

            testSet = new TestSet();

            return testSet;
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test StepData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestStepData()
        {
            InformationObject.TestStepData = (ITestStepData)this.GetTestData(2, this.TestStepDataType, this.TestStepDataArgs);
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test CaseData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestCaseData()
        {
            InformationObject.TestCaseData = (ITestCaseData)this.GetTestData(1, this.TestCaseDataType, this.TestCaseDataArgs);
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test SetData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestSetData()
        {
            InformationObject.TestSetData = (ITestSetData)this.GetTestData(0, this.TestSetDataType, this.TestSetDataArgs);
            InformationObject.TestSetData.SetUpTestSet();
        }

        /// <summary>
        /// The actual method to get the test set/case/step data.
        /// </summary>
        /// <param name="testDataType"> 0 = testSetData, 1 = testCaseData, 2 = testStepData. </param>
        /// <param name="dataTypeName"> Name where to get the data from. </param>
        /// <param name="dataTypeLocation"> Arguments for the data. </param>
        /// <returns> The test data.</returns>
        private ITestData GetTestData(int testDataType, string dataTypeName, string dataTypeLocation)
        {
            ITestData testData = null;

            switch (testDataType)
            {
                case 0:
                    testData = ReflectiveGetter.GetImplementationOfType<ITestSetData>()
                                .Find(x => x.Name.Equals(dataTypeName));
                    break;
                case 1:
                    testData = ReflectiveGetter.GetImplementationOfType<ITestCaseData>()
                                .Find(x => x.Name.Equals(dataTypeName));
                    break;
                case 2:
                    testData = ReflectiveGetter.GetImplementationOfType<ITestStepData>()
                                .Find(x => x.Name.Equals(dataTypeName));
                    break;
                default:
                    throw new Exception("Not a valid testDataType");
            }

            if (testData == null)
            {
                Console.WriteLine($"Sorry we do not currently support reading tests from: {dataTypeName}");
                throw new Exception($"Cannot Find test data type {dataTypeName}");
            }
            else
            {
                testData.InformationLocation = dataTypeLocation;
                testData.SetUp();
            }

            return testData;
        }

        private void InsantiateTestingDriver()
        {
            switch (this.TestingDataDriver.ToLower())
            {
                case "selenium":
                    TestAutomationBuilder builder;

                    ITestAutomationDriver.Browser browser = ITestAutomationDriver.Browser.Chrome;

                    if (this.Browser.ToLower().Contains("chrome"))
                    {
                        browser = ITestAutomationDriver.Browser.Chrome;
                    }
                    else if (this.Browser.ToLower().Contains("ie"))
                    {
                        browser = ITestAutomationDriver.Browser.IE;
                    }
                    else if (this.Browser.ToLower().Contains("firefox"))
                    {
                        browser = ITestAutomationDriver.Browser.Firefox;
                    }
                    else if (this.Browser.ToLower().Contains("edge"))
                    {
                        browser = ITestAutomationDriver.Browser.Edge;
                    }
                    else
                    {
                        Logger.Error($"Sorry we do not currently support the browser: {this.Browser}");
                        throw new Exception("Unsupported Browser.");
                    }

                    builder = new TestAutomationBuilder()
                    {
                        Browser = browser,
                        TimeOutThreshold = TimeSpan.FromSeconds(this.TimeOutThreshold),
                        Environment = this.Environment,
                        URL = this.URL,
                        ScreenshotSaveLocation = this.ScreenshotSaveLocation,
                        ErrorContainer = this.ErrorContainer,
                        LoadingSpinner = this.LoadingSpinner,
                    };
                    InformationObject.TestAutomationDriver = builder.BuildSeleniumDriver();
                    break;
                default:
                    Logger.Error($"Sorry we do not currently support the testing application: {this.TestingDataDriver}");
                    throw new Exception("Unsupported testing application");
            }
        }
    }
}
