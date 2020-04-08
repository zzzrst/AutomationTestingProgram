// <copyright file="VerifyEmail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class VerifyEmail : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyEmail";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string emailPath = Path.Combine(this.Arguments["comments"], "Emails");
            string emailShortLIst = Path.Combine(this.Arguments["comments"], "EmailList.txt");

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string resultFilePath = desktop + "\\compareEmailResult.txt";

            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string powershellFile = Path.Combine(executingPath, "scripts", "compareEmail.ps1");

            string command = $"powershell -Executionpolicy Bypass \"&'{powershellFile}' " +
                             $"-emailPath '{emailPath}' " +
                             $"-ExpectedPath '{this.Arguments["value"]}' " +
                             $"-emailShortList '{emailShortLIst}' " +
                             $"-patternToRemove '{this.Arguments["object"]}' " +
                             $"-resultFilePath '{resultFilePath}' \"";

            try
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
                this.TestStepStatus.RunSuccessful = p.ExitCode == 0 && !File.Exists(resultFilePath);
                this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? "Email Matched" : "Email did not match. Please check desktop for results.";
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
