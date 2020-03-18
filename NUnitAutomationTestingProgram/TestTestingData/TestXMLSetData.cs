using AutomationTestingProgram;
using AutomationTestingProgram.AutomationFramework;
using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
using AutomationTestingProgram.Builders;
using AutomationTestSetFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NUnitAutomationTestingProgram.TestTestingData
{
    class TestXMLSetData
    {
        private string saveFileLocation;
        private string readFileLocation;
        private string logName;
        private string reportName;

        [SetUp]
        public void Setup()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            saveFileLocation = $"{executingLocation}/SampleTests/Files";
            readFileLocation = $"{executingLocation}/SampleTests/XML/TestTestCaseFlow";
            logName = "/Log.txt";
            reportName = "/Report.txt";

            // Removes all previous ran test results
            // If directory does not exist, don't even try   
            if (Directory.Exists(saveFileLocation))
            {
                if (File.Exists(saveFileLocation + logName))
                    File.Delete(saveFileLocation + logName);
                if (File.Exists(saveFileLocation + reportName))
                    File.Delete(saveFileLocation + reportName);
            }
        }

        [Test]
        public void TestBareMinimum()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestBareMinimum.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;

            Assert.Pass();
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expeted to pass");
        }

        [Test]
        public void TestDuplicateIDs()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetDuplicateId.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;

            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expeted to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expeted to have 1 test case");
            Assert.AreEqual(1, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 1 test set");
        }

        [Test]
        public void TestSameCaseRanTwice()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSameCaseRanTwice.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(2, reporter.TestCaseStatuses.Count, "Expected to have 2 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
        }

        [Test]
        public void TestSimpleIf()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetSimpleIf.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(1, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 1 test steps");
        }

        [Test]
        public void TestNestedIf()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetNestedIf.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(2, reporter.TestCaseStatuses.Count, "Expected to have 2 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
        }

        [Test]
        public void TestElseIf()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetElseIf.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(2, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(1, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 1 test steps");
        }

        [Test]
        public void TestElse()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetElse.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(4, reporter.TestCaseStatuses.Count, "Expected to have 4 test case");
            Assert.AreEqual(3, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 3 test steps");
        }

        [Test]
        public void TestCannotFindTestCase()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetMissingTestCase.xml");

            try
            {
                AutomationTestSetDriver.RunTestSet(testSet);
            }
            catch (Exception)
            {
            }

            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsFalse(reporter.TestSetStatuses[0].RunSuccessful, "Expected to fail");
            Assert.AreEqual(0, reporter.TestCaseStatuses.Count, "Expected to have 0 test case");
            Assert.AreEqual(0, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 0 test steps");
        }

        [Test]
        public void UnknownNodeName()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestSetUnknownNodeName.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(2, reporter.TestCaseStatuses.Count, "Expected to have 2 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
        }

        private TestSet buildTestSet(string testFileName, string url = "testUrl")
        {
            Environment.SetEnvironmentVariable("browser", "chrome");
            Environment.SetEnvironmentVariable("environment", "");
            Environment.SetEnvironmentVariable("timeOutThreshold", "0");
            Environment.SetEnvironmentVariable("warningThreshold", "0");
            Environment.SetEnvironmentVariable("url", url);
            Environment.SetEnvironmentVariable("dataFile", $"{readFileLocation}{testFileName}");
            Environment.SetEnvironmentVariable("csvSaveFileLocation", saveFileLocation);
            Environment.SetEnvironmentVariable("logSaveFileLocation", saveFileLocation);
            Environment.SetEnvironmentVariable("reportSaveFileLocation", saveFileLocation);
            Environment.SetEnvironmentVariable("screenshotSaveLocation", saveFileLocation);
            Environment.SetEnvironmentVariable("testAutomationDriver", "selenium");
            Environment.SetEnvironmentVariable("testSetDataType", "XML");
            Environment.SetEnvironmentVariable("testSetDataArgs", $"{readFileLocation}{testFileName}");
            Environment.SetEnvironmentVariable("testCaseDataType", Environment.GetEnvironmentVariable("testSetDataType"));
            Environment.SetEnvironmentVariable("testStepDataType", Environment.GetEnvironmentVariable("testCaseDataType"));
            Environment.SetEnvironmentVariable("testCaseDataArgs", Environment.GetEnvironmentVariable("testSetDataArgs"));
            Environment.SetEnvironmentVariable("testStepDataArgs", Environment.GetEnvironmentVariable("testCaseDataArgs"));
            Environment.SetEnvironmentVariable("respectRepeatFor", "false");
            Environment.SetEnvironmentVariable("respectRunAODAFlag", "false");

            InformationObject.SetUp();
            TestSetBuilder builder = new TestSetBuilder();
            TestAutomationBuilder automationBuilder = new TestAutomationBuilder();

            automationBuilder.Build();

            return builder.Build();
        }
    }
}
