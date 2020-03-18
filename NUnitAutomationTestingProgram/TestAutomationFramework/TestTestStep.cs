using AutomationTestingProgram;
using AutomationTestingProgram.AutomationFramework;
using NUnit.Framework;
using NUnitAutomationTestingProgram.SampleTests;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAutomationTestingProgram.TestAutomationFramework
{
    class TestTestStep
    {
        private TestStep testStep;

        [SetUp]
        public void SetUp()
        {
            InformationObject.TestStepData = new FakeTestStepData();
            testStep = new TestStep();
        }

        [Test]
        public void TestExecute()
        {
            testStep.Execute();
            Assert.Pass("Should execute");
        }

        [Test]
        public void TestSetUp()
        {
            testStep.SetUp();
            Assert.Pass("Set up should pass");
        }

        [Test]
        public void TestShouldExecute()
        {
            Assert.IsTrue(testStep.ShouldExecute(),"Test Step should execute");
        }

        [Test]
        public void TestTearDown()
        {
            testStep.TestStepStatus = new TestStepStatus();
            testStep.TearDown();
            Assert.Pass("TearDown Should pass");
        }
    }
}
