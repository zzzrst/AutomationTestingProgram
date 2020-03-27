// <copyright file="DatabaseStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// A dummy class that isn't used.
    /// </summary>
    internal class DatabaseStepData : ITestStepData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStepData"/> class.
        /// A mandatory constructor that won't do anything.
        /// </summary>
        /// <param name="args">args.</param>
        public DatabaseStepData(string args)
        {
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Database";

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            // this should never be ran.
            throw new NotImplementedException();
        }
    }
}
