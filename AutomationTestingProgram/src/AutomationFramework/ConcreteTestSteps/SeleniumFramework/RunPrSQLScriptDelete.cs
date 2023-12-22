// <copyright file="RunPrSQLScriptDelete.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestinProgram.Helper;
    using Microsoft.Extensions.Primitives;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to run a sql script from paramterized values.Only works for PR applications.
    /// This one of the more complicated test step as it relys on the test step to be a databaseData type object.
    /// note the following format
    /// define formCode = 'VARIABLEOVERWRITE1'
    /// define formYear = VARIABLEOVERWRITE2
    /// define orgCode = 'VARIABLEOVERWRITE3'
    /// define dateOfExecution = 'VARIABLEOVERWRITE4'
    /// </summary>
    public class RunPrSQLScriptDelete : TestStep
    {
        public override string Name { get; set; } = "RunPrSQLScriptDelete";

        public override void Execute()
        {
            base.Execute();

            // for runsql, max attempts should be always set as 1
            this.MaxAttempts = 1;

            string[] runArgs = this.Arguments["object"].Split(',');

            string environment = GetEnvironmentVariable(EnvVar.Environment);

            string scriptPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\scripts\\picasso_delete_template.sql";
            string tempScriptPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\scripts\\temp_del.sql";
            Logger.Info("Script path: " + scriptPath);

            // modify template with the new values
            string text = File.ReadAllText(scriptPath);
            text = text.Replace("VARIABLE_OVERWRITE_FORM_NAME", runArgs[0].Trim());
            text = text.Replace("VARIABLE_OVERWRITE_FORM_YEAR", runArgs[1].Trim());
            text = text.Replace("VARIABLE_OVERWRITE_ORG_CODE", runArgs[2].Trim());
            text = text.Replace("VARIABLE_OVERWRITE_DATE_TIME", DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss_fff"));

            File.WriteAllText(tempScriptPath, text);

            DatabaseStepData dbdata = new DatabaseStepData("");
            this.TestStepStatus.RunSuccessful = dbdata.ExecuteEnvironmentNonQuery(environment, tempScriptPath, true);
            this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful
                ? "Script has been successfully run."
                : "Exited with non-zero code. Something may have went wrong.";

            InformationObject.TestSetData.AddAttachment(tempScriptPath);

        }
    }
}
