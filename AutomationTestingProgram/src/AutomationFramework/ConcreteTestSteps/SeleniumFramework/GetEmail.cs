// <copyright file="GetEmail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to get an email. Must be using a database test step.
    /// </summary>
    public class GetEmail : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "GetEmail";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string emailFolderLocation = string.Empty;
            string username = string.Empty;
            string password = string.Empty;

            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string emailPath = Path.Combine(this.Arguments["comment"], "Emails");

            emailFolderLocation = ((DatabaseStepData)TestStepData).GetEnvironmentEmailNotificationFolder(GetEnvironmentVariable(EnvVar.Environment));
            username = ((DatabaseStepData)TestStepData).GetGlobalVariableValue("WINDOW ACCOUNT USERNAME");
            password = ((DatabaseStepData)TestStepData).GetGlobalVariableValue("WINDOW ACCOUNT PASSWORD");

            try
            {
                string powershellFile = Path.Combine(executingPath, "scripts", "getEmail.ps1");

                string command = $"powershell -Executionpolicy Bypass \"&'{powershellFile}' " +
                    $"-emailSharedLocationPath '{emailFolderLocation}' " +
                    $"-emailLocalPath '{emailPath}' " +
                    $"-Username '{username}' " +
                    $"-Password '{password}' \"";

                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe",
                    Arguments = $"/C exit | {command}",
                };
                p.StartInfo = startInfo;
                p.Start();
                Logger.LogStdout();
                string line;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    Logger.LogWithFiveTabs(line);
                    this.TestStepStatus.Actual += "\n" + line;
                }

                p.WaitForExit();
                this.TestStepStatus.RunSuccessful = p.ExitCode == 0;
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                this.TestStepStatus.RunSuccessful = false;
            }

            try
            {
                string emailShortList = Path.Combine(this.Arguments["comment"], "EmailList.txt");
                string powershellFile = Path.Combine(executingPath, "scripts", "getEmailShortList.ps1");
                string command = $"powershell -Executionpolicy Bypass \"&'{powershellFile}' " +
                    $"-timeSpanValue '{this.Arguments["object"]}' " +
                    $"-emailPath '{emailPath}' " +
                    $"-subject '{this.Arguments["value"]}' " +
                    $"-emailList '{emailShortList}' \"";

                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe",
                    Arguments = $"/C exit | {command}",
                };
                p.StartInfo = startInfo;
                p.Start();
                Logger.LogStdout();
                string line;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    Logger.LogWithFiveTabs(line);
                    this.TestStepStatus.Actual += "\n" + line;
                }

                p.WaitForExit();
                this.TestStepStatus.RunSuccessful = File.Exists(emailShortList);
                this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? "At least 1 matching email was found." : "No emails were found";
                Logger.Info(this.TestStepStatus.Actual);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                this.TestStepStatus.RunSuccessful = false;
            }
        }
    }
}
