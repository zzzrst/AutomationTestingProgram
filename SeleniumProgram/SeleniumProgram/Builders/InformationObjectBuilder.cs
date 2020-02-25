namespace AutomationTestingProgram.Builders
{
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.TestingDriver;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Creates a new Information Object and returns it.
    /// </summary>
    public class InformationObjectBuilder
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

            /// <summary>
            /// From a txt file.
            /// </summary>
            TextFile,
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
        public string TestSetData { get; set; }

        /// <summary>
        /// Gets or sets where to get the test case data.
        /// </summary>
        public string TestCaseData { get; set; }

        /// <summary>
        /// Gets or sets where to get the test step.
        /// </summary>
        public string TestStepData { get; set; }

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
        public void Build()
        {
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
        }

        private void InsantiateTestStepData()
        {
            throw new NotImplementedException();
        }

        private void InsantiateTestCaseData()
        {
            throw new NotImplementedException();
        }

        private void InsantiateTestSetData()
        {
            throw new NotImplementedException();
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
                    Console.WriteLine($"Sorry we do not currently support {this.TestingDataDriver}");
                    break;
            }
        }
    }
}
