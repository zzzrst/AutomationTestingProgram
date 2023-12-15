﻿// <copyright file="ExcelStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;
    using NPOI.SS.Formula.Atp;

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
        }
    }
}
