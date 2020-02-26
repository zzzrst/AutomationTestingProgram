﻿namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test case data.
    /// </summary>
    public interface ITestCaseData
    {
        /// <summary>
        /// Gets or sets the location to get the information from.
        /// </summary>
        public string InformationLocation { get; set; }

        /// <summary>
        /// Gets the name to be found by the reflective getter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the driver for the test data.
        /// </summary>
        public static ITestCaseData Driver { get; }

        /// <summary>
        /// Gets the next Test Step.
        /// </summary>
        /// <returns>The next Test Step.</returns>
        public ITestStep GetNextTestStep();

        /// <summary>
        /// Sees if there is a next test step.
        /// </summary>
        /// <returns>Returns true if there is another test Step.</returns>
        public bool ExistNextTestStep();
    }
}
