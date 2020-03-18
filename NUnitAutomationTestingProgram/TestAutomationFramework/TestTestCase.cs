using AutomationTestingProgram;
using AutomationTestingProgram.AutomationFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAutomationTestingProgram.TestAutomationFramework
{
    class TestTestCase
    {
        private TestCase testCase;

        [SetUp]
        public void SetUp()
        {
            InformationObject.TestCaseData = new FakeTestCaseData();
            testCase = new TestCase();
        }

        [Test]
        public void TestSetUp()
        {
            testCase.SetUp();
            Assert.Pass("Test case set up should pass");
        }

        [Test]
        public void TestExistNext()
        {
            Assert.IsTrue(testCase.ExistNextTestStep(),"There exists an next test case.");
        }

        [Test]
        public void TestGetNext()
        {
            Assert.IsNotNull(testCase.GetNextTestStep(), "The next test step should not be null.");
        }

        [Test]
        public void TestShouldExecute()
        {
            Assert.IsTrue(testCase.ShouldExecute(),"Test Case should execute");
        }

        [Test]
        public void TestTearDown()
        {
            testCase.TestCaseStatus = new TestCaseStatus();
            testCase.TearDown();
            Assert.Pass("Tear Down should pass");
        }
    }
}
