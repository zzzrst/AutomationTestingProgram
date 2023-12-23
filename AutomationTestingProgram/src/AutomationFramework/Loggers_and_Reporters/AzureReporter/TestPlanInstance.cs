
namespace AzureReporter
{

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing.Printing;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Net.NetworkInformation;
    using System.Numerics;
    using System.Text;
    using System.Text.RegularExpressions;
    using AutomationTestingProgram;
    using AutomationTestSetFramework;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Office.Client.TranslationServices;
    using Microsoft.ProjectServer.Client;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.TeamFoundation.Test.WebApi;
    using Microsoft.TeamFoundation.TestManagement.WebApi;
    using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
    using Microsoft.TeamFoundation.Work.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;
    using Newtonsoft.Json;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// Connects to the Test Plan, reports results to devops, and reports test run results.
    /// </summary>
    public class TestPlanInstance
    {
        // initialize uri, pat, and project variables
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        // initialize a test plan private variable
        private Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan testPlan;

        // current test suite id
        private string testSuite;

        private DateTime startedTime;

        private DateTime testCaseST;

        private DateTime testStepST;

        private List<TestCaseResult> testCaseResults;

        // two lists, one for storing the attachment, one for storing the test step assigned to it
        private List<TestAttachmentRequestModel> testCaseResultAttachmentRefs;
        private List<int> testCaseResultAttachmentIds;

        private List<TestAttachmentRequestModel> testRunAttachmentRefs;

        /// <summary>
        /// The list of test cases. Index 0 is the first test case id.
        /// </summary>
        public List<string> testCaseList;

        /// <summary>
        /// Mapping for the test case id to the test case's order id.
        /// </summary>
        public Dictionary<int, int> TestCaseMapping = new Dictionary<int, int>();

        /// <summary>
        /// Mapping for the test case id to the test case's order id.
        /// </summary>
        private List<int> OrderedTestResult = new List<int>();

        /// <summary>
        /// The current step id of the test case.
        /// </summary>
        private int testCaseCounter = 0;

        /// <summary>
        /// The current step id of the test case.
        /// </summary>
        private int stepIdCounter = 1;

        /// <summary>
        /// The current test run.
        /// </summary>
        private TestRun testRun;

        /// <summary>
        /// The current iteration of the test case.
        /// </summary>
        private TestIterationDetailsModel iteration;

        /// <summary>
        /// The connection to the azure devops client.
        /// </summary>
        private VssConnection connection;

