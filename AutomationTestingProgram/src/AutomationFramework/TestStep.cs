// <copyright file="TestStep.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestSetFramework;
    using Microsoft.TeamFoundation.Pipelines.WebApi;
    using static AutomationTestingProgram.InformationObject;
    using static AutomationTestSetFramework.IMethodBoundaryAspect;

    /// <summary>
    /// An Implementation of the ITestStep class.
    /// </summary>
    public class TestStep : ITestStep
    {
        /// <inheritdoc/>
        public virtual string Name { get; set; } = "Test Step";

        /// <summary>
        /// Gets or sets arguments used for the test Step.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets a value indicating whether you should execute this step or skip it.
        /// </summary>
        public bool ShouldExecuteVariable { get; set; } = true;

        /// <inheritdoc/>
        public int TestStepNumber { get; set; } = 0;

        /// <inheritdoc/>
        public ITestStepStatus TestStepStatus { get; set; }

        /// <inheritdoc/>
        public FlowBehavior OnExceptionFlowBehavior { get; set; } = FlowBehavior.Return;

        /// <summary>
        /// Gets or sets a value indicating whether to run AODA.
        /// </summary>
        public bool RunAODA { get; set; } = false;

        /// <summary>
        /// Gets or sets the AODA page name.
        /// </summary>
        public string RunAODAPageName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether test step should be logged.
        /// </summary>
        public bool ShouldLog { get; set; } = true;

        /// <summary>
        /// Gets or sets number of attempts before a fail is thrown.
        /// </summary>
        public int MaxAttempts { get; set; } = 1;

        /// <summary>
        /// Gets or sets the description of the test step.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether determines whether the test step is optional or not.
        /// </summary>
        public bool Optional { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the test step should pass when it passes.
        /// </summary>
        public bool PassCondition { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to continue test case when a failure occurs.
        /// </summary>
        public bool ContinueOnError { get; set; } = false;

        /// <summary>
        /// Gets or sets the timeout indicate the timeout during the execution.
        /// </summary>
        public int LocalTimeout { get; set; } = 0;

        private int Attempts { get; set; } = 0;

        /// <inheritdoc/>
        public virtual void Execute()
        {
            // increase attempts in failures (moved by Victor)
            this.Attempts++;

            // added by Victor to log attempt number
            Logger.Info("Attempt number: " + this.Attempts + "/" + this.MaxAttempts);
        }

        /// <inheritdoc/>
        [RequiresAssemblyFiles]
        public virtual void HandleException(Exception e)
        {
            // edited by Victor to show attempt number correctly
            // this.TestStepStatus.Name += "Attempt 1/" + this.MaxAttempts;
            this.TestStepStatus.Name += " attempt " + this.Attempts + "/" + this.MaxAttempts;
            this.TestStepStatus.Description += this.Description + " FAILED ";
            this.TestStepStatus.Expected += " execute " + this.Description + " successfully";
            this.TestStepStatus.Actual += " failure in executing " + this.Description + "!\n" + e.ToString();
            this.TestStepStatus.ErrorStack += e.StackTrace;
            this.TestStepStatus.FriendlyErrorMessage += e.Message;
            this.TestStepStatus.RunSuccessful = false;

            // this is just for the console log
            // this action is different than the one that corresponds to TestStepStatus.
            this.Description += " FAILED";
            this.Name += " attempt " + this.Attempts + "/" + this.MaxAttempts;

            Logger.Error(e.Message);

            // added by Victor to print out Friendly error message
            Logger.Info("Error occured, handling exception");

            InformationObject.TestAutomationDriver.CheckErrorContainer();

            string fileName = $"{DateTime.Now:yyyy_MM_dd-hh_mm_ss_tt}.png";

            string path_ex = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // string codeBase = AppDomain.CurrentDomain.BaseDirectory;

            // implemented a new and old screenshot methodology
            bool success = InformationObject.TestAutomationDriver.TakeEntireScreenshot($"{path_ex}\\Log\\NEW_{fileName}", false);

            if (success)
            {
                InformationObject.TestSetData.AddErrorScreenshot($"{path_ex}\\Log\\NEW_{fileName}");
                InformationObject.Reporter.AddTestStepScreenshot(fileName, $"{path_ex}\\Log\\NEW_{fileName}", this.TestStepStatus.Actual);
            }
            else
            {
                Logger.Info("Full screen unsuccessful, taking current screen");
                InformationObject.TestAutomationDriver.TakeScreenShot($"{path_ex}\\Log\\OLD_{fileName}");
                InformationObject.TestSetData.AddErrorScreenshot($"{path_ex}\\Log\\OLD_{fileName}");
                InformationObject.Reporter.AddTestStepScreenshot(fileName, $"{path_ex}\\Log\\OLD_{fileName}", this.TestStepStatus.Actual);
            }

            // check if debug mode is on. Debug mode allows a user to enter new values as executions happen.
            if (ConfigurationManager.AppSettings["DebugMode"] == "true")
            {
                Console.WriteLine("Failure in test execution! \nThese are the current values: ");

                // Debug mode is on, then we will allow the user to modify any object they want
                foreach (string key in this.Arguments.Keys)
                {
                    Console.WriteLine($"Key Value pair is: {key} with value {this.Arguments[key]}");
                }

                string input_key; // = Console.ReadLine();
                string input_val;
                Console.WriteLine("Please input the new value(s) or click enter to skip\nNew key:");

                while ((input_key = Console.ReadLine()) != null && input_key != string.Empty)
                {
                    if (!this.Arguments.ContainsKey(input_key))
                    {
                        Console.WriteLine("invalid key, please retry\nNew key:");
                        continue;
                    }

                    Console.WriteLine($"Please indicate the value for key: {input_key}");
                    input_val = Console.ReadLine();

                    if (input_val != null)
                    {
                        this.Arguments[input_key] = input_val;
                        Console.WriteLine($"Successfully set input value: {input_val}\nNew key:");
                    }
                    else
                    {
                        Console.WriteLine("input value null, please retry\nNew key:");
                        continue;
                    }
                }
            }

            if (this.ShouldLog)
            {
                ITestStepLogger log = new TestStepLogger();
                log.Log(this);
            }
        }

        /// <inheritdoc/>
        public virtual void SetUp()
        {
            if (this.TestStepStatus == null)
            {
                var timeUTC = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, easternZone);
                this.TestStepStatus = new TestStepStatus()
                {
                    // set to defalt values
                    Name = this.Name,
                    StartTime = easternTime,
                    TestStepNumber = this.TestStepNumber,
                    Description = this.Description,
                    Expected = this.Description,
                    Optional = this.Optional,
                    ContinueOnError = this.ContinueOnError,
                    // RunSuccessful = false, // default to false (added by Victor)
                };
            }

            // look at the past test step
            if (!this.ShouldExecuteVariable)
            {
                this.TestStepStatus.Actual = "N/A";
            }
            else
            {
                // set local timeout to this dot local timeout
                // if N/A, then skip set args
                InformationObject.TestAutomationDriver.LocalTimeout = this.LocalTimeout;
                InformationObject.TestStepData.SetArguments(this);
            }
        }

        /// <inheritdoc/>
        public virtual bool ShouldExecute()
        {

            if (((this.Attempts > 0) && (this.Attempts < this.MaxAttempts)) && (this.TestStepStatus.RunSuccessful == false))
            {
                // when a run is unsuccessful
                return true;
            }
            else if ((this.Attempts > 0) && (this.TestStepStatus.RunSuccessful == true))
            {
                // if the program run is successful
                return false;
            }

            return this.ShouldExecuteVariable && this.Attempts < this.MaxAttempts;
        }

        /// <inheritdoc/>
        public virtual void TearDown()
        {
            // set to Eastern Time
            var timeUTC = DateTime.UtcNow;
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUTC, easternZone);

            // this is for the logger
            this.TestStepStatus.Name += $" attempt {this.Attempts}/{this.MaxAttempts}";

            // this is for the console write
            this.Name += $" attempt {this.Attempts}/{this.MaxAttempts}";

            this.TestStepStatus.EndTime = easternTime;
            this.TestStepStatus.RunSuccessful = !this.ShouldExecuteVariable || (this.TestStepStatus.RunSuccessful == this.PassCondition);

            // edited by Victor.
            // we want to only run the AODA report if the run was successful
            if (this.TestStepStatus.RunSuccessful && this.RunAODA)
            {
                // we know that this is from the perf xml version
                InformationObject.TestAutomationDriver.RunAODA(this.RunAODAPageName);
            }

            // This is where the majority of test steps will go through.
            else if (this.TestStepStatus.RunSuccessful && InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.RespectRunAODAFlag) == "true")
            {
                InformationObject.TestAutomationDriver.RunAODA(this.TestStepStatus.Description);
            }

            if (bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ReportToDevOps"]))
            {
                // record the test step results
                InformationObject.Reporter.RecordAzureTestStepResult(this.TestStepStatus.RunSuccessful, this.TestStepStatus.Actual);
            }

            if (this.ShouldLog)
            {
                ITestStepLogger log = new TestStepLogger();

                if (this.TestStepStatus.Actual == string.Empty)
                {
                    this.TestStepStatus.Actual = this.TestStepStatus.Expected;
                }

                // we will log afterwards
                log.Log(this);

                // for Azure
            }
            else
            {
                this.TestStepStatus.Actual = "No Log";
            }
        }
    }
}
