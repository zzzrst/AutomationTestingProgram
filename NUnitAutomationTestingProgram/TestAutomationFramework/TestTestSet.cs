using AutomationTestingProgram;
using AutomationTestingProgram.AutomationFramework;
using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitAutomationTestingProgram.TestAutomationFramework
{
    class TestTestSet
    {
        private TestSet testSet;

        [SetUp]
        public void SetUp()
        {
            InformationObject.TestSetData = new FakeTestSetData();
            testSet = new TestSet();
        }

        [Test]
        public void TestSetUp()
        {
            testSet.SetUp();
            Assert.Pass("Should pass Set up");
        }

        [Test]
        public void TestTearDown()
        {
            InformationObject.Reporter = new Reporter("");
            testSet.TestSetStatus = new TestSetStatus();
            testSet.TearDown();
            Assert.Pass("Should pass Tear Down");
        }

        [Test]
        public void TestExistNext()
        {
            Assert.IsTrue(testSet.ExistNextTestCase(), "There is a next Test case");
        }
        
        [Test]
        public void TestGetNext()
        {
            Assert.IsNotNull(testSet.GetNextTestCase(),"Test Case should not be null");
        }

        [Test]
        public void TestShouldExecute()
        {
            Assert.IsTrue(testSet.ShouldExecute(), "It should execute");
        }

    }
}
