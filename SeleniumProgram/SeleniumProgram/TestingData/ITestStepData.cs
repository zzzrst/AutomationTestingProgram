// <copyright file="ITestStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System.Collections.Generic;
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test step data.
    /// </summary>
    public interface ITestStepData : ITestData
    {
        /// <summary>
        /// Runs when getting the test step from the test case.
        /// </summary>
        /// <param name="testStepName">The name of the test Step.</param>
        /// <param name="shouldPerform">Determins if the test step should run.</param>
        /// <returns>The Test Step to run.</returns>
        public ITestStep SetUpTestStep(string testStepName, bool shouldPerform);
    }
}
