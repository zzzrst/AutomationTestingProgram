// <copyright file="ITestSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestSetFramework;

    /// <summary>
    /// The interface to get the test set data.
    /// </summary>
    public interface ITestSetData : ITestData
    {
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
