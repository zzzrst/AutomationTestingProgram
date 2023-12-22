// <copyright file="ExcelStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestSetFramework;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// The interface to get the test step data.
    /// </summary>
    public class ExcelStepData : ExcelData, ITestStepData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelStepData"/> class.
        /// </summary>
        /// <param name="args">The arguments to be passed in.</param>
        public ExcelStepData(string args)
           : base(args)
        {
        }

        /// <summary>
        /// Runs when getting the test step from the test case.
        /// </summary>
        /// <param name="testStepName">The name of the test Step.</param>
        /// <param name="performAction">Determins if the test step should run.</param>
        /// <returns>The Test Step to run.</returns>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets any arguments at runtime.
        /// </summary>
        /// <param name="testStep">Test steps to get the arguments for.</param>
        public void SetArguments(TestStep testStep)
        {
            // run for at most number of
            // attempts, until test action passes
            // update object and value by querying special characters
            this.QueryObjectAndArguments(testStep);
        }

        /// <summary>
        /// If object identifier or arguments begin with special characters in ['!', '@', '##', '$$'], then
        /// they are replaced by their respective value in the database.
        /// </summary>
        /// <param name="testStep">The Test step to query arguments.</param>.
        public void QueryObjectAndArguments(TestStep testStep)
        {
            string environment = GetEnvironmentVariable(EnvVar.Environment);

            Dictionary<string, string> arguments = new Dictionary<string, string>();

            DatabaseStepData dbdata = new DatabaseStepData(environment);

            // only update these variables if the control is not #
            if (testStep.ShouldExecuteVariable == true)
            {
                // query to update each of the test action's values
                foreach (string key in testStep.Arguments.Keys)
                {
                    arguments.Add(key, dbdata.QuerySpecialChars(environment, testStep.Arguments[key]) as string);
                }

                testStep.Arguments = arguments;
            }
        }

    }
}
