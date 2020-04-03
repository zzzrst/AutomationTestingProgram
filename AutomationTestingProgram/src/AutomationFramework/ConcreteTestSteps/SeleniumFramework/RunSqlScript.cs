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

            bool passed = false;
            string scriptPath = this.Arguments["object"];
            string environment = GetEnvironmentVariable(EnvVar.Environment);
            string message = $"Running script at '{scriptPath}'.";

            Logger.Info(message);

            ITestStepData testStepData = TestStepData;

            if (testStepData.Name.ToLower() == "database")
            {
                passed = ((DatabaseStepData)testStepData).ExecuteEnvironmentNonQuery(environment, scriptPath);

                message = passed
                    ? "Script has been successfully run."
                    : "Exited with non-zero code. Something may have went wrong.";

                Logger.Info(message);
                this.TestStepStatus.Actual = message;
                this.TestStepStatus.RunSuccessful = passed;
            }
            else
            {
                this.TestStepStatus.Actual = "The test step data must be a database";
            }
        }
    }
}
