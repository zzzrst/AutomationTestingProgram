// <copyright file="ITestGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// General data regarding the test data type. This should be ran once at the beginning and never used again.
    /// </summary>
    public interface ITestGeneralData
    {
        /// <summary>
        /// Gets the name to be used when being identifed by the reflective getter.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Verifys if the argument is valid. The arguments are
        /// Often File location, so this checks if the file is valid.
        /// </summary>
        /// <param name="testArgs">Test Arguments.</param>
        /// <returns>True if valid.</returns>
        public bool Verify(string testArgs);

        /// <summary>
        /// Parses the parameters in the given testArgs.
        /// Gets parameters such as enviornment, url, etc.
        /// Only runs for the test set args.
        /// </summary>
        /// <param name="testArgs">Test Arguments.</param>
        /// <param name="dataFile">The location of the datafile if any.</param>
        /// <returns>The parsed parameters.</returns>
        public Dictionary<string, string> ParseParameters(string testArgs, string dataFile);
    }
}
