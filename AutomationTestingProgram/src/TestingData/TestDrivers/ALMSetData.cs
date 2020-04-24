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
    using AutomationTestingProgram.TestingData;
    using AutomationTestSetFramework;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// An implementation of the test set data using ALM.
    /// </summary>
    public class ALMSetData : ITestSetData
    {
        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "ALM";

        private int UID { get; set; }

        private Connector ALM { get; set; }

        private TestSetInstance TestSet { get; set; }

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
            return this.TestSet.TotalUndelivered > 0;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            string testCaseID = this.TestSet.GetCurrentTestCaseName();
            ITestCase testCase = TestCaseData.SetUpTestCase(testCaseID, true);
            this.TestSet.MoveToNextTestCase();
            return testCase;
        }

        /// <inheritdoc/>
        public void SetUp()
        {
            string username = ConfigurationManager.AppSettings["ALMusername"].ToString();
            string password = ConfigurationManager.AppSettings["ALMpassword"].ToString();
            string domain = ConfigurationManager.AppSettings["ALMdomain"].ToString();
            string project = ConfigurationManager.AppSettings["ALMproject"].ToString();

            this.ALM = new Connector(username, password, domain, project);
        }

        /// <inheritdoc/>
        public void SetUpTestSet()
        {
            this.TestSet = this.ALM.SetTestSetByUID(int.Parse(GetEnvironmentVariable(EnvVar.TestSetDataArgs)));
        }
    }
}
