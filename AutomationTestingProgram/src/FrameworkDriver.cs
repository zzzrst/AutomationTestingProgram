// <copyright file="FrameworkDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using ALMConnector;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.GeneralData;
    using AutomationTestSetFramework;
    using AzureReporter;
    using CommandLine;
    using Microsoft.SharePoint.Client;
    using Microsoft.TeamFoundation.TestManagement.WebApi;
    using NPOI.HPSF;
    using static InformationObject;

    /// <summary>
    /// Main program.
    /// </summary>
    public class FrameworkDriver
    {
        private static string update = "false";

        /// <summary>
        /// The Main functionality.
        /// </summary>
        /// <param name="args">Arguments to be passed in.</param>
        /// <returns> 0 if no errors were met. </returns>
        [RequiresAssemblyFiles]
        public static int Main(string[] args)
        {
            bool errorParsing;
            int resultCode = 1;

            Logger.Info($"Running Automation Testing Program Version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");

            errorParsing = ParseCommandLine(args);

            if (!errorParsing)
            {
                if (update.Equals("true"))
                {
                    Logger.Info("Checking for updates...");
                    if (CheckForUpdates(Assembly.GetExecutingAssembly().Location))
                    {
                        string newArgs = string.Join(" ", args.Select(x => string.Format("\"{0}\"", x)).ToList());
                        using (Process p = new Process())
                        {
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
                        }

                        Thread.Sleep(5000);

                        // Closes the current process
                        Environment.Exit(0);
                    }
                    else
                    {
                        Logger.Info("Program is up to date");
                    }
                }

                errorParsing = ParseTestSetParameters();

                // this will set the default parameters for the environment variables
                SetDefaultParameters();
            }

            if (!errorParsing)
            {
                // catch interrupts for C Sharp for the entire program
                Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

                Logger.Info("Beginning set up all parts");

                // Set up all the parts.
                InformationObject.SetUp();

                Logger.Info("Setting up the Test Set Builder");
                TestSetBuilder setBuilder = new TestSetBuilder();

                // create a null test set
                TestSet testSet = new TestSet(); 
                try
                {
                    testSet = setBuilder.Build();
                }
                catch (Exception ex)
                {
                    // catch errors with setting up the test set
                    Logger.Error("Unhandled exception with setting up the test " + ex);

                    Environment.Exit(-1);
                    return resultCode;
                }

                Logger.Info("Setting up the Test Automation Builder");
                TestAutomationBuilder automationBuilder = new TestAutomationBuilder();
                automationBuilder.Build();

                // Run main program.
                Logger.Info("Running main program");
                DateTime start = DateTime.UtcNow;

                try
                {
                    AutomationTestSetDriver.RunTestSet(testSet);
                }
                catch (Exception ex)
                {
                    Logger.Info("Automation Testing Program Ran into an Unhandled Exception. Investigate!");
                    Logger.Info("Exception caught: " + ex);
                }

                try
                {
                    RunAODA();
                }
                catch (Exception ex)
                {
                    testSet.TestSetStatus.RunSuccessful = false;
                    Logger.Error("Run AODA failed with unhandled exception");
                }

                DateTime end = DateTime.UtcNow;

                string resultString = testSet.TestSetStatus.RunSuccessful ? "successful" : "not successful";

                InformationObject.CSVLogger.AddResults($"Total Time for Run, {Math.Abs((start - end).TotalSeconds)}");
                InformationObject.CSVLogger.AddResults($"Run Result, {resultString}");
                try
                {
                    InformationObject.CSVLogger.WriteOutResults();
                    InformationObject.Reporter.Report();
                }
                catch (Exception ex)
                {
                    testSet.TestSetStatus.RunSuccessful = false;
                    Logger.Error("Unhandled exception while executing WriteOutResults or Report " + ex);
                }

                Logger.Info($"Automation Testing Program has finished. It was {resultString}");
                // delete temporary_files folder
                string path_temp = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"]);
                if (Directory.Exists(path_temp))
                {
                    Directory.Delete(path_temp, true);
                }

                if (testSet.TestSetStatus.RunSuccessful)
                {
                    resultCode = 0; // test passed
                }
                else
                {
                    resultCode = 1; // test failed but not because of exception
                }
            }
            else
            {
                resultCode = 1; // test failed from exception
            }

            Logger.Info("Now returning with result code " + resultCode);

            Environment.Exit(resultCode);
            return resultCode;
        }

        /// <summary>
        /// Catches interrupts for the console interrupt.
        /// </summary>
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Logger.Info("Program was interrupted");
            Reporter tmp = new Reporter("");
            tmp.ReportAborted();
            Environment.Exit(0);
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
        [RequiresAssemblyFiles]
        public static void RunAODA()
        {
            if (InformationObject.RespectRunAODAFlag.ToString().ToUpper() == "TRUE")
            {
                Logger.Info("RunAODA assessing...");
                // TestSetData add attachment
                string path_ex = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                string tempFolder = $"{path_ex}\\temp\\";

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
                ZipFile.CreateFromDirectory(tempFolder, $"{path_ex}\\Log\\{zipFileName}");

                InformationObject.TestSetData.AddAODAReport($"{path_ex}\\Log\\{zipFileName}");

                // Remove all remaining contents.
                Directory.Delete(tempFolder, true);

                Logger.Info("Finished AODA accessment");
            }

            InformationObject.TestAutomationDriver.Quit();
        }

        private static string GetLatestReleaseVersion(string url)
        {
            // updated by Victor to HTTP Client, a newer version of WebClient
            WebClient wc = new WebClient();

            //Uri myUri = new Uri(URLInStringFormat, UriKind.Absolute);

            string result = wc.DownloadString(url);
            Regex rx = new Regex("v[0-9]*[.][0-9]*[.][0-9]*");
            return rx.Match(result).Value.Substring(1);
        }

        private static void SetDefaultParameters()
        {
            if (GetEnvironmentVariable(EnvVar.CsvSaveFileLocation) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.CsvSaveFileLocation, "Log"); // set csv file as default (added by Victor)
            }

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

            // Victor: I found that App.Config isn't used correctly when not using ALM. The default values defined are not set.
            if (GetEnvironmentVariable(EnvVar.RespectRepeatFor) == string.Empty)
            {
                // added by Victor
                SetEnvironmentVariable(EnvVar.RespectRepeatFor, ConfigurationManager.AppSettings["RespectRepeatFor"]);
            }

            if (GetEnvironmentVariable(EnvVar.RespectRunAODAFlag) == string.Empty)
            {
                // changed by Victor
                SetEnvironmentVariable(EnvVar.RespectRunAODAFlag, ConfigurationManager.AppSettings["RespectRunAODAFlag"]);
            }

            if (GetEnvironmentVariable(EnvVar.MaxFailures) == string.Empty)
            {
                SetEnvironmentVariable(EnvVar.MaxFailures, ConfigurationManager.AppSettings["MaxConsecutiveFailedTestCases"]);
            }

            // added by Victor for testing purposes
            if (GetEnvironmentVariable(EnvVar.TimeOutThreshold) == "0")
            {
                SetEnvironmentVariable(EnvVar.TimeOutThreshold, ConfigurationManager.AppSettings["TimeOutThreshold"]);

                // SetEnvironmentVariable(EnvVar.TimeOutThreshold, "240");
            }

            // added by Victor for testing purposes
            if (GetEnvironmentVariable(EnvVar.WarningThreshold) == "0")
            {
                SetEnvironmentVariable(EnvVar.WarningThreshold, ConfigurationManager.AppSettings["WarningThreshold"]);

                // SetEnvironmentVariable(EnvVar.TimeOutThreshold, "240");
            }
        }

        private static bool ParseTestSetParameters()
        {
            bool errorParsing = false;
            Dictionary<EnvVar, string> parameters;

            // set the enviro variables to empty
            SetEnvironmentVariable(EnvVar.LoadingSpinner, string.Empty);
            SetEnvironmentVariable(EnvVar.ErrorContainer, string.Empty);

            // get the test set data type
            var l = GetEnvironmentVariable(EnvVar.TestSetDataType);

            // print out the test set data type
            Logger.Info("test set data type is: " + l);

            ITestGeneralData dataInformation = ReflectiveGetter.GetImplementationOfType<ITestGeneralData>()
                .Find(x => x.Name.Equals(GetEnvironmentVariable(EnvVar.TestSetDataType)));
            if (dataInformation != null)
            {
                if (dataInformation.Verify(GetEnvironmentVariable(EnvVar.TestSetDataArgs)))
                {
                    parameters = dataInformation.ParseParameters(GetEnvironmentVariable(EnvVar.TestSetDataArgs), GetEnvironmentVariable(EnvVar.DataFile));
                    //Logger.Info("parameters from ALM: " + parameters.Keys);

                    foreach (EnvVar paramName in parameters.Keys)
                    {
                        // print out before
                        Logger.Info(GetEnvironmentVariable(paramName));

                        // If it's not filled in already, fill it in.
                        if (GetEnvironmentVariable(paramName) == string.Empty
                            || GetEnvironmentVariable(paramName) == "0")
                        {
                            SetEnvironmentVariable(paramName, parameters[paramName]);
                        }

                        // log out the new parameter name
                        Logger.Info(GetEnvironmentVariable(paramName));
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
            Logger.Info("Parsing command line");

            bool errorParsing = false;
            SetEnvironmentVariable(EnvVar.Attempts, string.Empty);
            Parser.Default.ParseArguments<FrameworkOptions>(args)
               .WithParsed(o =>
               {
                   // set default browser as chrome
                   SetEnvironmentVariable(EnvVar.Browser, o.Browser ?? " Chrome");

                   SetEnvironmentVariable(EnvVar.Environment, o.Environment ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.URL, o.URL ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.RespectRepeatFor, o.RespectRepeatFor ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.RespectRunAODAFlag, o.RespectRunAodaFlag ?? string.Empty);

                   // added by Victor
                   SetEnvironmentVariable(EnvVar.TimeOutThreshold, o.TimeOutThreshold.ToString());
                   SetEnvironmentVariable(EnvVar.WarningThreshold, o.WarningThreshold.ToString());
                   SetEnvironmentVariable(EnvVar.DataFile, o.DataFile ?? string.Empty);

                   SetEnvironmentVariable(EnvVar.CsvSaveFileLocation, o.CSVSaveFileLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.LogSaveFileLocation, o.LogSaveLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.ReportSaveFileLocation, o.ReportSaveLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.ScreenshotSaveLocation, o.ScreenShotSaveLocation ?? string.Empty);
                   SetEnvironmentVariable(EnvVar.TestAutomationDriver, o.AutomationProgram ?? "selenium");

                   // set the default project name
                   string defaultProject = System.Configuration.ConfigurationManager.AppSettings["PROJECT_NAME"];
                   InformationObject.TestProjectName = o.ProjectName ?? defaultProject;

                   // create the default set type as excel
                   SetEnvironmentVariable(EnvVar.TestSetDataType, o.TestSetDataType ?? "Excel");

                   // set the number of max failures to max failures dot to string.
                   SetEnvironmentVariable(EnvVar.MaxFailures, o.MaxFailures?.ToString() ?? string.Empty);

                   // create the default set type as excel
                   SetEnvironmentVariable(EnvVar.AzurePAT, o.AzurePAT ?? "");

                   // here we will override App.Config with our command line values
                   string overrides = o.AppConfigOverrides ?? string.Empty;

                   List<string> overRidesList = overrides.Split('|').ToList();

                   if (overRidesList.Count() > 0)
                   {
                       foreach (string valueOverride in overRidesList)
                       {
                           string[] keyValuePair = valueOverride.Split(':');

                           if (keyValuePair.Length == 2)
                           {
                               Logger.Info("Setting app setting key: " + keyValuePair[0] + " to value: " + keyValuePair[1]);
                               System.Configuration.ConfigurationManager.AppSettings[keyValuePair[0]] = keyValuePair[1];
                           }
                           else
                           {
                               Logger.Info("Key value pair is not of length 2, check colon, :,  usage");
                           }
                       }
                   }

                   // set the notification list based on the inputted values. 
                   InformationObject.NotifyEmails = o.NotifyList?.Split(",").ToList();

                   // set the test plan folder structure
                   InformationObject.FolderStructure = o.TestPlanStructure ?? string.Empty;

                   List<string> runParams = o.RunParameters?.Split(",")?.ToList();

                   if (runParams != null && runParams.Count > 0)
                   {
                       foreach (string param in runParams)
                       {
                           if (param == string.Empty)
                           {
                               continue;
                           }

                           List<string> key_value_pair = param.Split(':').ToList();
                           InformationObject.RunParameters.Add(key_value_pair[0], key_value_pair[1]);
                           Logger.Info("Added RunParameters: " + key_value_pair[0] + " with value " + key_value_pair[1]);
                       }
                   }

                   InformationObject.ExecutionURL = o.ExecutionURL ?? string.Empty;

                   InformationObject.ReleaseEnvUri = o.ReleaseEnvironmentUri ?? string.Empty;

                   InformationObject.TesterEmail = o.TesterContact ?? System.Configuration.ConfigurationManager.AppSettings["DefaultTesterEmail"];
                   InformationObject.TesterName = o.TesterName ?? System.Configuration.ConfigurationManager.AppSettings["DefaultTesterName"];

                   Logger.Info("Execution URL is: " + InformationObject.ExecutionURL);

                   // secret information variable.
                   SetEnvironmentVariable(EnvVar.SecretInformation, o.SecretInformation ?? "");

                   Logger.Info("Secret Value is: " + o.SecretInformation);

                   // if build number is empty, then call Sharepoint Getter to get the build number
                   if (o.BuildNumber == null)
                   {
                       // string buildNum = Helper.SharepointGetter.GetBuildNumber(o.Environment);
                       // Logger.Info("Build num not changed");
                   }

                   SetEnvironmentVariable(EnvVar.BuildNumber, o.BuildNumber ?? string.Empty);
                   ALMConnector.TestSetInstance.buildNumber = o.BuildNumber ?? "my build number";

                   // are we sure? Args are the same as the test set data type?
                   // double question mark means that if the first value is null, then use the second value
                   SetEnvironmentVariable(EnvVar.TestCaseDataType, o.TestCaseDataType ?? GetEnvironmentVariable(EnvVar.TestSetDataType));

                   // SetEnvironmentVariable(EnvVar.TestCaseDataType, o.TestCaseDataType ?? "Database");
                   SetEnvironmentVariable(EnvVar.TestStepDataType, o.TestStepDataType ?? GetEnvironmentVariable(EnvVar.TestCaseDataType));

                   // commenting out test case data args since it's being assigned as the test set data arg automatically
                   if (GetEnvironmentVariable(EnvVar.TestCaseDataType) == "Excel" || GetEnvironmentVariable(EnvVar.TestCaseDataType) == "XML")
                   {
                       InformationObject.TestSetName = Path.GetFileName(o.TestSetDataArgs).Replace(".xlsx", string.Empty);
                       InformationObject.OriginalTestSetDirectoryName = Path.GetDirectoryName(o.TestSetDataArgs);

                       // we will copy the file and run from C TEMP
                       string exAssembly = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                       string logFolder = "Log";
                       string temp_file_name = "temp_" + InformationObject.TestSetName + ".xlsx";
                       string tempFile = exAssembly + "\\" + logFolder + "\\" + temp_file_name;

                       // copy the execution file to a new folder
                       try
                       {
                           // third param is for overwrite
                           System.IO.File.Copy(o.TestSetDataArgs, tempFile, true);

                           Logger.Info($"Replaced file {o.TestSetDataArgs} with {tempFile}");
                           o.TestSetDataArgs = tempFile;
                       }
                       catch (IOException iox)
                       {
                           Logger.Error(iox.Message);
                       }

                       SetEnvironmentVariable(EnvVar.TestSetDataArgs, o.TestSetDataArgs);
                       SetEnvironmentVariable(EnvVar.TestCaseDataArgs, o.TestCaseDataArgs ?? o.TestSetDataArgs);
                       SetEnvironmentVariable(EnvVar.TestStepDataArgs, o.TestStepDataArgs ?? o.TestSetDataArgs);
                       InformationObject.TestPlanName = o.TestPlanName ?? InformationObject.TestSetName;
                   }

                   // db execution
                   else
                   {
                       SetEnvironmentVariable(EnvVar.TestSetDataArgs, o.TestSetDataArgs);
                       InformationObject.TestSetName = o.TestSetDataArgs;
                       InformationObject.TestPlanName = o.TestPlanName ?? InformationObject.TestSetName;
                   }
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
