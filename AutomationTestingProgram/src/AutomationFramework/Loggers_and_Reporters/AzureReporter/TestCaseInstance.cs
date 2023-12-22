namespace AzureReporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using AutomationTestingProgram;
    using Microsoft.TeamFoundation.TestManagement.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi;
    using Microsoft.VisualStudio.Services.WebApi.Patch;
    using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
    using Newtonsoft.Json;

    public class TestCaseInstance
    {
        readonly string _uri;
        readonly string _personalAccessToken;
        readonly string _project;

        private ITestBase testSteps;

        public List<string> testCaseList;

        private JsonPatchDocument patchDocument;

        public TestCaseInstance(string url, string pat, string project)
        {
            _uri = url;
            _personalAccessToken = pat;
            _project = project;

            this.testCaseList = new List<string> { };
        }

        /// <summary>
        /// Create a new test step for test cases that are already created and that we don't need to create using a client library.
        /// </summary>
        public void CreateTestCaseReference()
        {
            TestBaseHelper helper = new TestBaseHelper();
            this.testSteps = helper.Create();
            this.patchDocument = new JsonPatchDocument();
        }

        /// <summary>
        /// Create a bug using the .NET client library
        /// Takes in a paramter for the test case name that we will create.
        /// </summary>
        public void CreateTestCaseUsingClientLib(string testCaseName, int priority, string description)
        {
            Uri uri = new Uri(_uri);
            string personalAccessToken = _personalAccessToken;
            string project = _project;

            TestBaseHelper helper = new TestBaseHelper();
            this.testSteps = helper.Create();
            this.patchDocument = new JsonPatchDocument();

            // add fields and thier values to your patch document
            this.patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = $"{testCaseName}",
                });

            this.patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Description",
                    Value = $"{description}",
                });

            this.patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Common.Priority",
                    Value = $"{priority}",
                });

            this.patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.TCM.AutomationStatus",
                    Value = "Not Automated", // this cannot be change to automated
                });
            Logger.Info("Finished creating test case");
        }

        /// <summary>
        /// This function creates a new test step and saves it to this.testStep.
        /// </summary>
        public void CreateTestStep(string title, string expected, string description)
        {
            // create a test step
            ITestStep testStep = this.testSteps.CreateTestStep();
            testStep.Title = $"{title}";
            testStep.ExpectedResult = $"{expected}";
            testStep.Description = $"{description}";

            // add step action to testbase object
            this.testSteps.Actions.Add(testStep);
        }

        // this function saves the test steps into the UI by using the UpdateWorkItemAsync method
        public string SaveTestCaseSteps(string testCaseId)
        {
            // update json based on all actions (including teststeps and teststep attachemnts) 
            Uri uri = new Uri(this._uri);

            VssBasicCredential credentials = new VssBasicCredential("", this._personalAccessToken);

            // errors occur here
            VssConnection connection = new VssConnection(uri, credentials);

            WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            this.patchDocument = this.testSteps.SaveActions(this.patchDocument);

            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem result;

            if (testCaseId != "placeholder")
            {
                result = workItemTrackingHttpClient.UpdateWorkItemAsync(this.patchDocument, int.Parse(testCaseId)).Result;
            }
            else
            {
                Console.WriteLine("Creating new test case");
                result = workItemTrackingHttpClient.CreateWorkItemAsync(this.patchDocument, _project, "Test Case").Result;
            }

            Console.WriteLine("Test Case Successfully Created: Test Case #{0}", result.Id);

            return result.Id.ToString();
        }

        // queries for test cases and returns whether or not the test case exists using sdk not queries
        // if it does exist, return the workitem id of the first corresponding test case 
        public string QueryForTestCaseFaster(string testCaseName)
        {
            string uri = this._uri;
            string personalAccessToken = this._personalAccessToken;
            string project = this._project;
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));

            // set the fields for the creation of the test plan
            Dictionary<string, string> values = new Dictionary<string, string>
                  {
                      { "query", $"SELECT [Title],[ID]  FROM WorkItems where [Title] = '{testCaseName}'" },
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
                string message = $"{uri}/{project}/_apis/wit/wiql?api-version=7.0";

                // send the message with the post value as the contents
                var response = client.PostAsync(message, postValue).Result;

                // if the response is successfull, set the result to the test plan object
                if (response.IsSuccessStatusCode)
                {
                    var wiqlResult = response.Content.ReadAsAsync<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemQueryResult>().Result;

                    int skip = 0;
                    skip = this.testCaseList.Where(x => x.Equals(testCaseName)).Count();

                    Console.WriteLine("skip result " + skip);

                    // return false if the number of work items is 0 or if we are skipping more values than actually exist
                    if (wiqlResult.WorkItems.Count() == 0 || (skip >= 1 && skip >= wiqlResult.WorkItems.Count()))
                    {
                        Console.WriteLine("Count is zero, no work item found");
                        return string.Empty;
                    }

                    int idQueried = wiqlResult.WorkItems.Skip(skip).First().Id;

                    Console.WriteLine("Test Case Successfully received: " + idQueried);
                    return idQueried.ToString();
                }
                else
                {
                    Console.WriteLine("Error querying for Test Case: {0}", response.Content);
                }
            }
            Console.WriteLine("--------------------------------------------Failed in query for test case---------------------------------------");

            return string.Empty;
        }

            // queries for test cases and returns whether or not the test case exists
            // if it does exist, return the workitem id of the first corresponding test case 
        public string QueryForTestCase(string testCaseName)
            {
            if (testCaseName.Trim() == string.Empty)
            {
                Console.WriteLine("ERROR: Missing test case name in file");
                return string.Empty;
            }

            VssBasicCredential credentials = new VssBasicCredential("", this._personalAccessToken);
            var connection = new VssConnection(new Uri(this._uri), credentials);
            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // Get 2 levels of query hierarchy items
            List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(_project, depth: 2).Result;

            // Search for 'My Queries' folder
            QueryHierarchyItem myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));
            if (myQueriesFolder != null)
            {
                string queryName = "Query Test Cases";

                // See if our 'REST Sample' query already exists under 'My Queries' folder.
                QueryHierarchyItem newTestCaseQuery = null;

                // if there are children, determine if the query exists
                if (myQueriesFolder.Children != null)
                {
                    newTestCaseQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(queryName));
                }

                if (newTestCaseQuery == null)
                {
                    // if the 'Query Test Cases' query does not exist, create it. However, this will need to be updated later.
                    newTestCaseQuery = new QueryHierarchyItem()
                    {
                        Name = queryName,
                        Wiql = $"SELECT [System.Id],[System.WorkItemType],[System.Title],[System.AssignedTo],[System.State],[System.Tags] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.WorkItemType] = 'Test Case' AND [System.Title] = '{testCaseName}'",
                        IsFolder = false
                    };
                    // create a query if it doesn't already exist
                    newTestCaseQuery = witClient.CreateQueryAsync(newTestCaseQuery, _project, myQueriesFolder.Name).Result;
                }
                else
                {
                    // if newTestCaseQuery is not null, then update it
                    newTestCaseQuery.Wiql = $"SELECT [System.Id],[System.WorkItemType],[System.Title],[System.AssignedTo],[System.State],[System.Tags] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.WorkItemType] = 'Test Case' AND [System.Title] = '{testCaseName}'";
                    newTestCaseQuery = witClient.UpdateQueryAsync(newTestCaseQuery, _project, newTestCaseQuery.Id.ToString()).Result;
                }

                // run the 'REST Sample' query
                WorkItemQueryResult result = witClient.QueryByIdAsync(newTestCaseQuery.Id).Result;

                if (result.WorkItems.Any())
                {
                    int skip = 0;
                    skip = this.testCaseList.Where(x => x.Equals(testCaseName)).Count();

                    const int batchSize = 100;
                    IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference> workItemRefs;
                    do
                    {
                        workItemRefs = result.WorkItems.Skip(skip).Take(batchSize);

                        if (workItemRefs.Any())
                        {
                            // get details for each work item in the batch
                            List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> workItems = witClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)).Result;
                            foreach (Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem in workItems)
                            {
                                // we will return the very first item
                                if (workItem.Fields["System.Title"].ToString() == testCaseName)
                                {
                                    // return workItem id
                                    Console.WriteLine("{0} {1}", workItem.Id, workItem.Fields["System.Title"]);
                                    return workItem.Id.ToString();
                                }
                            }
                        }

                        skip += batchSize;
                    }
                    while (workItemRefs.Count() == batchSize);
                    return string.Empty;
                }
                else
                {
                    Console.WriteLine("No work items were returned from query.");
                    return string.Empty;
                }
            }

            Console.WriteLine("Error in querying for test case");
            return string.Empty;
        }
    }
}