using AutomationTestingProgram;
using AutomationTestingProgram.AutomationFramework;
using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
using AutomationTestingProgram.Builders;
using AutomationTestSetFramework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NUnitAutomationTestingProgram.TestTestingData
{
    class TestXMLStepData
    {
        private string saveFileLocation;
        private string readFileLocation;
        private string webSiteLocation;
        private string logName;
        private string reportName;

        [SetUp]
        public void SetUp()
        {
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            saveFileLocation = $"{executingLocation}/SampleTests/Files";
            readFileLocation = $"{executingLocation}/SampleTests/XML/TestTestStep";
            webSiteLocation = $"{executingLocation}/SampleTests";
            logName = $"{executingLocation}/SeleniumPerfXML.log";
            reportName = "/Report.txt";
            // Removes all previous ran test results
            // If directory does not exist, don't even try   
            if (Directory.Exists(saveFileLocation))
            {
                if (File.Exists(logName))
                {
                    bool notDeleted = true;
                    do
                    {
                        try
                        {
                            File.Delete(logName);
                            notDeleted = false;
                        }
                        catch (IOException)
                        {
                        }
                    } while (notDeleted);
                }
                if (File.Exists(saveFileLocation + reportName))
                    File.Delete(saveFileLocation + reportName);
                Directory.Delete(saveFileLocation, true);
            }
        }

        [Test]
        public void TestFailTestStep()
        {
            TestSet testSet;

            testSet = buildTestSet("/TestFailTestStep.xml");
            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();

            Reporter reporter = InformationObject.Reporter;

            Assert.IsFalse(reporter.TestSetStatuses[0].RunSuccessful);
            Assert.IsFalse(reporter.TestCaseStatuses[0].RunSuccessful);
            Assert.IsFalse(reporter.TestCaseToTestSteps[reporter.TestCaseStatuses[0]][0].RunSuccessful);
        }

        [Test]
        public void TestWait()
        {
            TestSet testSet;

            testSet = buildTestSet("/TestWait.xml");

            var start = DateTime.Now;
            AutomationTestSetDriver.RunTestSet(testSet);
            InformationObject.Reporter.Report();
            var end = DateTime.Now;

            Reporter reporter = InformationObject.Reporter;

            Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful);
            Assert.IsTrue(reporter.TestCaseStatuses[0].RunSuccessful);
            Assert.AreEqual(5, (end.Second - start.Second),"It should of waited 5 seconds");
        }

        //These don't work on the work flow for some reason...
        //[Test]
        //public void TestLog()
        //{
        //    TestSet testSet;

        //    testSet = buildTestSet("/TestLog.xml");
        //    AutomationTestSetDriver.RunTestSet(testSet);
        //    InformationObject.Reporter.Report();

        //    Reporter reporter = InformationObject.Reporter;

        //    string tempLogName = $"{this.logName}.tmp";

        //    File.Copy(this.logName, tempLogName);

        //    string logFile;
        //    using (StreamReader reader = new StreamReader(tempLogName))
        //    {
        //        logFile = reader.ReadToEnd();
        //    }

        //    File.Delete(tempLogName);

        //    Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful);
        //    Assert.IsTrue(reporter.TestCaseStatuses[0].RunSuccessful);
        //    Assert.IsTrue(reporter.TestCaseToTestSteps[reporter.TestCaseStatuses[0]][0].RunSuccessful);
        //    Assert.IsTrue(logFile.Contains("Name:Logging"), "Log file should have teststep in it");
        //}

        //[Test]
        //public void TestNoLog()
        //{
        //    TestSet testSet;

        //    testSet = buildTestSet("/TestNoLog.xml");
        //    AutomationTestSetDriver.RunTestSet(testSet);
        //    InformationObject.Reporter.Report();

        //    Reporter reporter = InformationObject.Reporter;

        //    string tempLogName = $"{this.logName}.tmp";

        //    File.Copy(this.logName, tempLogName);

        //    string logFile;
        //    using (StreamReader reader = new StreamReader(tempLogName))
        //    {
        //        logFile = reader.ReadToEnd();
        //    }

        //    File.Delete(tempLogName);

        //    Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful);
        //    Assert.IsTrue(reporter.TestCaseStatuses[0].RunSuccessful);
        //    Assert.IsTrue(reporter.TestCaseToTestSteps[reporter.TestCaseStatuses[0]][0].RunSuccessful);
        //    Assert.IsFalse(logFile.Contains("Name:No logging"), "Log file should not have teststep in it");
        //}

        ///// <summary>
        ///// Test To see if AODA Works
        ///// Not ran automaticaly since it requires a web browser
        ///// </summary>
        //[Test]
        //public void TestAODA()
        //{
        //    TestSet testSet;
        //    Reporter reporter;

        //    testSet = buildTestSet("/TestOpenClose.xml", $"{webSiteLocation}/Google.html");
        //    AutomationTestSetDriver.RunTestSet(testSet);
        //    InformationObject.Reporter.Report();
        //    FrameworkDriver.RunAODA();

        //    reporter = InformationObject.Reporter;

        //    Assert.IsTrue(Directory.Exists(saveFileLocation));
        //    Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful);
        //}

        ///// <summary>
        ///// Tests all concrete test steps except:
        ///// Sign in: it is a combination of click element and populate element
        ///// Not ran automaticaly since it requires a web browser
        ///// </summary>
        //[Test]
        //public void TestAllConcreteTestSteps()
        //{
        //    TestSet testSet;

        //    testSet = buildTestSet("/TestAllConcreteSteps.xml");
        //    AutomationTestSetDriver.RunTestSet(testSet);
        //    InformationObject.Reporter.Report();

        //    Reporter reporter = InformationObject.Reporter;

        //    Assert.IsTrue(reporter.TestSetStatuses[0].RunSuccessful);
        //    Assert.IsTrue(reporter.TestCaseStatuses[0].RunSuccessful);
        //}

        private TestSet buildTestSet(string testFileName, string url = "testUrl")
        {
            Environment.SetEnvironmentVariable("browser", "chrome");
            Environment.SetEnvironmentVariable("environment", "");
            Environment.SetEnvironmentVariable("timeOutThreshold", "50");
            Environment.SetEnvironmentVariable("warningThreshold", "50");
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
    }
}
