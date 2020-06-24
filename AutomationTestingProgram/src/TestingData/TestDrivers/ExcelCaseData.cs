// <copyright file="ExcelCaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System.Collections.Generic;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test case data.
    /// </summary>
    public class ExcelCaseData : ExcelData, ITestCaseData
    {
        private const int URLCOL = 0;
        private const int ACTIONCOL = 1;
        private const int XPATHCOL = 2;

        private Queue<ITestStep> testStepQueue = new Queue<ITestStep>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelCaseData"/> class.
        /// </summary>
        /// <param name="args">The arguments to be passed in.</param>
        public ExcelCaseData(string args)
            : base(args)
        {
        }

        private int ColIndex { get; set; }

        private int RowIndex { get; set; } = 1;

        /// <summary>
        /// Gets the next Test Step.
        /// </summary>
        /// <returns>The next Test Step.</returns>
        public ITestStep GetNextTestStep()
        {
            if (this.testStepQueue.Count == 0)
            {
                string url = this.TestSetSheet.GetRow(this.RowIndex).GetCell(URLCOL)?.ToString();
                string action = this.TestSetSheet.GetRow(this.RowIndex).GetCell(ACTIONCOL)?.ToString();
                string xpath = this.TestSetSheet.GetRow(this.RowIndex).GetCell(XPATHCOL)?.ToString();
                string permission = this.TestSetSheet.GetRow(this.RowIndex).GetCell(this.ColIndex)?.ToString();
                string value;
                TestStep testStep;

                this.CheckPageNavigation(url);
                value = this.GetValue(permission);
                try
                {
                    testStep = ReflectiveGetter.GetEnumerableOfType<TestStep>()
                        .Find(x => x.Name.Equals(action));
                    testStep.Arguments.Add("value", value);
                    testStep.Arguments.Add("comment", "xpath");
                    testStep.Arguments.Add("object", xpath);
                    this.testStepQueue.Enqueue(testStep);
                }
                catch (System.NullReferenceException)
                {
                    Logger.Error("Cannot Find Test Step" + action);
                }

                this.RowIndex++;
            }

            return (TestStep)this.testStepQueue.Dequeue();
        }

        /// <summary>
        /// Sees if there is a next test step. Usually needs to call the InformationObject.TestStepData.SetUpTestSet.
        /// </summary>
        /// <returns>Returns true if there is another test Step.</returns>
        public bool ExistNextTestStep()
        {
            return this.TestSetSheet.GetRow(this.RowIndex).GetCell(this.ColIndex).ToString() != null
                || this.testStepQueue.Count > 0;
        }

        /// <summary>
        /// Set up and returns the new test case.
        /// </summary>
        /// <param name="testCaseName">The name of the test case.</param>
        /// <param name="performAction">Determins if the test case should run.</param>
        /// <returns>The test Case to run.</returns>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            int index = 0;
            int.TryParse(testCaseName, out index);
            this.ColIndex = index;
            ITestCase testCase = new TestCase()
            {
                Name = this.TestSetSheet.GetRow(0).GetCell(this.ColIndex).ToString(),
            };

            return testCase;
        }

        /// <summary>
        /// Adds a test step if there is a url to navigate to.
        /// </summary>
        /// <param name="url">the url to navigate to.</param>
        private void CheckPageNavigation(string url)
        {
            if (url != string.Empty)
            {
                TestStep testStep = new OpenBrowser();
                testStep.Arguments.Add("value", url);
                this.testStepQueue.Enqueue(testStep);
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
