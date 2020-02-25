namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The interface to get the test step data.
    /// </summary>
    public interface ITestStepData
    {
        /// <summary>
        /// Gets the arguments for the current test Step.
        /// </summary>
        /// <returns>The arguments for the current test step.</returns>
        public Dictionary<string, string> GetArguments();
    }
}
