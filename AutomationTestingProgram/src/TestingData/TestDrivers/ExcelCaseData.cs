// <copyright file="ExcelCaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;
    using Microsoft.Extensions.Options;
    using NPOI.OpenXmlFormats.Dml;
    //using TDAPIOLELib;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// The interface to get the test case data.
    /// </summary>
    public class ExcelCaseData : ExcelData, ITestCaseData
    {
        //private const int URLCOL = 0;
        public const int TESTCASENAME = 0;
        public const int DESC = 1;
        private const int STEPNUM = 2;
        public const int ACTIONCOL = 3;
        public const int OBJECTCOL = 4;
        public const int VALUECOL = 5;
        public const int TYPECOL = 6;
        private const int RELEASE = 7;
        private const int LOCAL_ATTEMPTS = 8;
        private const int LOCAL_TIMEOUT = 9;
        public const int CONTROL = 10;
        private const int COLLECTION = 11;
        public const int TESTSTEPTYPE = 12;
        public const int GOTOSTEP = 13;

        //private const int MAXTESTSETS = 200;

        // these are from the DB example for test cases
        private const int MANDATORY = 1;

        private const int IMPORTANT = 2;

        private const int OPTIONAL = 3;

        private const int CONDITIONAL = 4;

        private const int INVERTEDMANDATORY = 5;

        private const int INVERTEDIMPORTANT = 6;

        private Dictionary<string, int> loops = new Dictionary<string, int>();

        public int NextTestStepPass { get; set; } = 0;

        private int NextTestStepFail { get; set; } = 0;

        public int GoToTestStep { get; set; } = -1;

        /// <summary>
        /// Gets the SKIP.
        /// </summary>
        private string SKIP { get; } = "#";

        /// <summary>
        /// Gets and sets the maximum loops for the test steps before failing
        /// </summary>
        private int MaxLoops { get; set; } = 0;

        private Queue<ITestStep> testStepQueue = new Queue<ITestStep>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCaseData"/> class.
        /// </summary>
        /// <param name="args">The arguments to be passed in.</param>
        public ExcelCaseData(string args)
            : base(args)
        {
            this.MaxLoops = int.Parse(ConfigurationManager.AppSettings["MaxLoops"]);
        }

        // test case name for determining when the test case is done
        private string TestCaseName { get; set; }

        // test case name for determining when the test case is done
        private TestStep testStepRef { get; set; }

        /// <summary>
        /// Gets the next Test Step.
        /// </summary>
        /// <returns>The next Test Step.</returns>
        public ITestStep GetNextTestStep()
        {
            int index = this.GetNextTestStepIndex();

            if (index > 0)
            {
            // we should do starting index plus change index
                ExcelData.RowIndex = ExcelData.TestCaseStartIndex + index - 1;
                // note that if the index is greater than the current test case, we should create a new test case
                Logger.Info("Test Case Start Index " + ExcelData.TestCaseStartIndex);
                Logger.Info("Index returned is " + index);
                Logger.Info("Next index is: " + ExcelData.RowIndex);

                // report whether the new test set sheet's row is not equal to the current test case name
                if (this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(TESTCASENAME)?.ToString().Trim() != this.TestCaseName)
                {
                    // log as an error for now, since we are not able to skip to a next test case
                    Logger.Error("GoToStep for test case name is for a new Test Case. Not implemented for new test case.");
                }
            }

            this.GoToTestStep = -1;

            if (this.testStepQueue.Count == 0)
            {
                // if this is a skip, then ignore everything else and continue
                string control = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(CONTROL)?.ToString().Trim() ?? string.Empty;

                // string url = this.TestSetSheet.GetRow(this.RowIndex).GetCell(URLCOL)?.ToString() ?? string.Empty;
                string action = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(ACTIONCOL)?.ToString().Trim() ?? string.Empty;
                string xpath = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(OBJECTCOL)?.ToString().Trim() ?? string.Empty;
                string description = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(DESC)?.ToString().Trim() ?? string.Empty;
                string value = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(VALUECOL)?.ToString().Trim() ?? string.Empty;
                string comment = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(TYPECOL)?.ToString().Trim() ?? string.Empty;
                string attempts = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(LOCAL_ATTEMPTS)?.ToString().Trim() ?? "0";
                string gotostep = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(GOTOSTEP)?.ToString().Trim() ?? string.Empty;
                string timeout = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(LOCAL_TIMEOUT)?.ToString().Trim() ?? "0";
                string testStepType = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(TESTSTEPTYPE)?.ToString().Trim() ?? "0";

                // assign current test case name
                this.TestCaseName = this.TestSetSheet.GetRow(ExcelData.RowIndex).GetCell(TESTCASENAME)?.ToString().Trim() ?? string.Empty;

                try
                {
                    // trim so that even if we misspell, there is no problem with finding the right action. 
                    this.testStepRef = ReflectiveGetter.GetEnumerableOfType<TestStep>()
                        .Find(x => x.Name.Replace(" ", string.Empty).ToUpper().Equals(action.Replace(" ", string.Empty).ToUpper()));

                    this.testStepRef.Arguments.Add("value", value);

                    // Victor: define the comment
                    this.testStepRef.Arguments.Add("comment", comment);

                    this.testStepRef.Arguments.Add("object", xpath);

                    // add test step description
                    this.testStepRef.Description = description;

                    this.testStepRef.ShouldExecuteVariable = control != this.SKIP;

                    this.testStepQueue.Enqueue(this.testStepRef);

                    int localAttempts = int.Parse(string.IsNullOrEmpty(attempts) ? "0" : attempts);
                    if (localAttempts == 0)
                    {
                        localAttempts = int.Parse(ConfigurationManager.AppSettings["ExcelLocalAttempts"]); // alm.GlobalAttempts
                    }

                    this.testStepRef.MaxAttempts = localAttempts;

                    // local timeout currently isn't implemented
                    int localTimeout = int.Parse(string.IsNullOrEmpty(timeout) ? "0" : timeout);

                    if (localTimeout == 0)
                    {
                        // take globally defined local timeout if not set already
                        localTimeout = int.Parse(ConfigurationManager.AppSettings["ExcelLocalTimeout"]); // alm.GlobalAttempts
                    }

                    this.testStepRef.LocalTimeout = localTimeout;

                    // Test Step Type 0 automatically changed to type 1
                    int testStepTypeId = int.Parse(string.IsNullOrEmpty(testStepType) ? "0" : testStepType);
                    if (testStepTypeId == 0)
                    {
                        testStepTypeId = 2; // IMPORTANT, not MANDATORY
                    }

                    if (testStepType != string.Empty)
                    {
                        switch (int.Parse(testStepType))
                        {
                            case MANDATORY:
                                // by default its manadatory settings.
                                this.testStepRef.ContinueOnError = false;
                                break;
                            case IMPORTANT:
                                // we need to separate the difference between test step type 1 and 2

                                // if this fails, we cannot fail the test case
                                this.testStepRef.Optional = true;

                                // pass condition set to true means that continue on error
                                this.testStepRef.ContinueOnError = true;
                                break;
                            case OPTIONAL:
                                this.testStepRef.Optional = true;
                                break;
                            case CONDITIONAL:

                                // here we implement loop functionality, which is a counter for the Excel Row number
                                if (this.loops.ContainsKey(ExcelData.RowIndex.ToString()))
                                {
                                    this.loops[ExcelData.RowIndex.ToString()] += 1;
                                    Console.WriteLine("Num loops: " + this.loops[ExcelData.RowIndex.ToString()]);

                                    // if the number of attempts is greater than the local attempts, then we will skip to the next test step below this one
                                    if (this.loops[ExcelData.RowIndex.ToString()] > this.MaxLoops)
                                    {
                                        this.testStepRef.Optional = true; // we will assign as an optional test case. If it fails, the test case does not fail too.
                                        Console.WriteLine("Max loop attempts reached, going to next test step");
                                        break;
                                    }
                                }
                                else
                                {
                                    // initialize the counter to 1
                                    this.loops.Add(ExcelData.RowIndex.ToString(), 1);
                                }

                                this.testStepRef.Optional = true; // we will assign as an optional test case. If it fails, the test case does not fail too.

                                var nextsteps = gotostep.Split(',').Select(int.Parse).ToList();

                                // we should always get just two values, otherwise raise error
                                if (nextsteps.Count != 2)
                                {
                                    Logger.Error("Format of GoToStep incorrect, not only 2 values");
                                }
                                else
                                {
                                    this.GoToTestStep = 1; // set go to test step as true
                                    this.NextTestStepPass = nextsteps[0];
                                    this.NextTestStepFail = nextsteps[1];
                                }

                                Logger.Info($"-----------------------------------Conditional step set to, set to pass, fail: {nextsteps[0]} {nextsteps[1]}");

                                break;
                            case INVERTEDIMPORTANT:
                                this.testStepRef.PassCondition = false; // default true
                                break;
                            case INVERTEDMANDATORY:
                                this.testStepRef.PassCondition = false;
                                break;
                            default: break;
                        }
                    }

                }
                catch (System.NullReferenceException)
                {
                    if (action == string.Empty)
                    {
                        Logger.Error("Missing Test Action!");
                    }
                    else
                    {
                        Logger.Error("Cannot Find Test Step: " + action);
                        Logger.Error("Possible resolution: verify ActionOnObject is correct");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }

                ExcelData.RowIndex++;
            }

            return (TestStep)this.testStepQueue.Dequeue();
        }

        /// <summary>
        /// Sees if there is a next test step. Usually needs to call the InformationObject.TestStepData.SetUpTestSet.
        /// </summary>
        /// <returns>Returns true if there is another test Step.</returns>
        public bool ExistNextTestStep()
        {
            // If GoToTestStep is 1, this means that it's a conditional step and another test step in this test case will execute. 
            if (this.GoToTestStep == 1)
            {
                return true;
            }

            // For debugging purposes
            // Logger.Info("Row index in ExistNextTestStep: " + ExcelData.RowIndex);
            // Logger.Info("GetCell in ExistNextTestStep: " + this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(ACTIONCOL)?.ToString());
            // Logger.Info("Test Step Queue in ExistNextTestStep: " + this.testStepQueue.Count);

            bool sameTestCase = true;

            if (this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(TESTCASENAME) == null)
            {
                Logger.Info("This is the last test step");
                return false;
            }

            // for debug purposes
            // Logger.Info("Old test case name: " + this.TestCaseName);
            // Logger.Info("New Test Case name: " + this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(TESTCASENAME).ToString());

            if (this.TestCaseName != null && this.TestCaseName != this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(TESTCASENAME).ToString())
            {
                this.TestCaseName = this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(TESTCASENAME).ToString();
                sameTestCase = false;
            }

            bool ret_val = (this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(ACTIONCOL) != null
                && this.TestSetSheet.GetRow(ExcelData.RowIndex)?.GetCell(ACTIONCOL)?.ToString() != string.Empty)
                || this.testStepQueue.Count > 0;

            return ret_val && sameTestCase;
        }

        /// <summary>
        /// Set up and returns the new test case.
        /// </summary>
        /// <param name="testCaseName">The name of the test case.</param>
        /// <param name="performAction">Determins if the test case should run.</param>
        /// <returns>The test Case to run.</returns>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            // we should set the go to test step to -1 for any new tests
            this.GoToTestStep = -1;

            ITestCase testCase = new TestCase()
            {
                Name = testCaseName,
            };

            ExcelData.TestCaseStartIndex = ExcelData.RowIndex;

            // for debugging
            // Logger.Info("Test Case Start Index set to: " + ExcelData.TestCaseStartIndex);

            this.TestCaseName = testCaseName;
            return testCase;
        }

        private int GetNextTestStepIndex()
        {
            if (this.GoToTestStep >= 0)
            {
                if (this.testStepRef.TestStepStatus.RunSuccessful)
                {
                    return this.NextTestStepPass;
                }

                return this.NextTestStepFail;
            }

            return 0;
        }

        /// <summary>
        /// Adds a test step if there is a url to navigate to.
        /// </summary>
        /// <param name="url">the url to navigate to.</param>
        private bool CheckPageNavigation(string url)
        {
            if (url != string.Empty)
            {
                // changed to LaunchBrowser by Victor since we don't want to use as much PerfXML as before.
                TestStep newTestStep = new LaunchBrowser();
                newTestStep.Arguments.Add("value", url);
                this.testStepQueue.Enqueue(newTestStep);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the element status value to check for.
        /// </summary>
        /// <param name="permission">The user's permission.</param>
        /// <returns>The value of the element status.</returns>
        private string GetValue(string permission)
        {
            string value;
            switch (permission)
            {
                case "write":
                    value = "enabled";
                    break;
                case "read":
                    value = "disabled";
                    break;
                case "none":
                    value = "does not exist";
                    break;
                default:
                    value = permission;
                    break;
            }

            return value;
        }
    }
}
