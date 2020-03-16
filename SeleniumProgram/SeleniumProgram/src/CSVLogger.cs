// <copyright file="CSVLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Logger class for CSV.
    /// </summary>
    public class CSVLogger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSVLogger"/> class.
        /// </summary>
        /// <param name="csvSaveFileLocation">The location to save the csv file.</param>
        public CSVLogger(string csvSaveFileLocation)
        {
            this.CsvSaveFileLocation = csvSaveFileLocation;
        }

        /// <summary>
        /// Gets or sets the csv save file location.
        /// </summary>
        private string CsvSaveFileLocation { get; set; } = string.Empty;

        private List<string> Results { get; set; } = new List<string>();

        /// <summary>
        /// Saves the results to be written to the CSV.
        /// </summary>
        /// <param name="result"> The result to be written to the csv. </param>
        public void AddResults(string result)
        {
            this.Results.Add(result);

            // Logger.Info(result);
        }

        /// <summary>
        /// Writes to the CSV file for the accumulated results saved.
        /// </summary>
        public void WriteOutResults()
        {
            if (!File.Exists(this.CsvSaveFileLocation))
            {
                File.WriteAllLines(this.CsvSaveFileLocation, this.Results.ToArray());
            }
            else
            {
                // spinlock until file is ready to be written to.
                while (!IsFileReady(this.CsvSaveFileLocation))
                {
                }

                int fileColumnCount = File.ReadLines(this.CsvSaveFileLocation).First().Split(',').Count();
                int fileCurrentLength = File.ReadLines(this.CsvSaveFileLocation).Count();

                List<string> fileRuntime = File.ReadAllLines(this.CsvSaveFileLocation).ToList();

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

                File.WriteAllLines(this.CsvSaveFileLocation, fileRuntime.ToArray());
            }
        }

        /// <summary>
        /// Checks if the file is ready before attempting to write to it.
        /// </summary>
        /// <param name="filename">The name of the file to write to.</param>
        /// <returns><code>true</code> if the file is ready. </returns>
        private static bool IsFileReady(string filename)
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
    }
}
