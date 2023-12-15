// <copyright file="TextData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The base class for Text data.
    /// </summary>
    public class TextData : ITestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextData"/> class.
        /// </summary>
        /// <param name="txtFileLocation">the location of the xml.</param>
        public TextData(string txtFileLocation)
        {
            this.TestArgs = txtFileLocation;
        }

        /// <inheritdoc/>
        public string Name { get; set; } = "Txt";

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <summary>
        /// Gets or sets text in file location provided by TestArgs.
        /// </summary>
        protected string TextFile { get; set; }

        /// <summary>
        /// Gets or sets test cases in a string array.
        /// </summary>
        protected string[] TestArr { get; set; }

        /// <summary>
        /// Gets or Sets positon in TestCaseArr.
        /// </summary>
        protected int TestArrIndex { get; set; }

        /// <inheritdoc/>
        public void SetUp()
        {
            if (File.Exists(this.TestArgs))
            {
                this.TestArr = File.ReadAllLines(this.TestArgs);
                this.TestArrIndex = 0;
            }
            else
            {
                Logger.Error("File Does Not Exist");
            }
        }
    }
}
