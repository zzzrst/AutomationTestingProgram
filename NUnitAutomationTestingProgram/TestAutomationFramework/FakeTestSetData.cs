using AutomationTestingProgram.AutomationFramework;
using AutomationTestingProgram.TestingData;
using AutomationTestSetFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAutomationTestingProgram.TestAutomationFramework
{
    class FakeTestSetData : ITestSetData
    {
        public string TestArgs { get; set; }
        public string Name { get; } = "FakeTestSetData";

        public void AddAttachment(string attachment)
        {
        }

        public bool ExistNextTestCase()
        {
            return true;
        }

        public ITestCase GetNextTestCase()
        {
            return new TestCase();
        }

        public void SetUp()
        {
        }

        public void SetUpTestSet()
        {
        }
    }
}
