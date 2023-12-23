// <copyright file="RunSqlScript.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestinProgram.Helper;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to run a sql script. Test Step must be Database.
    /// This one of the more complicated test step as it relys on the test step to be a databaseData type object.
    /// </summary>
    public class RunSQLScript : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "RunSQLScript";

        /// <inheritdoc/>
        [RequiresAssemblyFiles()]
        public override void Execute()
        {
            base.Execute();

            // for runsql, max attempts should be always set as 1
            this.MaxAttempts = 1;

            string scriptPath = this.Arguments["object"];

            scriptPath = FilePathResolver.Resolve(scriptPath);

            string environment = GetEnvironmentVariable(EnvVar.Environment);
            Logger.Info($"Running script at '{scriptPath}'. Note that this runs SQL Plus");

            // there are known errors with running a sql script from any network drive. SQLPlus cannot work with network drives
            // therefore, we are going to use a temp folder in the executing directory to run the script
            if (scriptPath.Contains("csc.ad.gov.on.ca") || scriptPath.Contains("K:")) 
            {
                Logger.Info("Running a script from a network drive, copying to C drive");

                string tempScriptPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\scripts\\temp_sql_script.sql";

                // check if a file at this path already exists, if it does, then delete
                if (File.Exists(tempScriptPath))
                {
                    File.Delete(tempScriptPath);
                }

                File.Copy(scriptPath, tempScriptPath);

                // attatch original file path
                InformationObject.TestSetData.AddAttachment(scriptPath);

                scriptPath = tempScriptPath;
                Logger.Info("Script path is now: " + scriptPath);
            }

            DatabaseStepData dbdata = new DatabaseStepData("");
            this.TestStepStatus.RunSuccessful = dbdata.ExecuteEnvironmentNonQuery(environment, scriptPath, true);
            this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful
                ? "Script has been successfully run."
                : "Exited with non-zero code. Something may have went wrong.";

        }
    }
}
