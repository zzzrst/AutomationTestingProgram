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
    class TestXMLCaseData
    {
        private string saveFileLocation;
        private string readFileLocation;
        private string logName;
        private string reportName;

        [SetUp]
        public void SetUp()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            saveFileLocation = $"{executingLocation}/SampleTests/Files";
            readFileLocation = $"{executingLocation}/SampleTests/XML/TestTestCase";
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
                Directory.Delete(saveFileLocation, true);
            }
        }

        [Test]
        public void TestDuplicateIDs()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseDuplicateId.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;

            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expeted to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expeted to have 1 test case");
            Assert.AreEqual(1, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 1 test set");
        }

        [Test]
        public void TestSimpleIf()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseSimpleIf.xml");

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

            testSet = buildTestSet("/TestCaseNestedIf.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
        }

        [Test]
        public void TestElseIf()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseElseIf.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
            Assert.AreEqual(1, countNotRanTestSteps(reporter), "Expected to have 1 not ran test steps");
        }

        [Test]
        public void TestElse()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseElse.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(4, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 4 test steps");
            Assert.AreEqual(1, countNotRanTestSteps(reporter), "Expected to have 1 not ran test steps");
        }

        [Test]
        public void TestCannotFindTestStep()
        {
            TestSet testSet;

            try
            {
                testSet = buildTestSet("/TestCaseMissingTestStep.xml");
                AutomationTestSetDriver.RunTestSet(testSet);
                InformationObject.Reporter.Report();
            }
            catch (Exception)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void TestRepeatForMultiple()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseRepeatMultiple.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
        }

        [Test]
        public void TestSameTestStep()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseSameTestStep.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;

            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
            Assert.AreEqual(2, reporter.TestCaseToTestSteps.Sum(x => x.Value.Count), "Expected to have 2 test steps");
        }

        [Test]
        public void TestUnknownNodeName()
        {
            TestSet testSet;
            Reporter reporter;

            testSet = buildTestSet("/TestCaseUnknownNodeName.xml");

            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            reporter = InformationObject.Reporter;
            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful, "Expected to pass");
            Assert.AreEqual(1, reporter.TestCaseStatuses.Count, "Expected to have 1 test case");
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
            Environment.SetEnvironmentVariable("respectRepeatFor", "true");
            Environment.SetEnvironmentVariable("respectRunAODAFlag", "true");

            InformationObject.SetUp();
            TestSetBuilder builder = new TestSetBuilder();
            TestAutomationBuilder automationBuilder = new TestAutomationBuilder();

            automationBuilder.Build();

            return builder.Build();
        }

        private int countNotRanTestSteps(Reporter reporter)
        {
            int count = 0;
            foreach (List<ITestStepStatus> list in reporter.TestCaseToTestSteps.Values)
            {
                foreach (ITestStepStatus status in list)
                {
                    if (status.Actual.Equals("N/A"))
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
