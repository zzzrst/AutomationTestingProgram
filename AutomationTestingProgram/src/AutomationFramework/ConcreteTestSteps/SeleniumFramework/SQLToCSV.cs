// <copyright file="SQLToCSV.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class SQLToCSV : TestStep
    {
        private string sqlQuery;
        private string csvPath;
        private string comment;
        private string environment;

        /// <inheritdoc/>
        public override string Name { get; set; } = "SQLToCSV";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            this.environment = GetEnvironmentVariable(EnvVar.Environment);
            this.sqlQuery = this.Arguments["object"];
            this.csvPath = this.Arguments["value"];
            this.comment = this.Arguments["comment"];

            if (this.comment.StartsWith("@") && !this.comment.ToLower().Contains("current"))
            {
                this.environment = this.comment.Substring(1);
            }

            DatabaseStepData dbdata = new DatabaseStepData("");

            // attempt to execute the SQL script
            List<List<object>> table = null;
            if (this.environment.ToLower() == ConfigurationManager.AppSettings["DBTestCaseDatabase"].ToString().ToLower())
            {
                table = dbdata.ProcessQADBSelectQuery(this.sqlQuery);
            }
            else
            {
                table = dbdata.ProcessEnvironmentSelectQuery(this.environment, this.sqlQuery);
            }

            string csv = string.Join("\n", table.Select(
                                                    x => string.Join(",", x.Select(
                                                                            y => y == null ? string.Empty : $"\"{y}\"").ToArray())).ToArray());
            if (File.Exists(this.csvPath))
            {
                File.Delete(this.csvPath);
            }

            File.WriteAllText(this.csvPath, csv);
            this.TestStepStatus.Actual = $"Sucessfully wrote sql (${table.Count} rows) to {this.csvPath}";
        }

        /// <inheritdoc/>
        public override void HandleException(Exception e)
        {
            base.HandleException(e);
            this.TestStepStatus.Actual += $"Something went wrong {e}";
            this.TestStepStatus.FriendlyErrorMessage = $"Tryed to execute {this.sqlQuery} in {this.environment} saving to {this.csvPath}.";
        }
    }
}
