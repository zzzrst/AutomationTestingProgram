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
        /// Gets or sets the arguments for the test data. Often the file location.
        /// You need to create a constructor that passes in a string/
        /// This is where you get the test args.
        /// </summary>
        public string TestArgs { get; set; }

        /// <summary>
        /// Gets the name to be found by the reflective getter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Since The constructor is called through reflective getter,
        /// any errors during the constructor caused by wrong arguments,
        /// will cause the program to crash. run here instead,
        /// guarantees that it is the right type.
        /// </summary>
        public void SetUp();
    }
}
