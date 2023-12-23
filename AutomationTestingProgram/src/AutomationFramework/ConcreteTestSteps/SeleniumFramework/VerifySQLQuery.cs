﻿// <copyright file="VerifySQLQuery.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Data.Common;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to verify an sql query. Test Step Data must be a database.
    /// </summary>
    public class VerifySQLQuery : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifySQLQuery";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string environment = GetEnvironmentVariable(EnvVar.Environment);
            string expectedQuery = this.Arguments["object"];
            string toCheckQuery = this.Arguments["value"];

            // query values from database
            DatabaseStepData dbdata = new DatabaseStepData("");

            string dbObject = dbdata.QuerySpecialChars(environment, expectedQuery).ToString();
            string dbValue = dbdata.QuerySpecialChars(environment, toCheckQuery).ToString();

            // check if both values are equal
            this.TestStepStatus.Actual = (this.TestStepStatus.RunSuccessful = dbValue == dbObject)
                ? $"Both expected and actual value were: '{dbValue}'."
                : $"Expected value is: '{dbValue}'. SQLQuery value is: '{dbObject}'. They are not equal.";
        }
    }
}
