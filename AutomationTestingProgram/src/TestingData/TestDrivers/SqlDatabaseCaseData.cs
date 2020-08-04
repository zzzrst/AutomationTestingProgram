// <copyright file="SqlDatabaseCaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// An implementation of the SQl Database Test step using the Original
    /// Database as a Base.
    /// </summary>
    public class SqlDatabaseCaseData : SqlDatabaseData, ITestCaseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseCaseData"/> class.
        /// </summary>
        /// <param name="args">args to be passed in.</param>
        public SqlDatabaseCaseData(string args)
            : base(args)
        {
        }

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            throw new NotImplementedException();
        }
    }
}
