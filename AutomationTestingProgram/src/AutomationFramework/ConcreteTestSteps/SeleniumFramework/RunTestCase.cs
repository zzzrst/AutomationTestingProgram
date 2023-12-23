// <copyright file="RunTestCase.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestSetFramework;

    /// <summary>
    /// Class to represent test action 'RunTestCase'.
    /// Executes the test action 'RunTestCase'
    /// and executing the test case.
    /// Testcase must be within the test case data.
    /// </summary>
    public class RunTestCase : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "RunTestCase";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string testCaseName = this.Arguments["value"];

            ITestCase testCase = InformationObject.TestCaseData.SetUpTestCase(testCaseName);

            Logger.Info("Trying to execute RunTestCase, which is currently not configured in Excel");
            Logger.Warn("Run Test Case is not configured for Excel executions");

            Logger.Info("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
            AutomationTestSetDriver.RunTestCase(testCase);
            Logger.Info("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            this.TestStepStatus.Actual = testCase.TestCaseStatus.Actual;
            this.TestStepStatus.RunSuccessful = testCase.TestCaseStatus.RunSuccessful;
            this.TestStepStatus.Description = testCase.TestCaseStatus.Description;
            this.TestStepStatus.Expected = testCase.TestCaseStatus.Expected;
            this.TestStepStatus.ErrorStack = testCase.TestCaseStatus.ErrorStack;
        }
    }
}
