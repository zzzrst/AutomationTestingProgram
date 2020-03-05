namespace AutomationTestingProgram.TestingData.DataDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;
    using TextInteractor;

    /// <summary>
    /// Not a good impentation to read test cases from text file.
    /// Only used for proof of concept.
    /// </summary>
    public class TextCaseData : ITestCaseData
    {
        /// <inheritdoc/>
        public string InformationLocation { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Txt";

        private List<string> FileData { get; set; } = new List<string>();

        private int FileIndex { get; set; } = 0;

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            return this.FileIndex < this.FileData.Count;
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            string testStepName = this.FileData[this.FileIndex];
            this.FileIndex++;

            return InformationObject.TestStepData.SetUpTestStep(testStepName);
        }

        /// <inheritdoc/>
        public void SetUp()
        {
        }

        /// <inheritdoc/>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            TextInteractor interactor = new TextInteractor(Path.ChangeExtension(Path.Combine(this.InformationLocation, testCaseName), ".txt"));
            interactor.Open();
            while (!interactor.FinishedReading())
            {
                this.FileData.Add(interactor.ReadLine());
            }

            interactor.Close();

            this.FileIndex = 0;

            ITestCase testCase = new TestCase()
            {
                Name = testCaseName,
                ShouldExecuteVariable = performAction,
            };
            return testCase;
        }
    }
}
