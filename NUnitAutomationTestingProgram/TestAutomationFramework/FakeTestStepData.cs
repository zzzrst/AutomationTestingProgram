using AutomationTestingProgram.AutomationFramework;
using AutomationTestingProgram.TestingData;
using AutomationTestSetFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAutomationTestingProgram.SampleTests
{
    class FakeTestStepData : ITestStepData
    {
        public string TestArgs { get; set; }
        public string Name { get; } = "FakeTestStepData";

        public void AddAttachment(string attachment)
        {
        }

        public void SetArguments(TestStep testStep)
        {
        }

        public void SetUp()
        {
        }

        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            return new TestStep();
        }
    }
}
