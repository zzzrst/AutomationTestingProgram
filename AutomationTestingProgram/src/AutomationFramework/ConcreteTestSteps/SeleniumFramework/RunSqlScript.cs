// <copyright file="RunSqlScript.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
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
        public override void Execute()
        {
            base.Execute();

            string scriptPath = this.Arguments["object"];
            string environment = GetEnvironmentVariable(EnvVar.Environment);
            Logger.Info($"Running script at '{scriptPath}'.");

            if ((TestStepData as DatabaseStepData) != null)
            {
                this.TestStepStatus.RunSuccessful = ((DatabaseStepData)TestStepData).ExecuteEnvironmentNonQuery(environment, scriptPath);
                this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful
                    ? "Script has been successfully run."
                    : "Exited with non-zero code. Something may have went wrong.";
            }
            else
            {
                this.TestStepStatus.Actual = "The test step data must be a database";
            }
        }
    }
}
