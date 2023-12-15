﻿// <copyright file="ITestCaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test case data.
    /// </summary>
    public interface ITestCaseData : ITestData
    {
        /// <summary>
        /// Gets the next Test Step.
        /// </summary>
        /// <returns>The next Test Step.</returns>
        public ITestStep GetNextTestStep();

        /// <summary>
        /// Sees if there is a next test step. Usually needs to call the InformationObject.TestStepData.SetUpTestSet.
        /// </summary>
        /// <returns>Returns true if there is another test Step.</returns>
        public bool ExistNextTestStep();

        /// <summary>
        /// Set up and returns the new test case.
        /// </summary>
        /// <param name="testCaseName">The name of the test case.</param>
        /// <param name="performAction">Determins if the test case should run.</param>
        /// <returns>The test Case to run.</returns>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true);
    }
}