        /// <summary>
        /// The credentials for the connection to the azure devops client.
        /// </summary>
        private VssBasicCredential credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPlanInstance"/> class.
        /// Generates a Test Plan on Azure DevOps.
        /// </summary>
        /// <param name="pat">Personal access token to the Azure DevOps project. PAT must have access to the project. <see cref="string"/>.</param>
        /// <param name="url">URL to the Azure DevOps project<see cref="string"/>.</param>
        /// <param name="testPlanDescription">the test plan description that will be visible on DevOps. <see cref="string"/>.</param>
        /// <param name="project">The Azure DevOps project we are using. <see cref="string"/>.</param>
        /// <param name="testSuiteParentName">The test suite parent suite name on DevOps<see cref="string"/>.</param>
        /// <param name="testSuiteName">The test suite name for execution<see cref="string"/>.</param>
        /// <param name="testPlanName">The plan name to create on DevOps<see cref="string"/>.</param>
        public TestPlanInstance(string testPlanName, string testSuiteName, string testSuiteParentName, string testPlanDescription, string url, string pat, string project)
        {
            this._uri = url;
            this._personalAccessToken = pat;
            this._project = project;

            Uri uri = new Uri(this._uri);

            // generate initial credentials
            this.credentials = new VssBasicCredential("", this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);

            // variable for the entire test set for each test case result
            this.testCaseResults = new List<TestCaseResult>();

            // reset the test case result attachments
            this.testCaseResultAttachmentRefs = new List<TestAttachmentRequestModel>();
            this.testCaseResultAttachmentIds = new List<int>();
            this.testRunAttachmentRefs = new List<TestAttachmentRequestModel>();

            if (InformationObject.GetEnvironmentVariable(InformationObject.EnvVar.TestSetDataType) == "Excel") { 
                // query Azure to determine if Test Plan already exists before creating one.
                try
                {
                    string existingTestPlan = this.CheckForTestPlan(testPlanName);
                    if (existingTestPlan != string.Empty)
                    {
                        this.GetTestPlanByID(existingTestPlan);
                    }
                    else
                    {
                        this.CreateTestPlan(testPlanName, testPlanDescription);
                    }

                    string suiteId = string.Empty;
                    string rootFolder;
                    string newId;

                    // if the folder structure is not empty, then we will iterate through and create the 
                    // appropriate number of folders
                    if (InformationObject.FolderStructure != string.Empty)
                    {
                        // replace all forward slashes with back slashes
                        string folderStructureStr = InformationObject.FolderStructure.Replace('/', '\\');

                        // crewate a list of all folders separated by slashes
                        List<string> folderStructure = folderStructureStr.Split('\\').ToList();

                        string currSuite = this.testPlan.RootSuite.Id;

                        foreach (string folder in folderStructure)
                        {
                            suiteId = this.CheckForTestSuite(folder, int.Parse(currSuite));

                            // we should see if the parent folder already exists, and if so, don't create another one
                            if (suiteId == string.Empty)
                            {
                                // create a test suite only if the test plan cannot find the test suite parent id
                                suiteId = this.CreateTestSuite(folder, currSuite);
                            }

                            currSuite = suiteId; // assign current suite to suiteId just created
                        }

                        // after all subfoldders are created, then continue
                        // here create a test suite twice because we want to use the parent folder and the non parent folder. 
                        newId = this.CheckForTestSuite(testSuiteParentName, int.Parse(currSuite));

                        // we should see if the parent folder already exists, and if so, don't create another one
                        if (newId == string.Empty)
                        {
                            // create a test suite only if the test plan cannot find the test suite parent id
                            newId = this.CreateTestSuite(testSuiteParentName, currSuite);
                        }
                    }

                    // if the config value create smoke or regression folder is set to true, then we will create a smoke or regression folder
                    else if (System.Configuration.ConfigurationManager.AppSettings["CREATE_SMOKE_REGRESSION_FOLDER"].ToUpper() == "TRUE")
                        {
                            // if the parent name contains smoke, then we will use the smoke folder
                            if (testSuiteParentName.ToUpper().Contains("SMOKE"))
                        {
                            rootFolder = "SMOKE";
                        }

                        // if the parent name contains regress, then we will use the regress folder
                        else if (testSuiteParentName.ToUpper().Contains("REGRESSION"))
                        {
                            rootFolder = "REGRESSION";
                        }

                        // if it's neither, we will use the MISC folder
                        else
                        {
                            rootFolder = "MISC";
                        }

                        suiteId = this.CheckForTestSuite(rootFolder, int.Parse(this.testPlan.RootSuite.Id));

                        // we should see if the parent folder already exists, and if so, don't create another one
                        if (suiteId == string.Empty)
                        {
                            // create a test suite only if the test plan cannot find the test suite parent id
                            suiteId = this.CreateTestSuite(rootFolder, this.testPlan.RootSuite.Id);
                        }

                        // here create a test suite twice because we want to use the parent folder and the non parent folder. 
                        newId = this.CheckForTestSuite(testSuiteParentName, int.Parse(suiteId));

                        // we should see if the parent folder already exists, and if so, don't create another one
                        if (newId == string.Empty)
                        {
                            // create a test suite only if the test plan cannot find the test suite parent id
                            newId = this.CreateTestSuite(testSuiteParentName, suiteId);
                        }
                    }

                    // otherwise, simply create it without the SMOKE or REGRESSION folders
                    else
                    {
                        // here create a test suite twice because we want to use the parent folder and the non parent folder. 
                        newId = this.CheckForTestSuite(testSuiteParentName, int.Parse(this.testPlan.RootSuite.Id));

                        // we should see if the parent folder already exists, and if so, don't create another one
                        if (newId == string.Empty)
                        {
                            // create a test suite only if the test plan cannot find the test suite parent id
                            newId = this.CreateTestSuite(testSuiteParentName, this.testPlan.RootSuite.Id);
                        }
                    }

                    string newVal;

                    // checking if we have set create new test suite to true or false
                    if (System.Configuration.ConfigurationManager.AppSettings["CREATE_NEW_TEST_SUITE"] == "false")
                    {
                        newVal = this.CheckTestSuiteChildren(testSuiteName, newId);

                        if (newVal != string.Empty)
                        {
                            Logger.Info("Test Suite already exists, using that one.");
                        }
                        else
                        {
                            Logger.Info("Test Suite does not already exist, creating new one.");

                            // string parentTestSuite = this.CheckForTestSuite(testSuiteParentName);
                            newVal = this.CreateTestSuite(testSuiteName, newId);
                        }
                    }
                    else
                    {
                        newVal = this.CreateTestSuite(testSuiteName, newId);
                    }

                    this.testSuite = newVal;

                    Logger.Info("------------------------------ Created Test Plan -------------------");
                }
                catch (Exception ex)
                {
                    Logger.Info("Exception caught in initializing Test Plan Instance: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Returns the current test run.
        /// </summary>
        /// <returns>Test Run of the current test run.</returns>
        public TestRun GetTestRun()
        {
            return this.testRun;
        }

        /// <summary>
        /// Returns the current test plan id.
        /// </summary>
        /// <returns>ID of the current test plan.</returns>
        public string GetTestPlanId()
        {
            return this.testPlan.Id.ToString();
        }

        /// <summary>
        /// Returns the current test suite id.
        /// </summary>
        /// <returns>ID of the current test suite.</returns>
        public string GetTestSuiteId()
        {
            return this.testSuite;
        }

        /// <summary>
        /// Initiatlize Test Set.
        /// Takes in a string for the Test Set Name.
        /// </summary>
        /// <param name="testPlanName">The azure test plan name. <see cref="string"/>.</param>
        public void CreateTestRun(string testPlanName)
        {
            try
            {
                Uri uri = new Uri(this._uri);
                this.credentials = new VssBasicCredential("", this._personalAccessToken);
                this.connection = new VssConnection(uri, this.credentials);

                // create test management client
                TestManagementHttpClient testMngmnt = this.connection.GetClient<TestManagementHttpClient>();

                // create a shaddow reference to the test plan
                Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference testPlanRef = new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference(this.testPlan.Id.ToString());

                // initialize test points list
                List<int> pointIds = new List<int>();

                // wait for two seconds to ensure that DevOps is populated
                System.Threading.Thread.Sleep(2000);

                // get test points in test plan under first test suite in the plan
                List<Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint> testPoints = testMngmnt.GetPointsAsync(this._project, this.testPlan.Id, int.Parse(this.testSuite)).Result;

                // get a list of test plan ids from the test plan
                // note that test points are sorted prior to execution
                // add key value mapping to the test point and result value
                int resultIdVal = 100000;
                foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint testPoint in testPoints)
                {
                    Logger.Info("------------------------------ Successfully Added Test Point -------------------" + testPoint.Id);
                    pointIds.Add(testPoint.Id);

                    // here we need to map the case with the test point so that it can be sorted
                    this.TestCaseMapping.Add(int.Parse(testPoint.TestCase.Id), resultIdVal);
                    resultIdVal += 1; // increase by 1
                }

                int[] listArray = pointIds.ToArray();

                // take utc time
                string startTime = DateTime.UtcNow.ToString();
                Logger.Info("Start time of execution: " + startTime);

                // tester
                IdentityRef identityRefSearch = new IdentityRef();
                identityRefSearch.DisplayName = InformationObject.TesterName;

                string releaseUri, releaseEnvironmentUri;
                if (InformationObject.ReleaseEnvUri != string.Empty)
                {
                    releaseUri = InformationObject.ReleaseEnvUri.Split(',')[0].Trim();
                    releaseEnvironmentUri = InformationObject.ReleaseEnvUri.Split(',')[1].Trim();
                }
                else
                {
                    releaseUri = string.Empty;
                    releaseEnvironmentUri = string.Empty;
                }

                // create a run model
                RunCreateModel testRunModel = new RunCreateModel(
                      name: "Run for " + testPlanName,
                      pointIds: listArray,
                      plan: testPlanRef,
                      isAutomated: true,
                      startedDate: startTime,
                      releaseUri: releaseUri,
                      releaseEnvironmentUri: releaseEnvironmentUri,
                      owner: identityRefSearch
                );

                // create a test run
                this.testRun = testMngmnt.CreateTestRunAsync(testRunModel, this._project).Result;

                // Record where the test plan has been created to
                string testRunLink = $"https://dev.azure.com/csc-ddsb/{this._project}/_testManagement/runs?_a=runCharts&runId={this.testRun.Id}";
                Logger.Info("Test Run Link: " + testRunLink);
            }
            catch (Exception e)
            {
                Logger.Info("Exception caught in creating test run: " + e.Message);
            }

            Logger.Info("------------------------------Test Run Successfully Created and Sent-------------------");
        }

        /// <summary>
        /// Record the test run result by calling UpdateTestRunAsync.
        /// </summary>
        /// <param name="runModel">If runmodel is already created, use that.</param>
        public void RecordTestRun(RunUpdateModel runModel = null)
        {
            Uri uri = new Uri(this._uri);
            this.credentials = new VssBasicCredential("", this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);

            // create test management client
            TestManagementHttpClient testMngmnt = this.connection.GetClient<TestManagementHttpClient>();

            if (runModel == null)
            {
                runModel = new RunUpdateModel(
                                state: "Completed",
                                completedDate: DateTime.UtcNow.ToString(),
                                errorMessage: "Placeholder run error message",
                                comment: "Placeholder run comment message"
                                );
            }

            // Check what is null
            if (testMngmnt == null)
            {
                Logger.Info("Test Management is null");
            }

            if (this._project == null)
            {
                Logger.Info("Project is null");
            }

            if (this.testRun == null)
            {
                Logger.Info("Test Run is null");
                this.CreateTestRun($"Test Run of {InformationObject.TestSetName} {InformationObject.GetEnvironmentVariable(EnvVar.Environment)} at {DateTime.Now}");
                Logger.Info("Test Run id is " + this.testRun?.Id);
            }

            // save test case results to run model
            this.testRun = testMngmnt.UpdateTestRunAsync(runModel, this._project, this.testRun.Id).Result;
        }

        /// <summary>
        /// Add test run attachment to the test run.
        /// </summary>
        /// <param name="comment">The comment <see cref="string"/>.</param>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>
        /// <param name="fileName">The file name of the attachment to add, will be used as the name of the file on DevOps<see cref="string"/>.</param>
        /// https://stackoverflow.com/questions/64160502/upload-test-result-documents-to-azure-test-plans-or-the-test-hub-in-tfs-2018
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.teamfoundation.testmanagement.webapi.testhttpclientbase.createtestresultattachmentasync?view=azure-devops-dotnet
        public void AddTestRunAttachment(string comment, string filePath, string fileName)
        {
            Uri uri = new Uri(this._uri);

            this.credentials = new VssBasicCredential(string.Empty, this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);

            // we need to encode the string as base 64
            byte[] bytes = File.ReadAllBytes(filePath);
            string file64 = Convert.ToBase64String(bytes);

            TestAttachmentRequestModel testAtt = new TestAttachmentRequestModel()
            {
                Comment = comment,
                FileName = fileName,
                Stream = file64,
            };
            TestManagementHttpClient testMngmnt = this.connection.GetClient<TestManagementHttpClient>();
            var res = testMngmnt.CreateTestRunAttachmentAsync(testAtt, this._project, this.testRun.Id).Result;

            Logger.Info("------------------ Added attachment : " + testAtt.FileName);
        }

        /// <summary>
        /// The AddTestCaseAttachment.
        /// </summary>
        /// <param name="comment">The comment <see cref="string"/>.</param>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>
        /// <param name="fileName">The file name<see cref="string"/>.</param>
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.teamfoundation.testmanagement.webapi.testhttpclientbase.createtestresultattachmentasync?view=azure-devops-dotnet
        public void AddTestCaseAttachment(string comment, string filePath, string fileName)
        {
            Uri uri = new Uri(this._uri);

            this.credentials = new VssBasicCredential("", this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);

            if (!File.Exists(filePath))
            {
                Logger.Info($"Could not read file at path {filePath} because did not exist");
                return;
            }

            // we need to encode the string as base 64
            byte[] bytes = File.ReadAllBytes(filePath);

            string fileNameErrorScreenshot = filePath.Substring(filePath.LastIndexOf("\\") + 1);

            string file64 = Convert.ToBase64String(bytes);

            TestAttachmentRequestModel testAtt = new TestAttachmentRequestModel()
            {
                Comment = comment,
                FileName = fileNameErrorScreenshot, // fileName
                Stream = file64,
                AttachmentType = "GeneralAttachment",
            };

            // add test case result attachment and the reference id of it together
            this.testCaseResultAttachmentRefs.Add(testAtt);
            this.testCaseResultAttachmentIds.Add(this.stepIdCounter);

            Logger.Info("------------------ Added attachment : " + testAtt.FileName);
        }

        /// <summary>
        /// The AddTestCaseAttachment.
        /// </summary>
        /// <param name="comment">The comment <see cref="string"/>.</param>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>
        /// <param name="fileName">The file name<see cref="string"/>.</param>
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.teamfoundation.testmanagement.webapi.testhttpclientbase.createtestresultattachmentasync?view=azure-devops-dotnet
        public void AddTestStepAttachment(string comment, string filePath, string fileName, string testCaseId)
        {
            Uri uri = new Uri(this._uri);

            this.credentials = new VssBasicCredential("", this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);
            TestManagementHttpClient testMngmnt = this.connection.GetClient<TestManagementHttpClient>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Could not read file at path {filePath} because did not exist");
                return;
            }

            // we need to encode the string as base 64
            byte[] bytes = File.ReadAllBytes(filePath);

            string fileNameErrorScreenshot = filePath.Substring(filePath.LastIndexOf("\\") + 1);

            string file64 = Convert.ToBase64String(bytes);

            TestAttachmentRequestModel testAtt = new TestAttachmentRequestModel()
            {
                Comment = comment,
                FileName = fileNameErrorScreenshot, // fileName
                Stream = file64,
                AttachmentType = "GeneralAttachment",
            };

            Logger.Info("------------------ Added attachment : " + testAtt.FileName);

            TestAttachmentReference attachmentRef = testMngmnt.CreateTestIterationResultAttachmentAsync(
                testAtt,
                this._project,
                this.testRun.Id,
                this.TestCaseMapping[int.Parse(testCaseId)],
                iterationId: 1,
                actionPath: this.stepIdCounter.ToString("x8")).Result;

            Logger.Info("URL for new test attachment: " + attachmentRef.Url);
        }

        /// <summary>
        /// Record a test step result.
        /// </summary>
        /// <param name="outcome">The result of the step execution <see cref="string"/>.</param>
        /// <param name="actual">The actual result of the execution stored in the comment <see cref="string"/>.</param>
        public void RecordTestStepResult(string outcome, string actual)
        {
            // convert to hex code for the action path
            string hex = this.stepIdCounter.ToString("x8"); // base 16

            DateTime completedTime = DateTime.UtcNow;
            TimeSpan runDiff = completedTime.Subtract(this.testStepST);

            Logger.Info("Test step started at: " + this.testStepST.ToString() + " and ended at " + completedTime.ToString() + " with a difference of " + runDiff.TotalSeconds+ " seconds.");

            Microsoft.TeamFoundation.TestManagement.WebApi.SharedStepModel sharedStepModel =
                new Microsoft.TeamFoundation.TestManagement.WebApi.SharedStepModel();

            // Test step execution result starting at index 1
            // action path (note that it starts internally for the first step with 2.
            Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel actionResult =
                new Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel(
                    actionPath: $"{hex}",
                    iterationId: "1",
                    sharedStepModel: sharedStepModel,
                    outcome: $"{outcome}",
                    startedDate: this.testStepST.ToString(),
                    completedDate: completedTime.ToString(),
                    comment: "Comment test",
                    duration: runDiff.TotalMilliseconds.ToString(),
                    errorMessage: actual);

            this.stepIdCounter++;
            this.testStepST = DateTime.UtcNow;

            // this is the result that is passed at the end of the execution and viewable on DevOps. 
            // this item should correspond to the actual result. We will keep on adding action results
            this.iteration.ActionResults.Add(actionResult);
        }

        /// <summary>
        /// Create a Test Case Result by setting this.iteration.
        /// </summary>
        public void CreateTestCaseResult()
        {
            // start time of the test case result
            this.testCaseST = DateTime.UtcNow;

            // reset the test step counter to 1
            this.stepIdCounter = 1;
            this.testStepST = DateTime.UtcNow;

            // here we will put the overall test case execution result
            this.iteration = new TestIterationDetailsModel()
            {
                Id = 1,
                StartedDate = this.testCaseST,
                ActionResults = new List<TestActionResultModel> { },
                Comment = "From the REST API automated run",
                ErrorMessage = "Iteration Details Error Message Placeholder",
            };
        }

        /// <summary>
        /// Prints result of the test run
        /// </summary>
        public void PrintBasicRunInfo()
        {
            Console.WriteLine("Information for test run:" + this.testRun.Id);
            Console.WriteLine("Automated - {0}; Start Date - '{1}'; Completed date - '{2}'", this.testRun.IsAutomated ? "Yes" : "No", testRun.StartedDate.ToString(), testRun.CompletedDate.ToString());
            Console.WriteLine("Total tests - {0}; Passed tests - {1}", this.testRun.TotalTests, this.testRun.PassedTests);
        }

        /// <summary>
        /// Add test cases to test suite of current test suites.
        /// </summary>
        /// <param name="testCaseIdList">The Azure DevOps test case ids that we are adding. <see cref="string"/>.</param>
        public void AddTestCasesToSuite(List <string> testCaseIdList)
        {
            Uri uri = new Uri(this._uri);
            this.credentials = new VssBasicCredential(string.Empty, this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);
            TestManagementHttpClient testMngmnt = this.connection.GetClient<TestManagementHttpClient>();

            // watch out for proxy issues here

            // string commaSeparatedIds = String.Join(",", testCaseIdList.GetRange(0, 50));

            string commaSeparated = string.Empty;

            // here we get the test cases in the test suite
            var testSuiteListRef = testMngmnt.GetTestCasesAsync(this._project, this.testPlan.Id, int.Parse(this.testSuite)).Result;

            Logger.Info("Test Suite ID: " + this.testSuite);

            foreach (SuiteTestCase val in testSuiteListRef)
            {
                // remove test cases that remain in the test suite
                Logger.Info("Removing Test Case: " + val.Workitem.Id);

                // System.TimeSpan maxWait = System.TimeSpan.FromSeconds(10);
                var awaitVal = testMngmnt.RemoveTestCasesFromSuiteUrlAsync(this._project, this.testPlan.Id, int.Parse(this.testSuite), val.Workitem.Id).SyncResult;

                // sleep for 1 second to ensure that all test cases are removed
                System.Threading.Thread.Sleep(1000);
            }

            int UPLOAD_AMT = 1;
            for (int i = 0; i < testCaseIdList.Count; i++)
            {
                // upload in multiples of UPLOAD_AMT and check that we are not at 0 when we start
                // or upload when we reach the very last test case id regardeless
                if ((i % UPLOAD_AMT == 0) || (i == testCaseIdList.Count - 1))
                {
                    commaSeparated += testCaseIdList[i];
                    Logger.Info($"Adding Test Case index {i}: " + commaSeparated);

                    var await = testMngmnt.AddTestCasesToSuiteAsync(this._project, this.testPlan.Id, int.Parse(this.testSuite), commaSeparated).Result;

                    //// update the test case mapping
                    // if it has been added, just update it
                    commaSeparated = string.Empty;
                }
                else
                {
                    commaSeparated += testCaseIdList[i] + ",";
                }
            }
         }

        /// <summary>
        /// Record a test this method records a test case result
        /// </summary>
        /// <param name="outcome">The test case outcome. <see cref="string"/>.</param>
        public void RecordTestCaseResult(string outcome, string testCaseId, ITestStatus testCaseStatus)
        {
            Uri uri = new Uri(this._uri);
            this.credentials = new VssBasicCredential("", this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);
            TestManagementHttpClient testMngmnt = this.connection.GetClient<TestManagementHttpClient>();

            // iteration
            DateTime completedTime = DateTime.UtcNow;

            TimeSpan runDiff = completedTime.Subtract(this.testCaseST);
            Logger.Info("Test Case stated at: " + this.testCaseST.ToString() + " and ended at " + completedTime.ToString() + " with a difference of " + runDiff.TotalMilliseconds + " milliseconds ");

            // create attachment model
            // https://learn.microsoft.com/en-us/dotnet/api/microsoft.teamfoundation.testmanagement.webapi.testhttpclientbase?view=azure-devops-dotnet
            List<TestCaseResultAttachmentModel> testCaseResultAttachments = new List<TestCaseResultAttachmentModel>();
            for (int i = 0; i < this.testCaseResultAttachmentRefs.Count; i++)
            {
                // convert test case result attatchment model into test result attatchments
                TestAttachmentReference newAttachment = testMngmnt.CreateTestResultAttachmentAsync(
                    this.testCaseResultAttachmentRefs[i],
                    this._project,
                    this.testRun.Id,
                    this.TestCaseMapping[int.Parse(testCaseId)]
                    ).Result;

                Logger.Info("URL for create test attachment reference: " + newAttachment.Url);
            }

            this.testCaseResultAttachmentRefs = new List<TestAttachmentRequestModel>();
            this.testCaseResultAttachmentIds = new List<int>();

            IdentityRef runById = new IdentityRef();

            string testerEmail = InformationObject.TesterEmail;
            string testerName = InformationObject.TesterName;
            string machineName = Environment.MachineName;

            // runById.DisplayName = "Nic Louie";
            runById.DisplayName = testerName;

            // here we create the test case result 
            TestCaseResult testCaseRes = new TestCaseResult()
            {
                State = "Completed",
                Outcome = $"{outcome}",
                Id = this.TestCaseMapping[int.Parse(testCaseId)],
                CompletedDate = completedTime,
                RunBy = runById,
                DurationInMs = runDiff.TotalMilliseconds,
                ErrorMessage = testCaseStatus.FriendlyErrorMessage,
                ComputerName = machineName,
                StackTrace = testCaseStatus.ErrorStack,
                Priority = 1,
                Owner = runById,
            };

            // if the test failed, populate the outcome as Failed
            if (outcome == "Failed")
            {
                testCaseRes.Comment = "Awaiting Analysis";
            }

            // record the finished date as the same for the iteration.
            this.iteration.CompletedDate = completedTime;
            this.iteration.Outcome = outcome; // set outcome of iteration to outcome of test case execution
            this.iteration.DurationInMs = runDiff.TotalMilliseconds * 10000; // calculuation for this is weird
            this.iteration.Attachments = testCaseResultAttachments;

            Logger.Info("Test Case Run Duration in seconds: " + runDiff.TotalSeconds.ToString());

            testCaseRes.IterationDetails = new List<TestIterationDetailsModel> { this.iteration };

            TestCaseResult[] testCaseArr = new TestCaseResult[] { testCaseRes };

            VssConnection connection = new VssConnection(uri, this.credentials);
            TestManagementHttpClient testMngmnt1 = this.connection.GetClient<TestManagementHttpClient>();

            // update the single test case result for the test case
            var await = testMngmnt1.UpdateTestResultsAsync( testCaseArr, this._project, this.testRun.Id).Result;

            // Logger.Info("------------------------------ Successfully Recorded Test Case Result -------------------");

            this.testCaseCounter += 1; // increment the test case counter by 1 up

            Logger.Info("Test Case Run Counter: " + this.testCaseCounter);
        }

        /// <summary>
        /// Create a test plan using direct HTTP (not available using Client Lib).
        /// </summary>
        /// <param name="testSuiteName"> Name of the Test Suite being created. <see cref="string"/>.</param>
        /// <param name="parentTestSuiteId"> The parent test suite id to creat the test suite under <see cref="string"/>.</param>
        private string CreateTestSuite(string testSuiteName, string parentTestSuiteId)
        {
            string uri = this._uri;
            string personalAccessToken = this._personalAccessToken;
            string project = this._project;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            // set the fields for the creation of the test plan
            Dictionary<string, string> values = new Dictionary<string, string>
                  {
                      { "name", $"{testSuiteName}" },
                      { "suiteType", "StaticTestSuite" },
                  };

            string jsonValue = JsonConvert.SerializeObject(values, Formatting.Indented);

            var postValue = new StringContent(jsonValue, Encoding.UTF8, "application/json");

            // create an http client (recommended method)
            using (var client = new HttpClient())
            {
                // set our headers for authenticating
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // set the message url
                string message = $"{uri}/{project}/_apis/test/Plans/{this.testPlan.Id}/suites/{parentTestSuiteId}?api-version=5.0";

                // send the message with the post value as the contents
                var response = client.PostAsync(message, postValue).Result;

                // if the response is successfull, set the result to the test plan object
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;

                    // parse data so that we find the id of the test suite
                    int loc = data.IndexOf("id");
                    List<string> list_data = data.Split(new char[] {','}).ToList();

                    list_data = list_data[0].Split(new char[] {':'}).ToList();

                    string locData = list_data.Last();

                    // Console.WriteLine("Test Case Successfully Created: Test Plan #{0}", testPlan.Id);
                    Logger.Info("Created Test Suite is: " + locData);

                    return locData;
                }
                else
                {
                    Logger.Info("Error posting test suite: " + response.Content);
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Check if a test suite is found under a test plan, if so, return the test suite id.
        /// </summary>
        /// <param name="testSuiteName">The test suite name. <see cref="string"/>.</param>
        /// <param name="testSuiteId">The test parent suite id. <see cref="string"/>.</param>
        private string CheckTestSuiteChildren(string testSuiteName, string testSuiteId)
        {
            string uri = this._uri;
            string personalAccessToken = this._personalAccessToken;
            string project = this._project;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            // create an http client (recommended method)
            using (var client = new HttpClient())
            {
                // set our headers for authenticating
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // set the message url
                string message = $"{uri}/{project}/_apis/testplan/Plans/{this.testPlan.Id}/suites/{testSuiteId}?expand=Children&api-version=7.0";

                // send the message with the post value as the contents
                var response = client.GetAsync(message).Result;

                // if the response is successfull, set the result to the test plan object
                if (response.IsSuccessStatusCode)
                {
                    Microsoft.TeamFoundation.TestManagement.WebApi.TestSuite testSuiteParent = response.Content.ReadAsAsync<Microsoft.TeamFoundation.TestManagement.WebApi.TestSuite>().Result;

                    // if the number is not greater than 0
                    if (testSuiteParent.Children != null)
                    {
                        foreach (var testSuite in testSuiteParent.Children)
                        {
                            if (testSuite.Name.Contains(testSuiteName))
                            {
                                return testSuite.Id.ToString();
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if a test suite is created and if so, return the test set id. This means that the test suite must have an unique name within the test plan.
        /// </summary>
        /// <param name="testSuiteName">The test suite name. <see cref="string"/>.</param>
        /// <param name="suiteId">The test suite id. <see cref="string"/>.</param>
        private string CheckForTestSuite(string testSuiteName, int suiteId)
        {
            // Logger.Info("Checking suite for " + testSuiteName + " using suite id " + suiteId.ToString());

            Uri uri = new Uri(this._uri);
            this.credentials = new VssBasicCredential("", this._personalAccessToken);
            this.connection = new VssConnection(uri, this.credentials);

            TestPlanHttpClient testPlanClient = this.connection.GetClient<TestPlanHttpClient>();

            List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry> listTestSuites = testPlanClient.GetSuiteEntriesAsync(this._project, suiteId, SuiteEntryTypes.Suite).Result ;

            Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite newSuite;

            foreach (Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry suite in listTestSuites)
            {
                newSuite = testPlanClient.GetTestSuiteByIdAsync(this._project, this.testPlan.Id, suite.Id).Result;

                Logger.Info("Suite Name: " + newSuite.Name);

                if (newSuite.Name == testSuiteName)
                {
                    return newSuite.Id.ToString();
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if a test plan exists using direct HTTP (not available using Client Lib)
        /// </summary>
        /// <param name="testPlanName">The test plan name. <see cref="string"/>.</param>
        private string CheckForTestPlan(string testPlanName)
        {
            if (testPlanName.Trim() == string.Empty)
            {
                Logger.Info("Test Plan Name not specified");
                return string.Empty;
            }

            VssBasicCredential credentials = new VssBasicCredential(string.Empty, this._personalAccessToken);
            var connection = new VssConnection(new Uri(this._uri), credentials);
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get 2 levels of query hierarchy items
            List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(this._project, depth: 2).Result;

            // Search for 'My Queries' folder
            QueryHierarchyItem myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));
            if (myQueriesFolder != null)
            {
                string queryName = "Query Test Plans " + testPlanName;

                // See if our 'REST Sample' query already exists under 'My Queries' folder.
                QueryHierarchyItem newTestCaseQuery = null;

                if (myQueriesFolder.Children != null)
                {
                    newTestCaseQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(queryName));
                }

                if (newTestCaseQuery == null)
                {
                    // if the 'REST Sample' query does not exist, create it.
                    newTestCaseQuery = new QueryHierarchyItem()
                    {
                        Name = queryName,
                        Wiql = $"SELECT [System.Id],[System.WorkItemType],[System.Title],[System.AssignedTo],[System.State],[System.Tags] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.WorkItemType] = 'Test Plan' AND [System.Title] = '{testPlanName}'",
                        IsFolder = false,
                    };
                    // create a query if it doesn't already exist
                    newTestCaseQuery = witClient.CreateQueryAsync(newTestCaseQuery, this._project, myQueriesFolder.Name).Result;
                }

                // run the 'REST Sample' query
                WorkItemQueryResult result = witClient.QueryByIdAsync(newTestCaseQuery.Id).Result;

                if (result.WorkItems.Any())
                {
                    const int batchSize = 10;
                    IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference> workItemRefs;
                    do
                    {
                        workItemRefs = result.WorkItems.Take(batchSize);

                        if (workItemRefs.Count() > 1)
                        {
                            Logger.Info("Number of Test Plans is greater than 1");
                        }

                        if (workItemRefs.Any())
                        {
                            // get details for each work item in the batch
                            List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = witClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)).Result;
                            foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in workItems)
                            {
                                // write work item to console
                                Console.WriteLine("{0} {1}", workItem.Id, workItem.Fields["System.Title"]);

                                // we will return the very first item
                                if (workItem.Fields["System.Title"].ToString() == testPlanName)
                                {

                                    return workItem.Id.ToString();
                                }
                            }
                        }
                    }
                    while (workItemRefs.Count() == batchSize);
                    return string.Empty;
                }
                else
                {
                    Logger.Info("No work items were returned from query.");
                    return string.Empty;
                }
            }
            Logger.Info("Error in querying for test plan");
            return string.Empty;
        }

        /// <summary>
        /// Create a test plan using direct HTTP (not available using Client Lib)
        /// </summary>
        /// <param name="testPlanId">The test plan id. <see cref="string"/>.</param>
        private void GetTestPlanByID(string testPlanId)
        {
            string uri = this._uri;
            string personalAccessToken = this._personalAccessToken;
            string project = this._project;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            // create an http client (recommended method)
            using (var client = new HttpClient())
            {
                // set our headers for authenticating
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // set the message url
                string message = $"{uri}/{project}/_apis/testplan/plans/{testPlanId}?api-version=7.0";

                // send the message with the post value as the contents
                var response = client.GetAsync(message).Result;

                // if the response is successfull, set the result to the test plan object
                if (response.IsSuccessStatusCode)
                {
                    Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan result = response.Content.ReadAsAsync<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>().Result;

                    this.testPlan = result;

                    // Console.WriteLine("Test Case Successfully Created: Test Plan #{0}", testPlan.Id);
                    Logger.Info("Root Test Suite was also created with: " + result.RootSuite.Name);
                }
                else
                {
                    Logger.Error("Error getting Test Plan: " + response.Content);
                    this.testPlan = null;
                }
            }
        }

        /// <summary>
        /// Create a test plan using direct HTTP (not available using Client Lib)
        /// </summary>
        /// <param name="name">The test plan name. <see cref="string"/>.</param>
        /// <param name="description">The test plan description. <see cref="string"/>.</param>
        private void CreateTestPlan(string name, string description)
        {
            string uri = this._uri;
            string personalAccessToken = this._personalAccessToken;
            string project = this._project;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            // set the fields for the creation of the test plan
            Dictionary<string, string> values = new Dictionary<string, string>
                  {
                      { "name", $"{name}" },
                      { "description", $"{description}" },
                  };

            string jsonValue = JsonConvert.SerializeObject(values, Formatting.Indented);

            var postValue = new StringContent(jsonValue, Encoding.UTF8, "application/json");

            // create an http client (recommended method)
            using (var client = new HttpClient())
            {
                // set our headers for authenticating
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                // set the message url
                string message = $"{uri}/{project}/_apis/testplan/plans?api-version=7.0";

                // send the message with the post value as the contents
                var response = client.PostAsync(message, postValue).Result;

                // if the response is successfull, set the result to the test plan object
                if (response.IsSuccessStatusCode)
                {
                    var testPlan = response.Content.ReadAsAsync<Microsoft.TeamFoundation.TestManagement.WebApi.TestPlan>().Result;

                    Logger.Info("Test Case Successfully Created: Test Plan #{0}" + testPlan.Id);
                    Logger.Info("Test Suite was also generated with: " + testPlan.RootSuite.Name);

                    this.testPlan = testPlan;
                }
                else
                {
                    Logger.Error("Error creating Test Plan: {0}" + response.Content);
                    this.testPlan = null;
                }
            }
        }
    }
}