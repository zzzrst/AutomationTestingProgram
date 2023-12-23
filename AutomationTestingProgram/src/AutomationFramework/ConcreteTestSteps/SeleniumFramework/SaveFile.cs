// <copyright file="SaveFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Configuration;
    using System.IO;
    using System.Reflection;
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
            // get the name of the file ignoring the path
            string fileName = Path.GetFileName(filePath);
            // append it to our temp folder
            filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"], fileName);

            // This is for temp files (chrome is crdownload)
            bool notDownloadedYet = true;

            while (notDownloadedYet)
            {
                // Condition: Temp folder is empty except for Archive Folder
                // search for any new files

                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"]);
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] filesFound = di.GetFiles();

                switch (filesFound.Length)
                {
                    case 0:
                        this.TestStepStatus.RunSuccessful = false;
                        this.TestStepStatus.Actual = $"No files were found in {path}.";
                        notDownloadedYet = false;
                        break;
                    case 1:
                        // there was only one downloaded file.
                        // we rename this file to be the name provided.
                        FileInfo latestDownloadedFile = filesFound[0];
                        string origName = path + latestDownloadedFile.Name;

                        if (!origName.Contains("crdownload"))
                        {
                            latestDownloadedFile.MoveTo(filePath);
                            this.TestStepStatus.RunSuccessful = true;
                            this.TestStepStatus.Actual = $"Renamed file {origName} to {filePath}.";
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
                        this.TestStepStatus.Actual = $"Too many files, total: " + filesFound;
                        Logger.Warn($"Possible resolution: close all open files in {path}");
                        break;
                }
            }
        }
    }
}
