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
    /// Not a good implementation of the text step data.
    /// Used mainly for proof of concept.
    /// </summary>
    public class TextStepData : ITestStepData
    {
        /// <inheritdoc/>
        public string InformationLocation { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Txt";

        private List<string> FileData { get; set; } = new List<string>();

        /// <inheritdoc/>
        public void SetUp()
        {
            TextInteractor interactor = new TextInteractor(this.InformationLocation);
            interactor.Open();
            while (!interactor.FinishedReading())
            {
                this.FileData.Add(interactor.ReadLine());
            }

            interactor.Close();
        }

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            string[] testStepValue = null;
            string testStepObjectName = string.Empty;
            Dictionary<string, string> args = new Dictionary<string, string>();

            foreach (string line in this.FileData)
            {
                string[] values = line.Split(';');
                if (values[0].Equals(testStepName))
                {
                    testStepValue = values;
                }
            }

            if (testStepValue == null)
            {
                throw new Exception($"Test Set: {testStepName} not found.");
            }

            // get the object name.
            testStepObjectName = testStepValue[1];

            // parse the arguments.
            foreach (string arg in testStepValue[2].Split(','))
            {
                string[] value = arg.Split('=');
                args.Add(value[0],value[1]);
            }

            TestStep testStep = ReflectiveGetter.GetEnumerableOfType<TestStep>()
                .Find(x => x.Name.Equals(testStepObjectName));

            testStep.Name = testStepName;
            testStep.Arguments = args;

            return testStep;
        }
    }
}
