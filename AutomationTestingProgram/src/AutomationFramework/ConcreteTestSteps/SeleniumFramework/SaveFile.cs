// <copyright file="SaveFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.IO;
    using System.Threading;

    /// <summary>
    /// This test step to get the downloaded file and moves them to the given directory.
    /// </summary>
    public class SaveFile : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Save File";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string filePath = this.Arguments["value"];

            // This is for temp files (chrome is crdownload)
            bool notDownloadedYet = true;

            while (notDownloadedYet)
            {
                // Condition: C:\Temp folder is empty except for Archive Folder
                // search for any new files
                DirectoryInfo di = new DirectoryInfo(@"C:\Temp");
                FileInfo[] filesFound = di.GetFiles();

                switch (filesFound.Length)
                {
                    case 0:
                        this.TestStepStatus.RunSuccessful = false;
                        this.TestStepStatus.Actual = "No files were found in C:\\Temp.";
                        notDownloadedYet = false;
                        break;
                    case 1:
                        // there was only one downloaded file.
                        // we rename this file to be the name provided.
                        FileInfo latestDownloadedFile = filesFound[0];
                        string origName = @"C:\Temp\" + latestDownloadedFile.Name;

                        if (!origName.Contains("crdownload"))
                        {
                            latestDownloadedFile.MoveTo(filePath);
                            this.TestStepStatus.RunSuccessful = true;
                            this.TestStepStatus.Actual = $"Renamed flie {origName} to {filePath}.";
                            notDownloadedYet = false;
                        }
                        else
                        {
                            // Sleep for half a second
                            Thread.Sleep(500);
                        }

                        break;
                    default:
                        // we in trouble!
                        notDownloadedYet = false;
                        this.TestStepStatus.RunSuccessful = false;
                        this.TestStepStatus.Actual = $"Too many files";
                        break;
                }
            }
        }
    }
}
