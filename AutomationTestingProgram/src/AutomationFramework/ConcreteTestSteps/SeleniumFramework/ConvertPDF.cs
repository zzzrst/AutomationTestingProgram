// <copyright file="ConvertPDF.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using ComparePDF;

    /// <summary>
    /// This test step to compare pdfs.
    /// </summary>
    public class ConvertPDF : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ConvertPDF";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string pdfPath = this.Arguments["object"];
            string cmdRun = this.Arguments["value"];
            string outputPath = this.Arguments["comments"];

            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string scriptPath = Path.Combine(executingPath, "scripts", "PDFConverter\\converter.bat");

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $"/C exit | \"{scriptPath}\" \"{pdfPath}\" \"{cmdRun}\" {outputPath}",
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
    }
}
