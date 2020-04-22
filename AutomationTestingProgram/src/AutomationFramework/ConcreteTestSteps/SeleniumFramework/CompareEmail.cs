// <copyright file="CompareEmail.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestSetFramework;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to compare two emails.
    /// </summary>
    public class CompareEmail : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "CompareEmail";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string emailFolderLocation = string.Empty;

            emailFolderLocation = ((DatabaseStepData)TestStepData).GetEnvironmentEmailNotificationFolder(GetEnvironmentVariable(EnvVar.Environment));

            GetEmail ge = new GetEmail();
            AutomationTestSetDriver.RunTestStep(ge);
            ge.Execute();
            this.TestStepStatus = ge.TestStepStatus;
            if (this.TestStepStatus.RunSuccessful)
            {
                VerifyEmail ve = new VerifyEmail();
                AutomationTestSetDriver.RunTestStep(ve);
                this.TestStepStatus.RunSuccessful = ve.TestStepStatus.RunSuccessful;
                this.TestStepStatus.Actual += "\n" + ve.TestStepStatus.Actual;
            }
        }
    }
}
