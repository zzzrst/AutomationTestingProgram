// <copyright file="CSVLogger.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram
{
    using Microsoft.TeamFoundation.Pipelines.WebApi;
    using NPOI.HPSF;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Configuration;

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
            string path_ex = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            this.CsvSaveFileLocation = path_ex + "\\" + csvSaveFileLocation;
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
            // here we will want to configure the files that we are not wanting to use

            if (!File.Exists(this.CsvSaveFileLocation))
            {
                File.WriteAllLines(this.CsvSaveFileLocation, this.Results.ToArray());
            }
            else
            {
                File.Delete(this.CsvSaveFileLocation);
                File.WriteAllLines(this.CsvSaveFileLocation, this.Results.ToArray());
            }

            // attach to DevOps if true
            if (ConfigurationManager.AppSettings["ReportToDevOps"] == "true")
            {
                InformationObject.Reporter.AddTestRunAttachment("CSV Run Report", this.CsvSaveFileLocation, "CSV-run-report.csv");
            }

            // here we will attach the CSV logger file to the Test Set Data
            InformationObject.TestSetData.AddAttachment(this.CsvSaveFileLocation);
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
