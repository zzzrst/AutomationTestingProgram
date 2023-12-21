// <copyright file="TextSetData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;
    using AutomationTestSetFramework;
    using TestingDriver;

    /// <inheritdoc/>
    public class TextSetData : TextData, ITestSetData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSetData"/> class.
        /// </summary>
        /// <param name="txt"> file location of Text Set.</param>
        public TextSetData(string txt)
            : base(txt)
        {
        }

        /// <inheritdoc/>
        public void AddAttachment(string attachment)
        {
        }

        /// <summary>
        /// Checks if there is a next text case, returns true or false.
        /// </summary>
        /// <returns>True if next testcase exists, False if end of testCases. </returns>
        public bool ExistNextTestCase()
        {
            return this.TestArr.Length - 1 > this.TestArrIndex;
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            this.TestArrIndex += 1;
            return InformationObject.TestCaseData.SetUpTestCase(this.TestArr[this.TestArrIndex]);
        }

        /// <inheritdoc/>
        public void SetUpTestSet()
        {
        }

        /// <summary>
        /// Adds AODA Report log.
        /// </summary>
        /// <param name="attachment">the attachment to attach.</param>
        public void AddAODAReport(string attachment)
        {
        }

        /// <summary>
        /// Adds error screenshot log.
        /// </summary>
        /// <param name="attachment">the attachment to attach.</param>
        public void AddErrorScreenshot(string attachment)
        {
        }
    }
}
