// <copyright file="NoGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Implemention of the general data for a no test set.
    /// </summary>
    public class NoGeneralData : ITestGeneralData
    {
        /// <inheritdoc/>
        public string Name { get; } = "None";

        /// <inheritdoc/>
        public Dictionary<InformationObject.EnvVar, string> ParseParameters(string testArgs, string dataFile)
        {
            return new Dictionary<InformationObject.EnvVar, string>();
        }

        /// <inheritdoc/>
        public bool Verify(string testArgs)
        {
            return true;
        }
    }
}
