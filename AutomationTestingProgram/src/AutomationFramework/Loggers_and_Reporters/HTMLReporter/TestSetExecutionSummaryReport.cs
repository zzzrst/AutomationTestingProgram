// <copyright file="TestSetExecutionSummaryReport.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Management;
    using System.Reflection;
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// TestSetExecutionSummaryReport : Used to store information and generate the HTML Report.
    /// </summary>
    public class TestSetExecutionSummaryReport
    {
        private const string SUMMARYTEMPLATE = "DevOpsTestSetExecutionSummaryTemplate.html";
        private const string TCEXECTEMPLATE = "DevOpsTestCaseExecutionTemplate_";
        private const string TSEXECTEMPLATE = "DevOpsTestStepExecutionTemplate_";

        private string templateFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ReportTemplates\\";

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSetExecutionSummaryReport"/> class.
        /// </summary>
        public TestSetExecutionSummaryReport()
        {
        }

        /// <summary>
        /// Gets or sets the connector used to get the test set.
        /// </summary>
        public TestSetHTMLInstance TestSet { get; set; }

        /// <summary>
        /// Generates the appropriate HTML Report.
        /// </summary>
        /// <returns>HTML report.</returns>
        public string GenerateHTMLReport(List<TestCaseHTML> testCaseResults, Dictionary<string, string> testSetDetails)
        {
            // Get the TestSetExecutionSummaryTemplate & Fill up all the information.
            string htmlReport = this.GenerateTestSetExecutionBody(testSetDetails);
            htmlReport = this.GenerateExecutedTestsTable(htmlReport, testCaseResults);

            return htmlReport;
        }

        private string GenerateTestSetExecutionBody(Dictionary<string, string> testSetDetails)
        {
            string htmlTemplateFilePath = this.templateFolderPath + SUMMARYTEMPLATE;
            string htmlTemplate = File.ReadAllText(htmlTemplateFilePath);

            htmlTemplate = this.ReplaceTestSetBasicInfoTable1(htmlTemplate);
            htmlTemplate = this.ReplaceTestSetBasicInfoTable2(htmlTemplate);
            htmlTemplate = this.ReplaceMachineSettings(htmlTemplate);
            htmlTemplate = this.ReplaceEnvironmentSetings(htmlTemplate, testSetDetails);

            return htmlTemplate;
        }

        private string ReplaceTestSetBasicInfoTable1(string htmlTemplate)
        {
            // Replace token with value
            htmlTemplate = htmlTemplate.Replace("{TEST_PLAN_NAME}", this.TestSet.PlanName);
            htmlTemplate = htmlTemplate.Replace("{PROJECT_NAME}", this.TestSet.Project);
            htmlTemplate = htmlTemplate.Replace("{ORG_NAME}", this.TestSet.OrgName);
            htmlTemplate = htmlTemplate.Replace("{STARTED_TIME}", $"{this.TestSet.Started:MM/dd/yyyy hh:mm:ss tt}");
            htmlTemplate = htmlTemplate.Replace("{END_TIME}", $"{this.TestSet.Finished:MM/dd/yyyy hh:mm:ss tt}");

            // Calculate Duration
            TimeSpan duration = this.TestSet.Finished - this.TestSet.Started;
            string reportDuration = duration.Hours > 0 ? $"{duration.Hours} h " : string.Empty;
            reportDuration += $"{duration.Minutes} min ";
            reportDuration += $"{duration.Seconds} sec";
            htmlTemplate = htmlTemplate.Replace("{DURATION}", reportDuration);
            htmlTemplate = htmlTemplate.Replace("{EXECUTION_URL}", this.TestSet.ExecutionURL);
            return htmlTemplate;
        }

        private string ReplaceTestSetBasicInfoTable2(string htmlTemplate)
        {
            // Replace Test Set ID ({TEST_SET_ID}) - {TEST_SET_NAME} Table Values
            htmlTemplate = htmlTemplate.Replace("{TEST_SET_ID}", this.TestSet.ID);
            htmlTemplate = htmlTemplate.Replace("{TEST_SET_NAME}", this.TestSet.Name);
            htmlTemplate = htmlTemplate.Replace("{TEST_SET_FOLDER}", this.TestSet.FolderPath);
            htmlTemplate = htmlTemplate.Replace("{TEST_RUN_ID}", this.TestSet.RunID);
            htmlTemplate = htmlTemplate.Replace("{TEST_PLAN_ID}", this.TestSet.PlanID);
            htmlTemplate = htmlTemplate.Replace("{FRAMEWORK_TYPE}", "Selenium");
            htmlTemplate = htmlTemplate.Replace("{EXECUTION_FORMAT}", this.TestSet.TestSetExecutionFormat);
            htmlTemplate = htmlTemplate.Replace("{TARGET_CYCLE}", this.TestSet.TargetCycle);
            htmlTemplate = htmlTemplate.Replace("{BASELINE}", this.TestSet.Baseline);

            // Replace with test set description
            htmlTemplate = htmlTemplate.Replace("{TEST_SET_DESCRIPTION}", this.TestSet.Description);

            return htmlTemplate;
        }

        private string ReplaceMachineSettings(string htmlTemplate)
        {
            OperatingSystem os_info = Environment.OSVersion;

            htmlTemplate = htmlTemplate.Replace("{OS_BUILD_NUMBER}", $"Build {os_info.Version.Build}");
            htmlTemplate = htmlTemplate.Replace("{OPERATING_SYSTEM}", this.GetOSInfo());
            htmlTemplate = htmlTemplate.Replace("{OS_Service_Pack}", os_info.ServicePack);
            htmlTemplate = htmlTemplate.Replace("{PROCESSOR}", $"{Environment.ProcessorCount}");
            htmlTemplate = htmlTemplate.Replace("{FREE_SPACE_ON_C_DRIVE}", this.GetTotalFreeSpace("C:\\"));

            htmlTemplate = htmlTemplate.Replace("{MACHINE_NAME}", Environment.MachineName);
            htmlTemplate = htmlTemplate.Replace("{IS_64_BIT}", Environment.Is64BitOperatingSystem.ToString());
            htmlTemplate = htmlTemplate.Replace("{PROCESS_PATH}", Environment.ProcessPath);
            htmlTemplate = htmlTemplate.Replace("{USER_DOMAIN}", Environment.UserDomainName);
            htmlTemplate = htmlTemplate.Replace("{USER_NAME}", Environment.UserName);
            htmlTemplate = htmlTemplate.Replace("{IS_USER_INTERACTIVE}", Environment.UserInteractive.ToString());

            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();
            double res = 0;
            double fres = 0;

            // There should be only one result
            foreach (ManagementObject result in results)
            {
                res = Convert.ToDouble(result["TotalVisibleMemorySize"]);
                fres = Math.Round((res / (1024 * 1024)), 2);
            }

            htmlTemplate = htmlTemplate.Replace("{TOTAL_RAM}", $"{fres} GB");

            return htmlTemplate;
        }

        private string GetOSInfo()
        {
            // Get Operating system information.
            OperatingSystem os = Environment.OSVersion;

            // Get version information about the os.
            Version vs = os.Version;

            // Variable to hold our return value
            string operatingSystem = string.Empty;

            if (os.Platform == PlatformID.Win32Windows)
            {
                // This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                        {
                            operatingSystem = "98SE";
                        }
                        else
                        {
                            operatingSystem = "98";
                        }

                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                        {
                            operatingSystem = "2000";
                        }
                        else
                        {
                            operatingSystem = "XP";
                        }

                        break;
                    case 6:
                        if (vs.Minor == 0)
                        {
                            operatingSystem = "Vista";
                        }
                        else if (vs.Minor == 1)
                        {
                            operatingSystem = "7";
                        }
                        else if (vs.Minor == 2)
                        {
                            operatingSystem = "8";
                        }
                        else
                        {
                            operatingSystem = "8.1";
                        }

                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }

            return $"Windows {operatingSystem}";
        }

        private string GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return $"{drive.TotalFreeSpace / ((1024 * 1024) * 1024)} GB";
                }
            }

            return string.Empty;
        }

        private string ReplaceEnvironmentSetings(string htmlTemplate, Dictionary<string, string> testSetDetails)
        {
            htmlTemplate = htmlTemplate.Replace("{APPLICATION}", testSetDetails.GetValueOrDefault("Application") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{APPLICATION_COLLECTION}", testSetDetails.GetValueOrDefault("Application Collection") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{TEST_ENVIRONMENT}", testSetDetails.GetValueOrDefault("Test Environment") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{BUILD_NUMBER}", testSetDetails.GetValueOrDefault("Build Number") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{AUTOMATION_LIBRARY_VERSION}", testSetDetails.GetValueOrDefault("Test Framework Version") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{BROWSER_VERSION}", testSetDetails.GetValueOrDefault("Browser") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{GLOBAL_ATTEMPTS}", testSetDetails.GetValueOrDefault("Global Attempts") ?? string.Empty);
            htmlTemplate = htmlTemplate.Replace("{GLOBAL_TIMEOUT}", testSetDetails.GetValueOrDefault("Global TimeOut") ?? string.Empty);
            return htmlTemplate;
        }

        private string GenerateExecutedTestsTable(string htmlTemplate, List<TestCaseHTML> testCaseExecutionsList)
        {
            List<TestCaseHTML> testCaseExecutions = testCaseExecutionsList;
            string replacementString = "{Executed_Tests_Table_BODY}";

            foreach (TestCaseHTML tcExec in testCaseExecutions)
            {
                string tableBodyTemplateFilePath = this.templateFolderPath + TCEXECTEMPLATE + tcExec.Status + ".html";
                string tableBodyTemplate = File.ReadAllText(tableBodyTemplateFilePath);

                string tableStepAllTestSteps = string.Empty;

                // if the test case execution status is failed, then we will create a test step execution report
                if (tcExec.Status == "Failed")
                {
                    string tableStepTemplateFilePath;

                    // test step execution results
                    for (int i = 0; i < tcExec.TestSteps.Count; i++)
                    {
                        Logger.Info("Test Steps: " + tcExec.TestSteps[i][0] + tcExec.TestSteps[i][1]);

                        string tableStepTemplate;
                        tableStepTemplateFilePath = this.templateFolderPath + TSEXECTEMPLATE + tcExec.TestSteps[i][0] + ".html";
                        tableStepTemplate = File.ReadAllText(tableStepTemplateFilePath);

                        tableStepTemplate = tableStepTemplate.Replace("{STEP_NUMBER}", (i+1).ToString());
                        tableStepTemplate = tableStepTemplate.Replace("{EXECUTION_RESULT}", tcExec.TestSteps[i][1]);

                        tableStepAllTestSteps += tableStepTemplate;
                    }
                }

                // replace values for tableBody
                tableBodyTemplate = tableBodyTemplate.Replace("{ORG_NAME}", this.TestSet.OrgName);
                tableBodyTemplate = tableBodyTemplate.Replace("{TEST_CASE_ID}", tcExec.RunID);
                tableBodyTemplate = tableBodyTemplate.Replace("{PROJECT_NAME}", this.TestSet.Project);
                tableBodyTemplate = tableBodyTemplate.Replace("{TEST_RUN_ID}", this.TestSet.RunID);
                tableBodyTemplate = tableBodyTemplate.Replace("{TEST_STEP_RESULT}", tableStepAllTestSteps);
                tableBodyTemplate = tableBodyTemplate.Replace("{TEST_CASE_NAME}", tcExec.TestCaseName);
                tableBodyTemplate = tableBodyTemplate.Replace("{TESTER_NAME}", tcExec.TesterName);
                tableBodyTemplate = tableBodyTemplate.Replace("{EXEC_DATE}", $"{tcExec.ExecDateTime:MM/dd/yyyy}");
                tableBodyTemplate = tableBodyTemplate.Replace("{EXEC_TIME}", $"{tcExec.ExecDateTime:hh:mm:ss tt}");
                tableBodyTemplate = tableBodyTemplate.Replace("{HAS_SCREENSHOT}", $"{tcExec.HasScreenShot}");
                tableBodyTemplate = tableBodyTemplate.Replace("{NUMSTEPS}", tcExec.NumSteps.ToString());

                htmlTemplate = htmlTemplate.Replace(replacementString, tableBodyTemplate + replacementString);
            }

            htmlTemplate = htmlTemplate.Replace("{HOST_NAME}", ConfigurationManager.AppSettings["HOST_NAME"]);
            htmlTemplate = htmlTemplate.Replace("{TESTLABMODULE_ID}", ConfigurationManager.AppSettings["TEST_LAB_MOUDLE_ID"]);
            htmlTemplate = htmlTemplate.Replace(replacementString, string.Empty);

            // replace values for the total count
            htmlTemplate = htmlTemplate.Replace("{TOTAL_BLOCKED}", $"{this.TestSet.TotalBlocked}");
            htmlTemplate = htmlTemplate.Replace("{TOTAL_FAILED}", $"{this.TestSet.TotalFailed}");
            htmlTemplate = htmlTemplate.Replace("{TOTAL_NA}", $"{this.TestSet.TotalNA}");
            htmlTemplate = htmlTemplate.Replace("{TOTAL_NO_RUN}", $"{this.TestSet.TotalNoRun}");
            htmlTemplate = htmlTemplate.Replace("{TOTAL_NOT_COMPLETED}", $"{this.TestSet.TotalNotCompleted}");
            htmlTemplate = htmlTemplate.Replace("{TOTAL_PASSED}", $"{this.TestSet.TotalPassed}");
            htmlTemplate = htmlTemplate.Replace("{TOTAL_UNDELIVERED}", $"{this.TestSet.TotalUndelivered}");

            return htmlTemplate;
        }
    }
}
