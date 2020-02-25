namespace AutomationTestingProgram.TestingData.DataDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// A singelton of a textDriver.
    /// </summary>
    public class TextDriver : ITestCaseData, ITestSetData, ITestStepData
    {
        static TextDriver()
        {
        }

        private TextDriver()
        {
        }

        /// <summary>
        /// Gets the driver for the text driver.
        /// </summary>
        public static TextDriver Driver { get; } = new TextDriver();

        /// <inheritdoc/>
        public Dictionary<string, string> GetArguments()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            throw new NotImplementedException();
        }
    }
}
