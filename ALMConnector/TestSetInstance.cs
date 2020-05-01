// <copyright file="TestSetInstance.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using TDAPIOLELib;

    /// <summary>
    /// A class to represent a test case instance on ALM.
    /// </summary>
    public class TestSetInstance
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
        public string buildNumber { get; set; } = "My build number";

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
        /// Gets or sets the attachments. It has to be List of objects since if string, the backend will not take it... (it has to be object instead of string array).
        /// </summary>
        public List<object> Attachments { get; set; } = new List<object>();

        /// <summary>
        /// Defines the CurrTestCaseID.
        /// </summary>
        private int currTestCaseID;

        /// <summary>
        /// Defines the LastTestCaseID.
        /// </summary>
        private int lastTestCaseID;

        /// <summary>
        /// Defines the testCaseConditions.
        ///  source -> Target, CondType.
        /// </summary>
        private Dictionary<int, List<int>> testCaseFlow = new Dictionary<int, List<int>>();

        /// <summary>
        /// Defines the testCaseDict.
        /// </summary>
        private Dictionary<int, ITSTest> testCaseDict = new Dictionary<int, ITSTest>();

        /// <summary>
        /// Defines testCaseCondDependencyGraph.
        /// </summary>
        private Dictionary<int, HashSet<int>> testCaseCondDependencyGraph = new Dictionary<int, HashSet<int>>();

        /// <summary>
        /// Defines the CurrTestCase.
        /// </summary>
        private TestCaseInstance currTestCase;

        /// <summary>
        /// The username.
        /// </summary>
        private string username;


        /// <summary>
        /// Initializes a new instance of the <see cref="TestSetInstance"/> class.
        /// </summary>
        /// <param name="testSet">The TestSet<see cref="TestSet"/>.</param>
        /// <param name="username">The username of the alm</param>
        public TestSetInstance(TestSet testSet, string username)
        {
            this.TestSet = testSet;
            this.Name = testSet.Name;
            this.ID = $"{testSet.ID}";
            this.username = username;

            try
            {
                this.IniatilizeTestSet();
            }
            catch (Exception e)
            {
                throw new ExecutionFlowDeterminer(ExecutionFlowDeterminer.ErrorMsg + " " + e.ToString());
            }

            this.currTestCase = new TestCaseInstance(this.testCaseDict[this.currTestCaseID], this.GetField("Test Environment"), this.buildNumber);
        }

        /// <summary>
        /// Gets or sets the TestSet.
        /// </summary>
        public TestSet TestSet { get; set; }

        /// <summary>
        /// Gets or sets the TestSet ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the EmailList.
        /// </summary>
        public string EmailList { get; set; } = string.Empty;

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
        public List<TestCaseExecution> TestCaseExecutions { get; set; } = new List<TestCaseExecution>();

        /// <summary>
        /// The Field.
        /// </summary>
        /// <param name="fieldName">The fieldName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetField(string fieldName)
        {
            if (this.TestSet != null && fieldName.Count() != 0)
            {
                // checks if field name is correct, probably is a better way of doing this
                if (ConfigurationManager.AppSettings.AllKeys.Contains(fieldName))
                {
                    // update field with new value
                    try
                    {
                        string fieldcode = ConfigurationManager.AppSettings[fieldName].ToString(); // gets field code
                        var value = this.TestSet[fieldcode];
                        return value != null ? value.ToString() : string.Empty;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Field Name '" + fieldName + "' is not valid");
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// The Field.
        /// </summary>
        /// <param name="fieldName">The fieldName<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetCurrentTestCaseField(string fieldName)
        {
            if (this.TestSet != null && fieldName.Count() != 0)
            {
                // checks if field name is correct, probably is a better way of doing this
                if (ConfigurationManager.AppSettings.AllKeys.Contains(fieldName))
                {
                    // update field with new value
                    try
                    {
                        string fieldcode = ConfigurationManager.AppSettings[fieldName].ToString(); // gets field code
                        return this.currTestCase.GetField(fieldcode);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Field Name '" + fieldName + "' is not valid");
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// The Field.
        /// </summary>
        /// <param name="fieldName">The fieldName<see cref="string"/>.</param>
        /// <param name="value"> The value to be overriden. </param>
        /// <returns>The <see cref="string"/>.</returns>
        public bool SetField(string fieldName, object value)
        {
            if (this.TestSet != null && fieldName.Count() != 0)
            {
                // checks if field name is correct, probably is a better way of doing this
                if (ConfigurationManager.AppSettings.AllKeys.Contains(fieldName))
                {
                    // update field with new value
                    try
                    {
                        string fieldcode = ConfigurationManager.AppSettings[fieldName].ToString(); // gets field code
                        this.TestSet.UnLockObject();
                        this.TestSet[fieldcode] = value;
                        this.TestSet.Post();
                        this.TestSet.UnLockObject();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Field Name '" + fieldName + "' is not valid");
                }
            }

            return false;
        }

        /// <summary>
        /// The Field.
        /// </summary>
        /// <param name="fieldName">The fieldName<see cref="string"/>.</param>
        /// <param name="value"> The value to be overriden. </param>
        /// <returns>The <see cref="string"/>.</returns>
        public bool SetCurrentTestCaseField(string fieldName, object value)
        {
            if (this.TestSet != null && fieldName.Count() != 0)
            {
                // checks if field name is correct, probably is a better way of doing this
                if (ConfigurationManager.AppSettings.AllKeys.Contains(fieldName))
                {
                    // update field with new value
                    try
                    {
                        string fieldcode = ConfigurationManager.AppSettings[fieldName].ToString(); // gets field code
                        this.currTestCase.SetField(fieldcode, value);
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Field Name '" + fieldName + "' is not valid");
                }
            }

            return false;
        }

        /// <summary>
        /// The GetFolderPath.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetFolderPath()
        {
            if (this.TestSet != null)
            {
                TestSetFolder folder = this.TestSet.TestSetFolder as TestSetFolder;
                return folder.Path;
            }
            else
            {
                Console.WriteLine("Test set is null");
                return null;
            }
        }

        /// <summary>
        /// Moves to the next test case. User must check for the condition type themselves. Conidtion type can return EXEC_FINISHED or EXEC_PASSED.
        /// </summary>
        /// <returns>true if there is a next test case.</returns>
        public bool MoveToNextTestCase()
        {
            if (this.currTestCaseID != this.lastTestCaseID)
            {
                this.currTestCaseID = this.testCaseFlow[this.currTestCaseID][0];
                this.currTestCase = new TestCaseInstance(this.testCaseDict[this.currTestCaseID], this.GetField("Test Environment"), this.buildNumber);
                return true;
            }

            this.Finished = DateTime.Now;
            return false;
        }

        /// <summary>
        /// The GetCurrentTestCaseName.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetCurrentTestCaseName()
        {
            return this.currTestCase.Name;
        }

        /// <summary>
        /// The GetCurrentTestCaseConditionType.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int GetCurrentTestCaseConditionType()
        {
            return this.currTestCaseID == this.lastTestCaseID ? EXECFINISHED : this.testCaseFlow[this.currTestCaseID][1];
        }

        /// <summary>
        /// The AddTestCaseAttachment.
        /// </summary>
        /// <param name="description">The description<see cref="string"/>.</param>
        /// <param name="filePath">The filePath<see cref="string"/>.</param>

        public void AddTestCaseAttachment(string description, string filePath)
        {
            string attachmentName = this.currTestCase.AddAttachment(description, filePath);

            // we keep track of the server filename for each attachment to be sent in execution report.
            this.Attachments.Add(attachmentName);
        }

        /// <summary>
        /// The AddTestStepToTestCase.
        /// </summary>
        /// <param name="testStepName">The testStepName<see cref="string"/>.</param>
        /// <param name="testStepStatus">The testStepStatus<see cref="string"/>.</param>
        /// <param name="testStepDescription">The testStepDescription<see cref="string"/>.</param>
        /// <param name="testStepExpected">The testStepExpected<see cref="string"/>.</param>
        /// <param name="testStepActual">The testStepActual<see cref="string"/>.</param>
        public void AddTestStepToTestCase(string testStepName, string testStepStatus, string testStepDescription, string testStepExpected, string testStepActual)
        {
            this.currTestCase.AddTestStep(testStepName, testStepStatus, testStepDescription, testStepExpected, testStepActual);
        }

        /// <summary>
        /// The SetTestCaseRunStatus.
        /// </summary>
        /// <param name="testStatus">The testStatus<see cref="string"/>.</param>
        public void SetTestCaseRunStatus(string testStatus)
        {
            this.currTestCase.SetTestRunStatus(testStatus);
            this.TestCaseExecutions.Add(new TestCaseExecution()
            {
                Attachments = null,
                ExecDateTime = DateTime.Now,
                HasScreenShot = this.currTestCase.HasAttachment,
                LogName = $"{this.currTestCase.RunName}.log",
                RunID = this.currTestCase.RunID,
                RunName = this.currTestCase.RunName,
                Status = this.currTestCase.TestStatus,
                TestCaseName = this.currTestCase.Name,
                TesterName = this.username,
            });
        }

        /// <summary>
        /// The SetTestCaseRunStatus.
        /// </summary>
        /// <param name="testPassed">The testPassed<see cref="bool"/>.</param>
        public void SetTestCaseRunStatus(bool testPassed)
        {
            this.currTestCase.SetTestRunStatus(testPassed);
            this.TestCaseExecutions.Add(new TestCaseExecution()
            {
                Attachments = null,
                ExecDateTime = DateTime.Now,
                HasScreenShot = this.currTestCase.HasAttachment,
                LogName = $"{this.currTestCase.RunName}.log",
                RunID = this.currTestCase.RunID,
                RunName = this.currTestCase.RunName,
                Status = this.currTestCase.TestStatus,
                TestCaseName = this.currTestCase.Name,
                TesterName = this.username,
            });
        }

        /// <summary>
        /// The IniatilizeTestSet.
        /// </summary>
        private void IniatilizeTestSet()
        {
            TSTestFactory tSTestFactory = this.TestSet.TSTestFactory as TSTestFactory;

            ITestSetExecutionReportSettings reportSettings = this.TestSet.ExecutionReportSettings as ITestSetExecutionReportSettings;
            this.EmailList = reportSettings.EMailTo;

            IExecEventNotifyByMailSettings2 execNotifyByMail = this.TestSet.ExecEventNotifyByMailSettings as IExecEventNotifyByMailSettings2;
            if (execNotifyByMail.Enabled[(int)TDAPI_EXECUTIONEVENT.EXECEVENT_TESTFAIL])
            {
                this.FailedEmailList = execNotifyByMail.EMailTo;
            }

            this.Description = this.GetField("Test Set Description");
            this.Baseline = this.GetField("Test Set Baseline");
            this.TargetCycle = this.GetField("Test Set Target Cycle");

            ITDFilter tDFilter = tSTestFactory.Filter as ITDFilter;
            List testcase = tDFilter.NewList();
            this.TotalTestCases = testcase.Count;

            // Populate the testCaseDict Lookup
            foreach (ITSTest test in testcase)
            {
                int testCaseID = int.Parse($"{test.ID}");
                this.testCaseDict.Add(testCaseID, test);

                // Console.WriteLine($"({testCaseID}) - {test.Name}");
            }

            Dictionary<int, Dictionary<int, CondInfo>> conditionLookup = this.CreateConditionLookupAndPopulateDependencyGraph();
            this.currTestCaseID = this.FindStartingTestCaseID(conditionLookup);

            // create linear flow based on dependency graph
            this.CreateLinearFlow(conditionLookup, this.currTestCaseID);
            this.Started = DateTime.Now;
        }

        /// <summary>
        /// Creates the ConditionLookup which is of the format source -> {target -> CondInfo}
        /// Populates the DependencyGraph which is of the format target > {source}.
        /// </summary>
        /// <returns> ConditionLookup. </returns>
        private Dictionary<int, Dictionary<int, CondInfo>> CreateConditionLookupAndPopulateDependencyGraph()
        {
            // Get list of the conditions that are on the execution flow
            ConditionFactory cf = this.TestSet.ConditionFactory as ConditionFactory;
            List conds = cf.NewList(string.Empty);
            Dictionary<int, Dictionary<int, CondInfo>> conditionLookup = new Dictionary<int, Dictionary<int, CondInfo>>();

            // Loop through the list of conditions to populate conditions list & create a dependency matrix
            //    source -> {target -> CondInfo}
            //    target -> {source}
            foreach (ICondition c in conds)
            {
                int source = int.Parse($"{c.Source}");
                int target = int.Parse($"{c.Target}");
                int condType = int.Parse(c.Value.ToString());

                // only add condition if both source and target is in our dictionary
                // there are phantom conditions recorded =.=
                if (this.testCaseDict.ContainsKey(source) && this.testCaseDict.ContainsKey(target))
                {
                    // Console.WriteLine("{0} : [{1} -> {2}]", c.ID, this.testCaseDict[source].Name, this.testCaseDict[target].Name);

                    // Populating Dependency Graph
                    if (this.testCaseCondDependencyGraph.ContainsKey(target))
                    {
                        this.testCaseCondDependencyGraph[target].Add(source);
                    }
                    else
                    {
                        this.testCaseCondDependencyGraph[target] = new HashSet<int>() { source };
                    }

                    if (conditionLookup.ContainsKey(source))
                    {
                        conditionLookup[source].Add(target, new CondInfo() { CondType = condType, CondID = int.Parse($"{c.ID}") });
                    }
                    else
                    {
                        conditionLookup[source] = new Dictionary<int, CondInfo>
                        {
                            {
                                target, new CondInfo()
                                {
                                   CondType = condType,
                                   CondID = int.Parse($"{c.ID}"),
                                }
                            },
                        };
                    }
                }
            }

            return conditionLookup;
        }

        /// <summary>
        /// Finds the starting test case ID.
        /// </summary>
        /// <param name="conditionLookup">Dictionary of the format source -> {target -> CondInfo}.</param>
        /// <returns>Starting test case ID. </returns>
        private int FindStartingTestCaseID(Dictionary<int, Dictionary<int, CondInfo>> conditionLookup)
        {
            // intialize variables
            int startingTestCaseID = -1;
            int startingTestMinCondID = -1;

            // loop through each testCaseID
            foreach (int key in this.testCaseDict.Keys)
            {
                // a starting test case has no dependencies ==> it will not be inside the testCaseCondDependencyGraph
                if (!this.testCaseCondDependencyGraph.ContainsKey(key))
                {
                    // replace if it is the initial value
                    if (startingTestCaseID == -1)
                    {
                        startingTestCaseID = key;
                        startingTestMinCondID = this.FindMinCond(conditionLookup, key);
                    }
                    else
                    {
                        // find minimum for newTestCase and replace if needed
                        int newTestMinCondID = this.FindMinCond(conditionLookup, key);
                        if (newTestMinCondID < startingTestMinCondID)
                        {
                            startingTestCaseID = key;
                            startingTestMinCondID = newTestMinCondID;
                        }
                    }
                }
            }

            return startingTestCaseID;
        }

        private void CreateLinearFlow(Dictionary<int, Dictionary<int, CondInfo>> conditionLookup, int start)
        {
            /*** Traverse graph in a DFS manner from start node
                 At child, check if it has a parent dependency. If it has, do parent first.
                 For any conflicts for order, use minimum condition ID.
            ***/

            HashSet<int> visited = new HashSet<int>();
            HashSet<int> expanded = new HashSet<int>();

            if (!this.testCaseDict.ContainsKey(start))
            {
                return;
            }

            Stack<int> stack = new Stack<int>();

            foreach (int testcase in this.testCaseDict.Keys.Reverse())
            {
                stack.Push(testcase);
            }

            stack.Push(start);

            while (stack.Count > 0)
            {
                int node = stack.Pop();

                // If we already have seen this node, skip
                if (visited.Contains(node))
                {
                    continue;
                }

                // check if this node has any dependencies
                if (this.testCaseCondDependencyGraph.ContainsKey(node) && !expanded.Contains(node))
                {
                    // push node back on stack in case this node has unvisited dependencies
                    stack.Push(node);
                    expanded.Add(node);

                    // Add into stack all the testcases that it depends on in order that has not been visited
                    List<int> dependecyList = this.testCaseCondDependencyGraph[node]
                                                  .OrderByDescending(i => i)
                                                  .ToList();
                    bool notVisitedDependency = false;
                    foreach (int dependency in dependecyList)
                    {
                        if (!visited.Contains(dependency))
                        {
                            stack.Push(dependency);
                            notVisitedDependency = true;
                        }
                    }

                    // If we have not visited dependencies, we have to visit the node, and therefore, end this loop, else continue loop
                    if (notVisitedDependency)
                    {
                        continue;
                    }
                    else
                    {
                        // pop node back out since all dependencies have been visited already
                        stack.Pop();
                    }
                }

                visited.Add(node);
                expanded.Add(node);
                this.lastTestCaseID = node;

                // Adjacency list
                List<int> adjacencyList = new List<int>();
                if (conditionLookup.ContainsKey(node))
                {
                    adjacencyList = conditionLookup[node].Keys.ToList();
                    adjacencyList = adjacencyList.OrderByDescending(i => i).ToList();
                }

                foreach (int sibling in adjacencyList)
                {
                    if (!visited.Contains(sibling))
                    {
                        stack.Push(sibling);
                    }
                }
            }

            // Populate testCaseFlow
            int source = start;
            foreach (int target in visited)
            {
                if (target != source)
                {
                    int condType = EXECFINISHED;
                    if (conditionLookup.ContainsKey(source))
                    {
                        if (conditionLookup[source].ContainsKey(target))
                        {
                            condType = conditionLookup[source][target].CondType;
                        }
                    }

                    this.testCaseFlow.Add(source, new List<int>() { target, condType });
                    source = target;
                }
            }

            return;
        }

        private int FindMinCond(Dictionary<int, Dictionary<int, CondInfo>> conditionLookup, int testCaseID)
        {
            int minCond = int.MaxValue;
            if (conditionLookup.ContainsKey(testCaseID))
            {
                foreach (CondInfo condInfo in conditionLookup[testCaseID].Values)
                {
                    if (minCond < condInfo.CondID)
                    {
                        minCond = condInfo.CondID;
                    }
                }
            }

            return minCond;
        }

        private class CondInfo
        {
            /// <summary>
            /// Gets or sets the condition ID.
            /// </summary>
            public int CondID { get; set; }

            /// <summary>
            ///  Gets or sets the condition Type.
            ///    Condition Type from ALM is either 1 or 2.
            ///    1 -> execute when finished
            ///    2 -> execute when passed.
            /// </summary>
            public int CondType { get; set; }
        }
    }
}
