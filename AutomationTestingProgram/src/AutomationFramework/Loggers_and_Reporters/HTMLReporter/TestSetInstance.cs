// <copyright file="TestSetInstance.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using AutomationTestSetFramework;
    using Microsoft.Extensions.Azure;
    using Microsoft.VisualStudio.Services.Common;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// A class to represent a test case instance on ALM.
    /// </summary>
    public class TestSetHTMLInstance
    {
        /// <summary>
        /// Defines the EXEC_FINISHED.
        /// </summary>
        public static readonly int EXECFINISHED = 1;

        /// <summary>
        /// Defines the EXEC_PASSED.
        /// </summary>
        public static readonly int EXECPASSED = 2;

        /// <summary>
        /// The build number associated.
        /// </summary>
        public static string buildNumber { get; set; } = "My build number";

        /// <summary>
        /// The description for this test set.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// The description for this test set.
        /// </summary>
        public string Baseline { get; set; } = string.Empty;

        /// <summary>
        /// The description for this test set.
        /// </summary>
        public string TargetCycle { get; set; } = string.Empty;

        /// <summary>
        /// The location of the folder path. 
        /// </summary>
        public string FolderPath { get; set; } = string.Empty;

        /// <summary>
        /// The location of the folder path. 
        /// </summary>
        public string TestSetExecutionFormat { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the attachments. It has to be List of objects since if string, the backend will not take it... (it has to be object instead of string array).
        /// </summary>
        public List<object> Attachments { get; set; } = new List<object>();

        // this is for the test set details
        public Dictionary<string, string> TestSetDetails = new Dictionary<string, string>();

        /// <summary>
        /// Defines the CurrTestCase.
        /// </summary>
        private TestCaseHTML currTestCase;

        ///// <summary>
        ///// The username.
        ///// </summary>
        private string username;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSetInstance"/> class.
        /// </summary>
        /// <param name="username">The username of the alm</param>
        public TestSetHTMLInstance(string name,
                                   string username,
                                   string setId,
                                   string planId,
                                   string project,
                                   string orgName,
                                   DateTime startTime,
                                   DateTime endTime,
                                   string folderPath,
                                   string executionFormat,
                                   string planName,
                                   string application,
                                   string applicationCollection,
                                   string environment,
                                   string buildNumber,
                                   string testFrameworkVersion,
                                   string browser,
                                   string description,
                                   string testSetRunId,
                                   string globalAttempts="",
                                   string globalTimeout="",
                                   string executionURL="")
        {
            this.Name = name;
            this.ID = setId;
            this.PlanID = planId;
            this.username = username;
            this.Project = project;
            this.PlanName = planName;
            this.OrgName = orgName;
            this.FolderPath = folderPath;
            this.TestSetExecutionFormat = executionFormat;
            this.Description = description;
            this.Started = startTime;
            this.Finished = endTime;
            this.ExecutionURL = executionURL;
            this.RunID = testSetRunId;

            // set the test set details
            this.TestSetDetails.Add("Application", application);
            this.TestSetDetails.Add("Application Collection", applicationCollection);
            this.TestSetDetails.Add("Test Environment", environment);
            this.TestSetDetails.Add("Build Number", buildNumber);
            this.TestSetDetails.Add("Test Framework Version", testFrameworkVersion);
            this.TestSetDetails.Add("Browser", browser);
            this.TestSetDetails.Add("Global Attempts", globalAttempts);
            this.TestSetDetails.Add("Global TimeOut", globalTimeout);
        }

        /// <summary>
        /// Gets or sets the TestSet ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Test Set's Plan ID.
        /// </summary>
        public string PlanID { get; set; }

        /// <summary>
        /// Gets or sets the TestSet Run ID.
        /// </summary>
        public string RunID { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the EmailList.
        /// </summary>
        public string EmailList { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Project Name.
        /// </summary>
        public string PlanName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Project Name.
        /// </summary>
        public string Project { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Project Name.
        /// </summary>
        public string OrgName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the FailedEmailList.
        /// </summary>
        public string FailedEmailList { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start date time for this test set.
        /// </summary>
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets the finished date time for this test set.
        /// </summary>
        public DateTime Finished { get; set; }

        /// <summary>
        /// Gets or sets the total test cases the test set contains.
        /// </summary>
        public int TotalTestCases { get; set; } = 0;

        /// <summary>
        /// Gets the total number of blocked test cases.
        /// </summary>
        public int TotalBlocked => this.TestCaseExecutions.Count(tcExec => tcExec.Status == "Blocked");

        /// <summary>
        /// Gets the total number of failed test cases.
        /// </summary>
        public int TotalFailed => this.TestCaseExecutions.Count(tcExec => tcExec.Status == "Failed");

        /// <summary>
        /// Gets the total number of not available test cases.
        /// </summary>
        public int TotalNA => this.TestCaseExecutions.Count(tcExec => tcExec.Status == "N/A");

        /// <summary>
        /// Gets the total number of unrun test cases.
        /// </summary>
        public int TotalNoRun => this.TotalTestCases - this.TestCaseExecutions.Count;

        /// <summary>
        /// Gets the total number of Not Completed test cases.
        /// </summary>
        public int TotalNotCompleted => this.TestCaseExecutions.Count(tcExec => tcExec.Status == "Not Completed");

        /// <summary>
        /// Gets the total number of Passed test cases.
        /// </summary>
        public int TotalPassed => this.TestCaseExecutions.Count(tcExec => tcExec.Status == "Passed");

        /// <summary>
        /// Gets the total number of undelivered test cases.
        /// </summary>
        public int TotalUndelivered => this.TestCaseExecutions.Count(tcExec => tcExec.Status == "Undelivered");

        /// <summary>
        /// Gets or sets the TestCaseExecutions for this test set.
        /// </summary>
        public List<TestCaseHTML> TestCaseExecutions { get; set; } = new List<TestCaseHTML>();

        /// <summary>
        /// Gets or sets the URL corresponding to the DevOps execution.
        /// </summary>
        public string ExecutionURL { get; set; }

        /// <summary>
        /// Gets or sets the list of test step and the result.
        /// </summary>
        public List<List<string>> Steps { get; set; } = new List<List<string>>();

        /// <summary>
        /// The SetTestCaseRunStatus.
        /// </summary>
        public void SetUpTestCaseRunResult(bool hasScreenshot,
                                         string testCaseName,
                                         string runId,
                                         string runName,
                                         string testerName,
                                         DateTime startTime,
                                         int numSteps)
        {
            // increase the total number of test cases by 1
            this.TotalTestCases += 1;

            this.currTestCase = new TestCaseHTML()
            {
                Attachments = null,
                ExecDateTime = startTime,
                HasScreenShot = hasScreenshot,
                RunID = runId,
                RunName = runName,
                TestCaseName = testCaseName,
                TesterName = testerName,
                NumSteps = numSteps,
            };

            // reset the test steps list
            this.Steps = new List<List<string>>();
        }

        /// <summary>
        /// The test steps run statuses in the current test case.
        /// </summary>
        public void SetTestStepRunStatus(string result, string description)
        {
            // create a nested list with the first value indicating the result (pass or fail)
            List<string> newList = new List<string>();
            newList.Add(result);

            // second argument indicates the description of the test step to put in the HTML report
            newList.Add(description);

            this.Steps.Add(newList);
        }

        /// <summary>
        /// The test case run statuses.
        /// </summary>
        public void AddTestCaseRunStatus(string status)
        {
            this.currTestCase.Status = status;
            this.currTestCase.TestSteps = this.Steps;
            this.TestCaseExecutions.Add(this.currTestCase);
        }
    }
}
