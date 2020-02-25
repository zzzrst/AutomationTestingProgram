namespace AutomationTestingProgram.TestingData
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
        /// Gets the next Test Step.
        /// </summary>
        /// <returns>The next Test Step.</returns>
        public ITestStep GetNextTestStep();
    }
}
