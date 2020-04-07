// <copyright file="NoSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;

    /// <summary>
    /// Skips the test set and goes staight to the test case.
    /// </summary>
    public class NoSetData : ITestSetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoSetData"/> class.
        /// </summary>
        /// <param name="args">args to be passed in.</param>
        public NoSetData(string args)
        {
            this.TestArgs = args;
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "None";

        private bool Ran { get; set; } = false;

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            return !this.Ran;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            this.Ran = true;
            return InformationObject.TestCaseData.SetUpTestCase(this.TestArgs);
        }

        /// <inheritdoc/>
        public void SetUp()
        {
        }

        /// <inheritdoc/>
        public void SetUpTestSet()
        {
        }
    }
}
