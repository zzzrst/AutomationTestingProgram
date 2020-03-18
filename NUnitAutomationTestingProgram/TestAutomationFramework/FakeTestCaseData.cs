using AutomationTestingProgram.AutomationFramework;
using AutomationTestingProgram.TestingData;
using AutomationTestSetFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAutomationTestingProgram.TestAutomationFramework
{
    class FakeTestCaseData : ITestCaseData
    {
        public string TestArgs { get; set; }
        public string Name { get; } = "FakeTestCaseData";

        public bool ExistNextTestStep()
        {
            return true;
        }

        public ITestStep GetNextTestStep()
        {
            return new TestStep();
        }

        public void SetUp()
        {
        }

        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            return new TestCase();
        }
    }
}
