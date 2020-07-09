// <copyright file="RunShellScript.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// This test step to run a shell script.
    /// </summary>
    public class RunShellScript : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "RunShellScript";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string cmdRun = this.Arguments["value"];

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $"/C exit | {cmdRun}",
            };
            p.StartInfo = startInfo;
            p.Start();
            Logger.Info($"{Logger.Tab(4)}Stdout:");
            string line;
            while ((line = p.StandardOutput.ReadLine()) != null)
            {
                Logger.Info($"{Logger.Tab(5)}{line}");
                this.TestStepStatus.Actual += "\n" + line;
            }

            p.WaitForExit();
        }
    }
}
