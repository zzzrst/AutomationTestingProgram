// <copyright file="FrameworkOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// This class stores the command line arguments that are taken in, both mandatory and optional.
    /// </summary>
    public class FrameworkOptions
    {
        /// <summary>
        /// Gets usage for AutomationTestingProgram.
        /// </summary>
        [Usage(ApplicationAlias = "AutomationTestingProgram.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Example.",
                    new FrameworkOptions()
                    {
                    });
            }
        }

        /// <summary>
        /// Gets or sets the browser to use.
        /// </summary>
        [Option('b', "browser", Required = false, HelpText = "Overrides the browser set in the Testing files.")]
        public string Browser { get; set; }

        /// <summary>
        /// Gets or sets the environment to use.
        /// </summary>
        [Option('e', "environment", Required = false, HelpText = "Overrides the browser set in the Testing files.")]
        public string Environment { get; set; }

        /// <summary>
        /// Gets or sets the URL to use. By default, the URL is derived by the environment provided.
        /// </summary>
        [Option('u', "url", Required = false, HelpText = "Overrides the url set in the Testing files. Also overrides the derived URL from the environment.")]
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to respect RespectRepeatFor flag.
        /// </summary>
        [Option("respectRepeatFor", Required = false, HelpText = "Overrides the respect repeat for flag set in the Testing files")]
        public string RespectRepeatFor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to respect RunAODAFlag.
        /// </summary>
        [Option("respectRunAODAFlag", Required = false, HelpText = "Overrides the respect run AODA flag set in the Testing files")]
        public string RespectRunAodaFlag { get; set; }

        /// <summary>
        /// Gets or sets the timeout threshold to use.
        /// </summary>
        [Option("timeOutThreshold", Required = false, HelpText = "Overrides the timeout threshold set in the Testing files")]
        public int TimeOutThreshold { get; set; }

        /// <summary>
        /// Gets or sets the warning threshold. Warning must be less than Timeout.
        /// </summary>
        [Option("warningThreshold", Required = false, HelpText = "Overrides the warning threshold set in the Testing files")]
        public int WarningThreshold { get; set; }

        /// <summary>
        /// Gets or sets the datafile.
        /// </summary>
        [Option("dataFile", Required = false, HelpText = "Overrides the data file location")]
        public string DataFile { get; set; }

        /// <summary>
        /// Gets or sets the CSV file location.
        /// </summary>
        [Option("csvSaveFileLocation", Required = false, HelpText = "Overrides the csv save file location set in the Testing files")]
        public string CSVSaveFileLocation { get; set; }

        /// <summary>
        /// Gets or sets the log save location.
        /// <para> </para>
        /// If not set, will use the directory path of CSVSaveFileLocation.
        /// </summary>
        [Option("logSaveLocation", Required = false, HelpText = "Overrides the log save location set in the Testing files")]
        public string LogSaveLocation { get; set; }

        /// <summary>
        /// Gets or sets the log save location.
        /// <para> </para>
        /// If not set, will use the directory path of CSVSaveFileLocation.
        /// </summary>
        [Option("reportSaveLocation", Required = false, HelpText = "Overrides the report save location set in the Testing files")]
        public string ReportSaveLocation { get; set; }

        /// <summary>
        /// Gets or sets the screenshot save location.
        /// <para> </para>
        /// If not set, will use the directory path of CSVSaveFileLocation.
        /// </summary>
        [Option("screenShotSaveLocation", Required = false, HelpText = "Overrides the screenshot save location set in the Testing files")]
        public string ScreenShotSaveLocation { get; set; }

        /// <summary>
        /// Gets or sets the Testing driver to use.
        /// <para> </para>
        /// If not set, will use Selenium.
        /// </summary>
        [Option("automationProgram", Required = false, HelpText = "The Automation Program to use. Defaults to Selenium.")]
        public string AutomationProgram { get; set; }

        /// <summary>
        /// Gets or sets the type of data to use for the test set.
        /// </summary>
        [Option("setType", Required = true, HelpText = "The data type to get the test set from. " +
            "The type avaliable are: XML")]
        public string TestSetDataType { get; set; }

        /// <summary>
        /// Gets or sets the type of data to use for the test case.
        /// <para> </para>
        /// If not set, will use the test set data type.
        /// </summary>
        [Option("caseType", Required = false, HelpText = "The data type to get the test case from. Defaults to the test step type." +
            "The type avaliable are: XML, Txt")]
        public string TestCaseDataType { get; set; }

        /// <summary>
        /// Gets or sets the type of data to use for the test step.
        /// <para> </para>
        /// If not set, will use the test case data type.
        /// </summary>
        [Option("stepType", Required = false, HelpText = "The data type to get the test step from. Defaults to the test case type." +
            "The type avaliable are: XML, Txt")]
        public string TestStepDataType { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the test set test data. Most often it is the location of the file.
        /// </summary>
        [Option("setArgs", Required = true, HelpText = "The argument to use for the test step. Most often the file location.")]
        public string TestSetDataArgs { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the test case test data. Most often the location of the file.
        /// <para> </para>
        /// If not set, will use the test set args.
        /// </summary>
        [Option("caseArgs", Required = false, HelpText = "The argument to use for the test case. Most often the file location. Defaults to the test set args.")]
        public string TestCaseDataArgs { get; set; }

        /// <summary>
        /// Gets or sets the arguments for the test step test data. Most often the location of the file.
        /// <para> </para>
        /// If not set, will use the test case args.
        /// </summary>
        [Option("stepArgs", Required = false, HelpText = "The argument to use for the test step. Most often the file location. Defaults to the test case args.")]
        public string TestStepDataArgs { get; set; }
    }
}
