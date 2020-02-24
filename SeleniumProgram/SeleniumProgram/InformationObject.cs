// <copyright file="InformationObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingDriver;

    /// <summary>
    /// An information class that contains information needed by other objects/methods.
    /// </summary>
    public static class InformationObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether to respect the repeat for value.
        /// </summary>
        public static bool RespectRepeatFor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to run AODA.
        /// </summary>
        public static bool RespectRunAODAFlag { get; set; }

        /// <summary>
        /// Gets or sets the location to save the log.
        /// </summary>
        public static string LogSaveFileLocation { get; set; }

        /// <summary>
        /// Gets or sets the location to save the CSV file.
        /// </summary>
        public static CSVLogger CSVLogger { get; set; }

        /// <summary>
        /// Gets or sets the object to get the test set data from.
        /// </summary>
        public static ITestSetData TestSetData { get; set; }

        /// <summary>
        /// Gets or sets the object to get the test case data from.
        /// </summary>
        public static ITestCaseData TestCaseData { get; set; }

        /// <summary>
        /// Gets or sets the object to get the test step data from.
        /// </summary>
        public static ITestSetData TestStepData { get; set; }

        /// <summary>
        /// Gets or sets the testing driver to run the testing program on.
        /// </summary>
        public static ITestingDriver TestingDriver { get; set; }
    }
}
