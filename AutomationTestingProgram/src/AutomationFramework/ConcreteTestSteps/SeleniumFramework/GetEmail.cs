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
        private string executingPath;
        private string emailPath;
        private string emailFolderLocation;
        private string username;
        private string password;

        /// <inheritdoc/>
        public override string Name { get; set; } = "GetEmail";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            this.executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.emailPath = Path.Combine(this.Arguments["comment"], "Emails");

            this.emailFolderLocation = ((DatabaseStepData)TestStepData).GetEnvironmentEmailNotificationFolder(GetEnvironmentVariable(EnvVar.Environment));
            this.username = ((DatabaseStepData)TestStepData).GetGlobalVariableValue("WINDOW ACCOUNT USERNAME");
            this.password = ((DatabaseStepData)TestStepData).GetGlobalVariableValue("WINDOW ACCOUNT PASSWORD");

            this.SaveEmailToFile();
            this.VerifyEmailExists();
        }

        private void SaveEmailToFile()
        {
            string powershellFile = Path.Combine(this.executingPath, "scripts", "getEmail.ps1");

            string command = $"powershell -Executionpolicy Bypass \"&'{powershellFile}' " +
                $"-emailSharedLocationPath '{this.emailFolderLocation}' " +
                $"-emailLocalPath '{this.emailPath}' " +
                $"-Username '{this.username}' " +
                $"-Password '{this.password}' \"";

            this.TestStepStatus.RunSuccessful = this.StartProcess(command) == 0;
        }

        private void VerifyEmailExists()
        {
            string emailShortList = Path.Combine(this.Arguments["comment"], "EmailList.txt");
            string powershellFile = Path.Combine(this.executingPath, "scripts", "getEmailShortList.ps1");
            string command = $"powershell -Executionpolicy Bypass \"&'{powershellFile}' " +
                $"-timeSpanValue '{this.Arguments["object"]}' " +
                $"-emailPath '{this.emailPath}' " +
                $"-subject '{this.Arguments["value"]}' " +
                $"-emailList '{emailShortList}' \"";

            this.StartProcess(command);
            this.TestStepStatus.RunSuccessful = File.Exists(emailShortList);
            this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? "At least 1 matching email was found." : "No emails were found";
        }

        private int StartProcess(string command)
        {
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
            return p.ExitCode;
        }
    }
}
