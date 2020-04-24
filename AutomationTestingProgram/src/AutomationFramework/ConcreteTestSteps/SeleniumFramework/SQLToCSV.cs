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
        /// <inheritdoc/>
        public override string Name { get; set; } = "SQLToCSV";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            bool passed = false;
            string environment = GetEnvironmentVariable(EnvVar.Environment);

            string sqlQuery = this.Arguments["object"];
            string csvPath = this.Arguments["value"];
            string comment = this.Arguments["comment"];
            string message = string.Empty;

            // need to implement.
            if (comment.StartsWith("@"))
            {
                if (!comment.ToLower().Contains("current"))
                {
                    environment = comment.Substring(1);
                }
            }

            // attempt to execute the SQL script
            try
            {
                List<List<object>> table = null;
                if (environment.ToLower() == ConfigurationManager.AppSettings["DBTestCaseDatabase"].ToString().ToLower())
                {
                    table = ((DatabaseStepData)TestStepData).ProcessQADBSelectQuery(sqlQuery);
                }
                else
                {
                    table = ((DatabaseStepData)TestStepData).ProcessEnvironmentSelectQuery(environment, sqlQuery);
                }

                string csv = string.Join("\n", table.Select(
                                                     x => string.Join(",", x.Select(
                                                                             y => y == null ? string.Empty : $"\"{y}\"").ToArray())).ToArray());
                if (File.Exists(csvPath))
                {
                    File.Delete(csvPath);
                }

                File.WriteAllText(csvPath, csv);
                passed = true;
                message = $"Sucessfully wrote sql (${table.Count} rows) to {csvPath}";
            }
            catch (Exception ex)
            {
                message = $"Something went wrong {ex.ToString()}";
                this.TestStepStatus.FriendlyErrorMessage = $"Tryed to execute {sqlQuery} in {environment} saving to {csvPath}.";
            }

            this.TestStepStatus.ErrorStack = message;
            this.TestStepStatus.RunSuccessful = passed;
        }
    }
}
