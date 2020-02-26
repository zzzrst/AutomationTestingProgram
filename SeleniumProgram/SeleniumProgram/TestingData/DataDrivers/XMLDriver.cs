// <copyright file="XMLDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.DataDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// The XML Driver to get data from an xml.
    /// </summary>
    public class XMLDriver : ITestSetData, ITestCaseData, ITestStepData
    {
        static XMLDriver()
        {
        }

        private XMLDriver()
        {
        }

        /// <summary>
        /// Gets the driver for the text driver.
        /// </summary>
        public static XMLDriver Driver { get; } = new XMLDriver();

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            throw new NotImplementedException();
        }

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
