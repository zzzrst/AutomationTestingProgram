// <copyright file="TestCaseInstance.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ALMConnector
{
    using System;
    using System.IO;
    using System.Linq;
    using TDAPIOLELib;

    /// <summary>
    /// A class to represent a test case instance on ALM.
    /// </summary>
    public class TestCaseInstance
    {
        /// <summary>
        /// An array of all possible statuses that the test instance can be set to on ALM.
        /// </summary>
        private static readonly string[] STATUS = new string[] { "No Run", "Blocked", "Failed", "N/A", "Not Completed", "Passed", "Undelivered" };

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseInstance"/> class.
        /// </summary>
        /// <param name="test">The test<see cref="ITSTest"/>.</param>
        /// <param name="buildNumber"> The build number for this test run. </param>
        /// <param name="environment"> The environmnet the test is run on.</param>
        public TestCaseInstance(ITSTest test, string environment, string buildNumber)
        {
            this.Test = test;
            this.Name = test.Name.Substring(test.Name.LastIndexOf("]") + 1); // remove the test case instance number
            this.StartTestRun(environment, buildNumber);
        }

        /// <summary>
        /// Gets or sets the Test.
        /// </summary>
        public ITSTest Test { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the TestStatus.
        /// </summary>
        public string TestStatus { get; private set; } = "Blocked";

        /// <summary>
        /// Gets the test run's unique id.
        /// </summary>
        public string RunID => $"{this.TestRun.ID}";

        /// <summary>
        /// Gets the test run's unique name.
        /// </summary>
        public string RunName => this.TestRun.Name;

        /// <summary>
        /// Gets or sets the varible has attachment.
        /// </summary>
        public bool HasAttachment { get; set; } = false;

        /// <summary>
        /// Gets or sets the TestRun.
        /// </summary>
        private Run TestRun { get; set; }

        /// <summary>
        /// Works when file path is on the desktop / c drive, but not on other drives....
        /// </summary>
        /// <param name="description"> Description of the attached file.</param>
        /// <param name="filePath"> Filepath to the file.</param>
        /// <returns>The server file name for the attachment.</returns>
        public string AddAttachment(string description, string filePath)
        {
            try
            {
                AttachmentFactory attachmentFactory = this.TestRun.Attachments as AttachmentFactory;
                attachmentFactory.NewList(string.Empty);
                Attachment attachment = attachmentFactory.AddItem(DBNull.Value) as Attachment;
                attachment.Description = description;
                attachment.Type = 1;
                attachment.FileName = filePath;
                attachment.Post();
                attachment.Refresh();
                attachment.Save(false);

                // We set that this test case instance has an attachment.
                this.HasAttachment = true;
                return attachment.ServerFileName;
            }
            catch (Exception e)
            {
                e.ToString();
                throw new CannotAddTestCaseAttachment(CannotAddTestCaseAttachment.ErrorMsg + $"({this.Name}) - {description} and {filePath}");
            }
        }

        /// <summary>
        /// The AddTestStep.
        /// </summary>
        /// <param name="testStepName">The testStepName<see cref="string"/>.</param>
        /// <param name="testStepStatus">The testStepStatus<see cref="string"/>.</param>
        /// <param name="testStepDescription">The testStepDescription<see cref="string"/>.</param>
        /// <param name="testStepExpected">The testStepExpected<see cref="string"/>.</param>
        /// <param name="testStepActual">The testStepActual<see cref="string"/>.</param>
        public void AddTestStep(string testStepName, string testStepStatus, string testStepDescription, string testStepExpected, string testStepActual)
        {
            try
            {
                StepFactory stepFactory = this.TestRun.StepFactory as StepFactory;
                Step step = stepFactory.AddItem("Selenium Automated Tests") as Step;
                step["ST_STEP_NAME"] = testStepName;
                step["ST_STATUS"] = testStepStatus;
                step["ST_DESCRIPTION"] = testStepDescription;
                step["ST_EXPECTED"] = testStepExpected;
                step["ST_ACTUAL"] = testStepActual;
                step["ST_EXECUTION_DATE"] = DateTime.Now;
                step["ST_EXECUTION_TIME"] = DateTime.Now;
                step.Post();
            }
            catch (Exception)
            {
                throw new CannotAddTestStep(CannotAddTestStep.ErrorMsg + $"({this.Name}) {testStepName} -> {testStepStatus}");
            }
        }

        /// <summary>
        /// The SetTestRunStatus.
        /// </summary>
        /// <param name="testStatus">The testStatus<see cref="string"/>.</param>
        public void SetTestRunStatus(string testStatus)
        {
            try
            {
                if (STATUS.Contains(testStatus))
                {
                    this.TestStatus = testStatus;

                    this.TestRun["RN_STATUS"] = testStatus;
                }
                else
                {
                    this.TestRun["RN_STATUS"] = "Blocked";
                }

                this.TestRun.Post();
            }
            catch (NullReferenceException)
            {
                throw new NoTestRunStarted(NoTestRunStarted.ErrorMsg);
            }
            catch (Exception)
            {
                throw new CannotPostTestRun(CannotPostTestRun.ErrorMsg + $"({this.TestRun.Name}) - {testStatus}");
            }
        }

        /// <summary>
        /// The SetTestRunStatus.
        /// </summary>
        /// <param name="testPassed">The testPassed<see cref="bool"/>.</param>
        public void SetTestRunStatus(bool testPassed)
        {
            if (testPassed)
            {
                this.SetTestRunStatus("Passed");
            }
            else
            {
                this.SetTestRunStatus("Failed");
            }
        }

        /// <summary>
        /// Returns the field value.
        /// </summary>
        /// <param name="fieldName">Name of the field to retrieve.</param>
        /// <returns>The value stored in this field. </returns>
        public string GetField(string fieldName)
        {
            var value = this.Test[fieldName];
            if (value != null)
            {
                return value.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="fieldName">name of the field to retrieve.</param>
        /// <param name="value">The value stored in this field.</param>
        public void SetField(string fieldName, object value)
        {
            this.Test.UnLockObject();
            this.Test[fieldName] = value;
            this.Test.Post();
            this.Test.UnLockObject();
        }

        /// <summary>
        /// The StartTestRun.
        /// </summary>
        private void StartTestRun(string environment, string buildNumber)
        {
            try
            {
                RunFactory testRunFactory = this.Test.RunFactory as RunFactory;
                this.TestRun = testRunFactory.AddItem($"{environment}_{buildNumber}_{testRunFactory.UniqueRunName}") as Run;
            }
            catch (Exception)
            {
                throw new CannotStartTestRun(CannotStartTestRun.ErrorMsg + this.Name);
            }
        }
    }
}
