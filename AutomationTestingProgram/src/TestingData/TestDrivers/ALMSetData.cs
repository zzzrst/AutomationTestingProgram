// <copyright file="ALMSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using ALMConnector;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.TestingData;
    using AutomationTestSetFramework;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// An implementation of the test set data using ALM.
    /// </summary>
    public class ALMSetData : ITestSetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ALMSetData"/> class.
        /// </summary>
        /// <param name="args">argument to be passed in.</param>
        public ALMSetData(string args)
        {
            this.TestIDName = args;
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "ALM";

        /// <summary>
        /// Gets the current Test Set Instance.
        /// </summary>
        public TestSetInstance TestSet { get; private set; }

        /// <summary>
        /// Gets or sets the ALM Connector.
        /// </summary>
        public Connector ALM { get; set; }

        public bool ContinueToRun { get; set; } = true;

        private string TestIDName { get; set; }

        /// <inheritdoc/>
        public void AddAttachment(string attachment)
        {
            this.TestSet.GetCurrentTestCaseName();
            this.TestSet.AddTestCaseAttachment("test File", attachment);
            this.TestSet.AddTestStepToTestCase("test", "Passed", "test description", "test expected", "test actual");
            this.TestSet.SetTestCaseRunStatus(true);
        }

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            this.ConnectToALM();
            return this.ContinueToRun;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            this.ConnectToALM();
            ITestCase testCase;
            string testCaseID = this.TestSet.GetCurrentTestCaseName();
            try
            {
                testCase = TestCaseData.SetUpTestCase(testCaseID, true);

                int testCondType = this.TestSet.GetCurrentTestCaseConditionType();
                //this.ContinueToRun = this.TestSet.MoveToNextTestCase();
            }
           catch (Exception e)
            {
                Logger.Error("Sorry, something went wrong during the creation of the test case." + testCaseID);
                // this.TestSet.AddTestStepToTestCase(testStepName, "Failed", testStepDesc, testStepExp, testStepAct);
                this.TestSet.SetTestCaseRunStatus("Blocked");

                // Set all the remaining test cases as blocked
                while (this.TestSet.MoveToNextTestCase())
                {
                    this.TestSet.SetTestCaseRunStatus("Blocked");
                }

                Logger.Error(e.InnerException);
                throw e;
            }

            return testCase;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            this.ConnectToALM();
        }

        /// <inheritdoc/>
        public void SetUpTestSet()
        {
            if (int.TryParse(this.TestIDName, out int uID))
            {
                this.TestSet = this.ALM.SetTestSetByUID(uID);
            }
            else
            {
                this.TestSet = this.ALM.SetTestSetByPath(this.TestIDName);
            }
        }

        /// <summary>
        /// Connects to ALM.
        /// </summary>
        public void ConnectToALM()
        {
            string username = ConfigurationManager.AppSettings["ALMusername"].ToString();
            string password = ConfigurationManager.AppSettings["ALMpassword"].ToString();
            string domain = ConfigurationManager.AppSettings["ALMdomain"].ToString();
            string project = ConfigurationManager.AppSettings["ALMproject"].ToString();

            if (this.ALM == null)
            {
                this.ALM = new Connector(username, password, domain, project);
            }
            else if (!this.ALM.IsConnected())
            {
                this.ALM.ConnectToServer();
            }
        }
    }
}
