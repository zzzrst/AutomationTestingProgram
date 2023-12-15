// <copyright file="TextGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Src.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestingProgram.GeneralData;
    using static AutomationTestingProgram.InformationObject;

    /// <inheritdoc/>
    public class TextGeneralData : ITestGeneralData
    {
        /// <inheritdoc/>
        public string Name { get; } = "Txt";

        /// <inheritdoc/>
        public Dictionary<EnvVar, string> ParseParameters(string testArgs, string dataFile)
        {
            return new Dictionary<EnvVar, string>();
        }

        /// <inheritdoc/>
        public bool Verify(string testArgs)
        {
            return true;
        }
    }
}
