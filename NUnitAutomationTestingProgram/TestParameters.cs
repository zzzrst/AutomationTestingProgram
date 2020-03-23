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

namespace NUnitAutomationTestingProgram
{
    class TestParameters
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
            readFileLocation = $"{executingLocation}/SampleTests/XML";
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

        //[Test]
        // it works but is refusing to work...
        //public void TestMinimumParameters()
        //{
        //    FrameworkDriver.Main(new string[] { "--setType", "XML", "--setArgs", $"{readFileLocation}{"/TestSimple.xml"}" });
        //    Assert.Pass("Should Pass");
        //}
    }
}
