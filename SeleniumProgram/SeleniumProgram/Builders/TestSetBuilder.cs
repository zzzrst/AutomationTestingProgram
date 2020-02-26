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
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.DataDrivers;
    using AutomationTestingProgram.TestingDriver;

    /// <summary>
    /// Creates a new Information Object and returns it.
    /// </summary>
    public class TestSetBuilder
    {
        /// <summary>
        /// The usable testing applications.
        /// </summary>
        public enum TestingDriverType
        {
            /// <summary>
            /// Selenium program.
            /// </summary>
            Selenium,
        }

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
        /// Gets or sets the location of the test set data.
        /// </summary>
        public string TestSetDataLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the test case data.
        /// </summary>
        public string TestCaseDataLocation { get; set; }

        /// <summary>
        /// Gets or sets the location of the test step.
        /// </summary>
        public string TestStepDataLocation { get; set; }

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
            ITestStepData testStepData = null;
            testStepData = ReflectiveGetter.GetEnumerableOfType<ITestStepData>()
                .Find(x => x.Name.Equals(this.TestStepDataType));

            if (testStepData == null)
            {
                Console.WriteLine($"Sorry we do not currently support reading test cases from: {this.TestStepDataType}");
                throw new Exception($"Cannot Find test case data type {this.TestStepDataType}");
            }
            else
            {
                testStepData.InformationLocation = this.TestStepDataLocation;
                InformationObject.TestStepData = testStepData;
            }
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test CaseData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestCaseData()
        {
            ITestCaseData testCaseData = null;

            testCaseData = ReflectiveGetter.GetEnumerableOfType<ITestCaseData>()
                .Find(x => x.Name.Equals(this.TestCaseDataType));

            if (testCaseData == null)
            {
                Console.WriteLine($"Sorry we do not currently support reading test cases from: {this.TestCaseDataType}");
                throw new Exception($"Cannot Find test case data type {this.TestCaseDataType}");
            }
            else
            {
                testCaseData.InformationLocation = this.TestCaseDataLocation;
                InformationObject.TestCaseData = testCaseData;
            }
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test SetData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestSetData()
        {
            ITestSetData testSetData = null;

            testSetData = ReflectiveGetter.GetEnumerableOfType<ITestSetData>()
                .Find(x => x.Name.Equals(this.TestSetDataType));

            if (testSetData == null)
            {
                Console.WriteLine($"Sorry we do not currently support reading test cases from: {this.TestSetDataType}");
                throw new Exception($"Cannot Find test case data type {this.TestSetDataType}");
            }
            else
            {
                testSetData.InformationLocation = this.TestSetDataLocation;
                InformationObject.TestSetData = testSetData;
            }
        }

        private void InsantiateTestingDriver()
        {
            switch (this.TestingDataDriver.ToLower())
            {
                case "selenium":
                    SeleniumDriverBuilder builder;

                    ITestingDriver.Browser browser = ITestingDriver.Browser.Chrome;

                    if (this.Browser.ToLower().Contains("chrome"))
                    {
                        browser = ITestingDriver.Browser.Chrome;
                    }
                    else if (this.Browser.ToLower().Contains("ie"))
                    {
                        browser = ITestingDriver.Browser.IE;
                    }
                    else if (this.Browser.ToLower().Contains("firefox"))
                    {
                        browser = ITestingDriver.Browser.Firefox;
                    }
                    else if (this.Browser.ToLower().Contains("edge"))
                    {
                        browser = ITestingDriver.Browser.Edge;
                    }

                    builder = new SeleniumDriverBuilder()
                    {
                        Browser = browser,
                        TimeOutThreshold = TimeSpan.FromSeconds(this.TimeOutThreshold),
                        Environment = this.Environment,
                        URL = this.URL,
                        ScreenshotSaveLocation = this.ScreenshotSaveLocation,
                        ErrorContainer = this.ErrorContainer,
                        LoadingSpinner = this.LoadingSpinner,
                    };
                    InformationObject.TestingDriver = builder.BuildSeleniumDriver();
                    break;
                default:
                    Console.WriteLine($"Sorry we do not currently support the testing application: {this.TestingDataDriver}");
                    break;
            }
        }
    }
}
