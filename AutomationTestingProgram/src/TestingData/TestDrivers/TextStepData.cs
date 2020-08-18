// <copyright file="TextStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;
    using TextInteractor;

    /// <summary>
    /// Not a good implementation of the text step data.
    /// Used mainly for proof of concept.
    /// </summary>
    public class TextStepData : ITestStepData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextStepData"/> class.
        /// The Implementation of TestStepData for text file.
        /// </summary>
        /// <param name="textLocation">the location of the text file.</param>
        public TextStepData(string textLocation)
        {
            this.TestArgs = textLocation;
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Txt";

        private List<string> FileData { get; set; } = new List<string>();

        /// <inheritdoc/>
        public void SetArguments(TestStep testStep)
        {
        }

        /// <inheritdoc/>
        public void SetUp()
        {
        }

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepFileLocation, bool performAction = true)
        {
            TextInteractor interactor = new TextInteractor(testStepFileLocation);
            Dictionary<string, string> args = new Dictionary<string, string>();

            interactor.Open();
            while (!interactor.FinishedReading())
            {
                this.FileData.Add(interactor.ReadLine());
            }

            interactor.Close();

            string[] values = this.FileData[0].Split(';');
            TestStep testStep = ReflectiveGetter.GetEnumerableOfType<TestStep>()
                .Find(x => x.Name.Equals(values[1]));

            testStep.Name = values[0];

            foreach (string arg in values[2].Split(','))
            {
                string[] value = arg.Split('=');
                args.Add(value[0], value[1]);
            }

            testStep.Arguments = args;

            return testStep;
        }
    }
}
