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
        /// Gets or sets the location to get the information from.
        /// </summary>
        public string InformationLocation { get; set; }

        /// <summary>
        /// Gets the name to be found by the reflective getter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets The next test case.
        /// </summary>
        /// <returns>The next test case to run.</returns>
        public ITestCase GetNextTestCase();

        /// <summary>
        /// Sees if there exist another test case.
        /// </summary>
        /// <returns>Returns true if there is another test case.</returns>
        public bool ExistNextTestCase();
    }
}
