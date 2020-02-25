namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public interface ITestSetData
    {
        /// <summary>
        /// Gets The next test case.
        /// </summary>
        /// <returns>The next test case to run.</returns>
        public ITestCase GetNextTestCase();
    }
}
