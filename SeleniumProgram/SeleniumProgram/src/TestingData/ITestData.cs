// <copyright file="ITestData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    /// <summary>
    /// The interface to get the test case data.
    /// </summary>
    public interface ITestData
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
        /// Runs before running the main testing loop.
        /// Used To initilize all the data.
        /// </summary>
        public void SetUp();
    }
}
