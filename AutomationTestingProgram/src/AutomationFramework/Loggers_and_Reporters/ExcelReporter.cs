// <copyright file="ExcelReporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomationTestSetFramework;

    /// <summary>
    /// The implementation of the reporter class.
    /// </summary>
    public class ExcelReporter : Reporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelReporter"/> class.
        /// </summary>
        /// <param name="saveLocation">The location to save the file to.</param>
        public ExcelReporter(string saveLocation)
            : base(saveLocation)
        {
        }

        private List<string> Results { get; set; } = new List<string>();

        /// <inheritdoc/>
        public override void Report()
        {
            this.Results.Add($"SeleniumPerfXML Version,{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            this.Results.Add($"{this.TestSetStatuses[0].Name}," + string.Join(",", this.TestCaseStatuses.Select(x => x.Name)));
            foreach (ITestCaseStatus testCaseStatus in this.TestCaseStatuses)
            {
                int colIndex = 2;
                if (this.TestCaseToTestSteps.ContainsKey(testCaseStatus))
                {
                    // log the test steps.
                    foreach (ITestStepStatus testStepStatus in this.TestCaseToTestSteps[testCaseStatus])
                    {
                        if (colIndex >= this.Results.Count)
                        {
                            this.Results.Add($"{testStepStatus.Name},{testStepStatus.Expected}-{testStepStatus.RunSuccessful}");
                        }
                        else
                        {
                            this.Results[colIndex] += $",{testStepStatus.Expected}-{testStepStatus.RunSuccessful}";
                        }

                        colIndex++;
                    }
                }
            }

            this.WriteOutResults();
        }

        /// <summary>
        /// Writes to the CSV file for the accumulated results saved.
        /// </summary>
        private void WriteOutResults()
        {
            if (!File.Exists(this.SaveFileLocation))
            {
                File.WriteAllLines(this.SaveFileLocation, this.Results.ToArray());
            }
            else
            {
                // spinlock until file is ready to be written to.
                while (!this.IsFileReady(this.SaveFileLocation))
                {
                }

                int fileColumnCount = File.ReadLines(this.SaveFileLocation).First().Split(',').Count();
                int fileCurrentLength = File.ReadLines(this.SaveFileLocation).Count();

                List<string> fileRuntime = File.ReadAllLines(this.SaveFileLocation).ToList();

                for (int i = 0; i < fileCurrentLength; i++)
                {
                    if (i < this.Results.Count())
                    {
                        fileRuntime[i] += "," + this.Results[i].Split(',').Last();
                    }
                    else
                    {
                        fileRuntime[i] += string.Empty.PadRight(this.Results[0].Split(',').Count(), ',');
                    }
                }

                if (this.Results.Count() > fileCurrentLength)
                {
                    for (int i = fileCurrentLength; i < this.Results.Count(); i++)
                    {
                        fileRuntime.Add(this.Results[i].PadLeft(fileColumnCount + this.Results[i].Length, ','));
                    }
                }

                File.WriteAllLines(this.SaveFileLocation, fileRuntime.ToArray());
            }
        }

        /// <summary>
        /// Checks if the file is ready before attempting to write to it.
        /// </summary>
        /// <param name="filename">The name of the file to write to.</param>
        /// <returns><code>true</code> if the file is ready. </returns>
        private bool IsFileReady(string filename)
        {
            // https://stackoverflow.com/questions/1406808/wait-for-file-to-be-freed-by-process
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    return inputStream.Length > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string Tab(int indents = 1)
        {
            return string.Concat(Enumerable.Repeat("    ", indents));
        }
    }
}
