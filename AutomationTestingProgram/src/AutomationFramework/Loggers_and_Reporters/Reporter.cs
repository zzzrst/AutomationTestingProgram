// <copyright file="Reporter.cs" company="PlaceholderCompany">
// Copyright (c) DDSB QA Regression. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomationTestSetFramework;
    using Microsoft.TeamFoundation.TestManagement.WebApi;
    using static AutomationTestingProgram.InformationObject;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The implementation of the reporter class.
    /// </summary>
    public class Reporter : IReporter
    {
        private AzureReporter.TestCaseInstance azureTestCase;
        private AzureReporter.TestPlanInstance azureTestRun;

        private string existingTestCaseID;

        private bool reportToDevOps;

        //private 

        /// <summary>
        /// The list of test case ids.
        /// </summary>
        private List<string> testCaseIdList = new List<string>();
        private int testCaseIdCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reporter"/> class.
        /// </summary>
        /// <param name="saveLocation">The location to save the file to.</param>
        public Reporter(string saveLocation)
        {
            this.SaveFileLocation = saveLocation;
            this.TestSetStatuses = new List<ITestSetStatus>();
            this.TestCaseStatuses = new List<ITestCaseStatus>();
            this.TestCaseToTestSteps = new Dictionary<ITestCaseStatus, List<ITestStepStatus>>();

            // whether or not to run the DevOps reporter
            this.reportToDevOps = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["ReportToDevOps"]);

            if (this.reportToDevOps)
            {
                string orgName = System.Configuration.ConfigurationManager.AppSettings["ORG_NAME"];
                string connectionUrl = "https://dev.azure.com/" + orgName;

                // set environment variable for AzurePAT
                string token;
                if (GetEnvironmentVariable(EnvVar.AzurePAT).ToString() == string.Empty)
                {
                    token = System.Configuration.ConfigurationManager.AppSettings["DevOpsPAT"];
                }
                else
                {
                    token = GetEnvironmentVariable(EnvVar.AzurePAT).ToString();
                }

                // getting the project name
                string project = InformationObject.TestProjectName;

                // testing whether or not we want to create a new test suite, or use the one already created
                // string testSuiteName = $"{InformationObject.TestSetName} {InformationObject.GetEnvironmentVariable(EnvVar.Environment)} {InformationObject.GetEnvironmentVariable(EnvVar.BuildNumber)} {DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}";
                string testSuiteName = $"{InformationObject.TestSetName} {InformationObject.GetEnvironmentVariable(EnvVar.Environment)} {InformationObject.GetEnvironmentVariable(EnvVar.BuildNumber)}".Trim(); // trim in case build number is empty string

                this.azureTestRun = new AzureReporter.TestPlanInstance(InformationObject.TestPlanName, testSuiteName, InformationObject.TestSetName, "Placeholder Test Plan description", connectionUrl, token, project);
                this.azureTestCase = new AzureReporter.TestCaseInstance(connectionUrl, token, project);
                Console.WriteLine("------------------------------ Created Azure Test Plan and Test Case Instance  -------------------");
            }
            else
            {
                Logger.Info("Not reporting to DevOps. ReportToDevOps set to FALSE");
            }
        }

        /// <summary>
        /// Gets or sets the location to save the report to.
        /// </summary>
        public string SaveFileLocation { get; set; }

        /// <summary>
        /// Gets or sets the list of test set statuses.
        /// </summary>
        public List<ITestSetStatus> TestSetStatuses { get; set; }

        /// <summary>
        /// Gets or sets the list of test case statuses.
        /// </summary>
        public List<ITestCaseStatus> TestCaseStatuses { get; set; }

        /// <summary>
        /// Gets or sets the dictonary with key as Test Cases status and value as a list of Test step statuses.
        /// </summary>
        public Dictionary<ITestCaseStatus, List<ITestStepStatus>> TestCaseToTestSteps { get; set; }

        /// <summary>
        /// Creates an initial test case list and returns the newly created test case id
        /// </summary>
        /// <param name="testCase">Created intiial test case and return the result. <see cref="string"/>.</param>
        /// <returns>string value indicating indicating the test case id created</returns>
        public string CreateInitialTestCase(string testCase)
        {
            if (this.reportToDevOps)
            {

                string newTestCaseId;

                // newTestCaseId = this.azureTestCase.QueryForTestCase(testCase);
                newTestCaseId = this.azureTestCase.QueryForTestCaseFaster(testCase);

                if (newTestCaseId == string.Empty)
                {
                    this.azureTestCase.CreateTestCaseUsingClientLib(testCase, 1, "Automated Test generated by Testing Automation Program");
                    newTestCaseId = "placeholder";
                }
                else
                {
                    // reset test case by setting values to original
                    this.azureTestCase.CreateTestCaseReference();
                }

                //// as we are iterating through the test case list, add the test case to the test case list. We should not add them all at once because the "skip" will does not work. 
                this.azureTestCase.testCaseList.Add(testCase);

                return newTestCaseId;
            }
            else
            {
                // ERROR if we reach here
                Logger.Error("Should not be reporting to devops for create initial test case");
                return string.Empty;
            }
        }

        /// <summary>
        /// Sets the test case list and used for creating test steps to be added to the test case.
        /// </summary>
        /// <param name="testCases">Test cases list. <see cref="List{T}"/>.</param>
        public void SetTestCaseList(List<string> testCases)
        {
            if (this.reportToDevOps)
            {
                this.azureTestRun.testCaseList = testCases;

                this.azureTestRun.AddTestCasesToSuite(this.testCaseIdList);

                Console.WriteLine("------------------------------ Successfully Set Test Case List -------------------");

                this.azureTestRun.CreateTestRun($"Test Run of {InformationObject.TestSetName} {InformationObject.GetEnvironmentVariable(EnvVar.Environment)} at {DateTime.Now}");
            }
        }

        /// <summary>
        /// Add test run attachments.
        /// </summary>
        /// <param name="comment">Comment for the test run attachment. <see cref="string"/>.</param>
        /// <param name="filePath">Filepath of the attachment. <see cref="string"/>.</param>
        /// <param name="fileName">FileName of the attachment. <see cref="string"/>.</param>
        public void AddTestRunAttachment(string comment, string filePath, string fileName)
        {
            // if we intend to report to devops
            if (this.reportToDevOps)
            {
                this.azureTestRun.AddTestRunAttachment(comment, filePath, fileName);
            }
        }

        // since this is already created, we only need to create a test case reference and link to the existing test case id
        public virtual void CreateAzureTestCase(string testCaseName, string description)
        {
            // if we intend to report to devops
            if (this.reportToDevOps)
            {

                // go through test case ids
                if (this.testCaseIdCount >= this.testCaseIdList.Count)
                {
                    Console.WriteLine("ERROR: testcase id count greater than list");
                    return;
                }

                this.existingTestCaseID = this.testCaseIdList[this.testCaseIdCount];
                this.testCaseIdCount++;

                // reset test case by setting values to original, test cases are already created.
                this.azureTestCase.CreateTestCaseReference();

                // create a test case for the suite
                this.azureTestRun.CreateTestCaseResult();
                Console.WriteLine($"------------------------------ Created Test Case Result for ID: {this.existingTestCaseID} -------------------");
            }
        }

        /// <summary>
        /// Add a test attachment to a test case.
        /// </summary>
        /// <param name="fileName">The file name of the attachment to add, will be used as the name of the file on DevOps<see cref="string"/>.</param>
        /// <param name="filePath">The file path of the attachment to add, will be used as the name of the file on DevOps<see cref="string"/>.</param>
        /// <param name="comment">The comment for the test attachment <see cref="string"/>.</param>
        public void AddTestAttachment(string fileName, string filePath, string comment)
        {
            // only add if we are intending to report to DevOps
            if (this.reportToDevOps)
            {
                this.azureTestRun.AddTestCaseAttachment(comment, filePath, fileName);
                Console.WriteLine("------------------------------ Added Test Case Attachment -------------------");
            }
        }

        /// <summary>
        /// Add a test attachment to a test case.
        /// </summary>
        /// <param name="fileName">The file name of the attachment to add, will be used as the name of the file on DevOps<see cref="string"/>.</param>
        /// <param name="filePath">The file path of the attachment to add, will be used as the name of the file on DevOps<see cref="string"/>.</param>
        /// <param name="comment">The comment for the test attachment <see cref="string"/>.</param>
        public void AddTestStepScreenshot(string fileName, string filePath, string comment)
        {
            // only add if we are intending to report to DevOps
            if (this.reportToDevOps)
            {
                this.azureTestRun.AddTestStepAttachment(comment, filePath, fileName, this.existingTestCaseID);
                Console.WriteLine("------------------------------ Added Test Case Attachment -------------------");
            }
        }

        /// <summary>
        /// Create a azure test step.
        /// </summary>
        /// <param name="testStepName">The name of the test step to be displayed on DevOps<see cref="string"/>.</param>
        /// <param name="expected">the expected result of the test step<see cref="string"/>.</param>
        /// <param name="testStepDescription">The description of the test step. <see cref="string"/>.</param>
        public void CreateAzureTestStep(string testStepName, string expected, string testStepDescription)
        {
            this.azureTestCase.CreateTestStep(testStepName, expected, testStepDescription);
        }

        /// <summary>
        /// Saves the test steps to the testCaseId.
        /// </summary>
        /// <param name="testCaseId">the test case id that we will save to. <see cref="string"/>.</param>
        /// <returns>String value indicating the test case id that we saved the test case to.</returns>
        public string SaveTestSteps(string testCaseId)
        {
            string newTestCaseId = this.azureTestCase.SaveTestCaseSteps(testCaseId);

            // add new test case id to the list
            this.testCaseIdList.Add(newTestCaseId);

            return newTestCaseId;
        }

        /// <summary>
        /// Records azure test step result.
        /// </summary>
        /// <param name="runsuccessful">Whether the test step was successfully executed. <see cref="string"/>.</param>
        /// <param name="testStepName">The test step name<see cref="string"/>.</param>
        /// <param name="expected">The expected result of the test step<see cref="string"/>.</param>
        /// <param name="actual">The actual result of the test step. <see cref="string"/>.</param>
        /// <param name="testStepDescription">The description of the test step. <see cref="string"/>.</param>
        public void RecordAzureTestStepResult(bool runsuccessful, string actual)
        {
            string success = runsuccessful ? "Passed" : "Failed";

            // check if we want to report to DevOps
            if (this.reportToDevOps)
            {
                this.azureTestRun.RecordTestStepResult(success, actual);
                Console.WriteLine("------------------------------ Added Test Step Result to Test Case -------------------");
            }
        }

        /// <summary>
        /// Add test case status to the execution.
        /// </summary>
        /// <param name="testCaseStatus">Added test case status information to the test case. <see cref="string"/>.</param>
        public virtual void AddTestCaseStatus(ITestCaseStatus testCaseStatus)
        {
            this.TestCaseStatuses.Add(testCaseStatus);

            // check if we want to report to DevOps
            if (this.reportToDevOps)
            {

                if (this.existingTestCaseID != null)
                {
                    string success = testCaseStatus.RunSuccessful ? "Passed" : "Failed";

                    // if actual is blocked, then report Blocked
                    if (testCaseStatus.Actual == "Blocked")
                    {
                        success = "Blocked";
                    }

                    this.azureTestRun.RecordTestCaseResult(success, this.existingTestCaseID, testCaseStatus);

                    Console.WriteLine($"------------------------------ Wrote Test Case Result: {this.existingTestCaseID} -------------------");
                }
            }
        }

        /// <inheritdoc/>
        public virtual void AddTestSetStatus(ITestSetStatus testSetStatus)
        {
            this.TestSetStatuses.Add(testSetStatus);

            Console.WriteLine("------------------------------ Added Test Set Status Run -------------------");
        }

        /// <inheritdoc/>
        public virtual void AddTestStepStatusToTestCase(ITestStepStatus testStepStatus, ITestCaseStatus testCaseStatus)
        {
            if (!this.TestCaseToTestSteps.ContainsKey(testCaseStatus))
            {
                this.TestCaseToTestSteps.Add(testCaseStatus, new List<ITestStepStatus>());
            }

            this.TestCaseToTestSteps[testCaseStatus].Add(testStepStatus);
        }

        /// <inheritdoc/>
        [RequiresAssemblyFiles()]
        public virtual void Report()
        {
            // check if we want to generate an HTML report
            if (System.Configuration.ConfigurationManager.AppSettings["GenerateHTMLReport"] == "true")
            {
                // go through each test set file
                foreach (AutomationTestingProgram.AutomationFramework.TestSetStatus testSet in this.TestSetStatuses)
                {
                    TestSetHTMLInstance newTestSet = new TestSetHTMLInstance(
                        name: InformationObject.TestSetName,
                        username: this.azureTestRun?.GetTestRun()?.Owner?.DisplayName ?? "Local Executer",
                        setId: this.azureTestRun?.GetTestSuiteId() ?? "LOCAL",
                        planId: this.azureTestRun?.GetTestPlanId() ?? "LOCAL",
                        project: InformationObject.TestProjectName,
                        orgName: System.Configuration.ConfigurationManager.AppSettings["ORG_NAME"],
                        startTime: testSet.StartTime,
                        endTime: testSet.EndTime,
                        folderPath: InformationObject.GetEnvironmentVariable(EnvVar.TestSetDataArgs),
                        executionFormat: InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.TestSetDataType),
                        planName: InformationObject.TestPlanName,
                        application: "Placeholder application",
                        applicationCollection: "Placeholder application collection",
                        environment: InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.Environment),
                        buildNumber: InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.BuildNumber),
                        testFrameworkVersion: FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion,
                        browser: InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.Browser),
                        description: testSet.Description,
                        testSetRunId: this.azureTestRun?.GetTestRun()?.Id.ToString() ?? "LOCAL",
                        globalAttempts: System.Configuration.ConfigurationManager.AppSettings["ExcelLocalAttempts"],
                        globalTimeout: System.Configuration.ConfigurationManager.AppSettings["TimeOutThreshold"],
                        executionURL: InformationObject.ExecutionURL
                        );

                    // populate the test case information for the test set for HTML reporter
                    int startVal = 0;
                    foreach (AutomationFramework.TestCaseStatus testCase in this.TestCaseStatuses)
                    {
                        string result = testCase.RunSuccessful.ToString() == "True" ? "Passed" : "Failed";
                        if (testCase.Actual == "Blocked")
                        {
                            result = "Blocked";
                        }

                        // runId is mapped by using the start val and test case mapping combination.
                        newTestSet.SetUpTestCaseRunResult(
                            hasScreenshot: testCase.RunSuccessful, // if run unsuccessful, then screenshot
                            testCaseName: testCase.Name,
                            runId: this.azureTestRun?.TestCaseMapping[int.Parse(this.testCaseIdList[startVal])].ToString() ?? "LOCAL",
                            runName: testCase.Name,
                            testerName: InformationObject.TesterName + " " + InformationObject.TesterEmail,
                            startTime: testCase.StartTime,
                            numSteps: this.TestCaseToTestSteps[testCase].Count());

                        // iterate through the test step statuses for the test case and populate the test step run status
                        string runStepRes;
                        string description;
                        string concatRes;
                        for (int stepNum = 0; stepNum < this.TestCaseToTestSteps[testCase].Count(); stepNum++)
                        {
                            // if run successful, then runStepRes is Passed
                            if (this.TestCaseToTestSteps[testCase][stepNum].RunSuccessful == true)
                            {
                                runStepRes = "Passed";
                            }
                            else
                            {
                                runStepRes = "Failed";
                            }

                            description = this.TestCaseToTestSteps[testCase][stepNum].Description;

                            newTestSet.SetTestStepRunStatus(runStepRes, description);
                        }

                        newTestSet.AddTestCaseRunStatus(result);

                        // increase the start val by 1, as the test cases should already be in order
                        startVal += 1;
                    }

                    // generate a test set execution summary report
                    TestSetExecutionSummaryReport tsExecSummaryReport = new TestSetExecutionSummaryReport()
                    {
                        TestSet = newTestSet,
                    };

                    string htmlReport = tsExecSummaryReport.GenerateHTMLReport(newTestSet.TestCaseExecutions,
                                                                                newTestSet.TestSetDetails);
                    // get executing directory
                    string fileName = "execution_report.html";
                    string path_ex = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    path_ex += "\\" + InformationObject.GetEnvironmentVariable(EnvVar.CsvSaveFileLocation) + "\\" + fileName;

                    // write html report to the file location
                    File.WriteAllText(path_ex, htmlReport);

                    // create a list for the emails
                    List<string> emails;
                    emails = InformationObject.NotifyEmails;

                    // if there are no emails specified
                    if (emails == null)
                    {
                        Logger.Info("Emails from command line do not exist, taking emails from App.Config");

                        // if the EMAILS LIST in App.Config is not empty
                        if (System.Configuration.ConfigurationManager.AppSettings["EMAILS_LIST"] != string.Empty)
                        {
                            emails = System.Configuration.ConfigurationManager.AppSettings["EMAILS_LIST"]?.Split(",").ToList();
                        }
                    }

                    // if report to devops true and more than one person to send to, then attach the report
                    // send emails even if not reporting to dveops
                    if (emails != null && emails.Count() > 0) {
                        // if the emails after joining are single string, then fail
                        if (string.Join(",", emails) != string.Empty)
                        {
                            Logger.Info("Sending emails to: " + string.Join(",", emails));
                            RunMailJet.CreateEmail(htmlReport, emails);
                        }
                        else
                        {
                            Logger.Info("Not sending email to anyone");
                        }
                    }

                    // always attach the report pdf
                    if (this.reportToDevOps)
                    {
                        this.azureTestRun.AddTestRunAttachment("PDF execution report", path_ex, fileName);
                    }
                }
            }

            List<string> str = new List<string>();

            str.Add("Running Excel Execution"); // {InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.TestSetDataArgs)}");

            // {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            foreach (InformationObject.EnvVar var in Enum.GetValues(typeof(InformationObject.EnvVar)))
            {
                str.Add($"{var} = {InformationObject.GetEnvironmentVariable(var)}");
            }

            str.Add("-----------------------");
            foreach (ITestSetStatus testSetStatus in this.TestSetStatuses)
            {
                str.Add("Name: " + testSetStatus.Name);
                str.Add("RunSuccessful:" + testSetStatus.RunSuccessful.ToString());
                str.Add("StartTime:" + testSetStatus.StartTime.ToString());
                str.Add("EndTime:" + testSetStatus.EndTime.ToString());
                str.Add("Description:" + testSetStatus.Description);
                str.Add("ErrorStack:" + testSetStatus.ErrorStack);
                str.Add("FriendlyErrorMessage:" + testSetStatus.FriendlyErrorMessage);
                str.Add("Expected:" + testSetStatus.Expected);
                str.Add("Actual:" + testSetStatus.Actual);
            }

            foreach (ITestCaseStatus testCaseStatus in this.TestCaseStatuses)
            {
                str.Add(this.Tab(1) + "Name:" + testCaseStatus.Name);
                str.Add(this.Tab(1) + "TestCaseNumber:" + testCaseStatus.TestCaseNumber.ToString());
                str.Add(this.Tab(1) + "RunSuccessful:" + testCaseStatus.RunSuccessful.ToString());
                str.Add(this.Tab(1) + "StartTime:" + testCaseStatus.StartTime.ToString());
                str.Add(this.Tab(1) + "EndTime:" + testCaseStatus.EndTime.ToString());
                str.Add(this.Tab(1) + "Description:" + testCaseStatus.Description);
                str.Add(this.Tab(1) + "ErrorStack:" + testCaseStatus.ErrorStack);
                str.Add(this.Tab(1) + "FriendlyErrorMessage:" + testCaseStatus.FriendlyErrorMessage);
                str.Add(this.Tab(1) + "Expected:" + testCaseStatus.Expected);
                str.Add(this.Tab(1) + "Actual:" + testCaseStatus.Actual);

                if (this.TestCaseToTestSteps.ContainsKey(testCaseStatus))
                {
                    // log the test steps.
                    foreach (ITestStepStatus testStepStatus in this.TestCaseToTestSteps[testCaseStatus])
                    {
                        str.Add(this.Tab(2) + "Name:" + testStepStatus.Name);
                        str.Add(this.Tab(2) + "TestStepNumber:" + testStepStatus.TestStepNumber.ToString());
                        str.Add(this.Tab(2) + "RunSuccessful:" + testStepStatus.RunSuccessful.ToString());
                        str.Add(this.Tab(2) + "StartTime:" + testStepStatus.StartTime.ToString());
                        str.Add(this.Tab(2) + "EndTime:" + testStepStatus.EndTime.ToString());
                        str.Add(this.Tab(2) + "Description:" + testStepStatus.Description);
                        str.Add(this.Tab(2) + "ErrorStack:" + testStepStatus.ErrorStack);
                        str.Add(this.Tab(2) + "FriendlyErrorMessage:" + testStepStatus.FriendlyErrorMessage);
                        str.Add(this.Tab(2) + "Expected:" + testStepStatus.Expected);
                        str.Add(this.Tab(2) + "Actual:" + testStepStatus.Actual);
                        str.Add(this.Tab(2) + "-----------------------");
                    }
                }
            }

            str.Add(string.Empty);
            using (StreamWriter file =
            new StreamWriter($"{this.SaveFileLocation}", true))
            {
                foreach (string line in str)
                {
                    file.WriteLine(line);
                }
            }

            if (this.reportToDevOps)
            {
                // excel execution run
                // add the log file just created to the test run
                string fileName = this.SaveFileLocation.Substring(this.SaveFileLocation.LastIndexOf("\\") + 1);

                this.azureTestRun.AddTestRunAttachment("Run results txt", this.SaveFileLocation, fileName);

                // get the console log information and publish results to DevOps. Because this file may be in use, we need to make a copy of it before attaching. 
                string fileLogLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string logLocation = fileLogLocation + "/SeleniumExecution.log";
                string tempLocation = fileLogLocation + "/TEMP_SeleniumLog.log";

                // if the temp file location currently has a file, then delete it. 
                if (File.Exists(tempLocation))
                {
                    File.Delete(tempLocation);
                }

                File.Copy(logLocation, tempLocation);
                this.azureTestRun.AddTestRunAttachment("Console Logger Contents", tempLocation, "SeleniumExecution.log");

                // file location
                this.azureTestRun.AddTestRunAttachment("Execution excel xlsx file", InformationObject.GetEnvironmentVariable(EnvVar.TestSetDataArgs), $"executed_file_{InformationObject.TestSetName}.xlsx");

                // record the test run
                this.azureTestRun.RecordTestRun();
                this.azureTestRun.PrintBasicRunInfo();
            }
        }

        public void ReportAborted()
        {
            if (this.reportToDevOps)
            {
                RunUpdateModel runmodel = new RunUpdateModel(
                                state: "Aborted",
                                completedDate: DateTime.UtcNow.ToString(),
                                errorMessage: "User aborted",
                                comment: "SIGINT signal received"
                                );
                this.azureTestRun.RecordTestRun(runmodel);
            }
        }

        private string Tab(int indents = 1)
        {
            return string.Concat(Enumerable.Repeat("    ", indents));
        }
    }
}
