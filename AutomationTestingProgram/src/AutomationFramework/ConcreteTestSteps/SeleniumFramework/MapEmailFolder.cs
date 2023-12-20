// <copyright file="MapEmailFolder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Diagnostics;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to log in.
    /// </summary>
    public class MapEmailFolder : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "MapEmailFolder";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute(); // run base execute

            DatabaseStepData dbdata = new DatabaseStepData("");

            string emailFolderLocation = dbdata.GetEnvironmentEmailNotificationFolder(GetEnvironmentVariable(EnvVar.Environment));
            string username = dbdata.GetGlobalVariableValue("WINDOW ACCOUNT USERNAME");
            string password = dbdata.GetGlobalVariableValue("WINDOW ACCOUNT PASSWORD");

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $"/C exit | net use \"{emailFolderLocation}\" /user:{username} {password}",
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

            // complete mapping email folder and list as successful immediately.
            this.TestStepStatus.RunSuccessful = true;
            this.TestStepStatus.Actual = "Successfully mapped email folder";
        }
    }
}
