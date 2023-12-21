// <copyright file="VerifyEmail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestinProgram.Helper;
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

            // we have to use the argument comment, not comments
            string emailPath = Path.Combine(this.Arguments["comment"], "Emails");
            string emailShortLIst = Path.Combine(this.Arguments["comment"], "EmailList.txt");

            string logFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log\\";

            // we should resolve the filepath first
            string expectedFilePath = this.Arguments["value"];

            expectedFilePath = FilePathResolver.Resolve(expectedFilePath);

            if (expectedFilePath.Contains("K:\\"))
            {
                string tempFileLoc = logFolder + "temp_email_compare.eml";

                File.Copy(expectedFilePath, tempFileLoc, true);
                expectedFilePath = tempFileLoc;

                Logger.Info("Successfully copied to new file on local disk: " + expectedFilePath);
            }

            Logger.Info("Comparing email from: " + expectedFilePath + " to file located at " + emailPath);

            string resultFilePath = logFolder + "\\compareEmailResult.txt";

            Logger.Info("CSV logger location: " + InformationObject.CSVLogger);
            Logger.Info("CSV logger location: " + InformationObject.LogSaveFileLocation);
            Logger.Info("Result file path is: " + resultFilePath);

            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string powershellFile = Path.Combine(executingPath, "scripts", "compareEmail.ps1");

            string command = $"powershell -Executionpolicy Bypass \"&'{powershellFile}' " +
                             $"-emailPath '{emailPath}' " +
                             $"-ExpectedPath '{expectedFilePath}' " +
                             $"-emailShortList '{emailShortLIst}' " +
                             $"-resultFilePath '{resultFilePath}' \"";

            this.TestStepStatus.RunSuccessful = this.RunProcess(command) == 0 && !File.Exists(resultFilePath);
            this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? "Email Matched" : "Email did not match. Please check desktop for results.";

            // attach email attachment log if it fails
            if(File.Exists(resultFilePath))
            {
                InformationObject.TestSetData.AddAttachment(resultFilePath);
            }

            Logger.Info(this.TestStepStatus.Actual);
        }

        private int RunProcess(string command)
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
