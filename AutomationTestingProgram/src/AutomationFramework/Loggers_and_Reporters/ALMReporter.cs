// <copyright file="ALMReporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestSetFramework;

    /// <summary>
    /// The implementation of the reporter class.
    /// </summary>
    public class ALMReporter : Reporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ALMReporter"/> class.
        /// </summary>
        /// <param name="saveLocation">The location to save the file to.</param>
        public ALMReporter(string saveLocation)
            : base(saveLocation)
        {
        }

        /// <inheritdoc/>
        public override void AddTestCaseStatus(ITestCaseStatus testCaseStatus)
        {
            ALMSetData almConnecter = (ALMSetData)InformationObject.TestSetData;
            almConnecter.ConnectToALM();
            almConnecter.TestSet.SetTestCaseRunStatus(testCaseStatus.RunSuccessful);
            almConnecter.ContinueToRun = almConnecter.TestSet.MoveToNextTestCase();
        }

        /// <inheritdoc/>
        public override void AddTestSetStatus(ITestSetStatus testSetStatus)
        {
        }

        /// <inheritdoc/>
        public override void AddTestStepStatusToTestCase(ITestStepStatus testStepStatus, ITestCaseStatus testCaseStatus)
        {
            ALMSetData almConnecter = (ALMSetData)InformationObject.TestSetData;
            almConnecter.ConnectToALM();
            string testName = testStepStatus.Name;
            string testStatus = testStepStatus.RunSuccessful ? "Passed" : "Failed";
            string testDescription = testStepStatus.Description;
            string testExpected = testStepStatus.Expected;
            string testActual = testStepStatus.Actual;
            almConnecter.TestSet.AddTestStepToTestCase(testName, testStatus, testDescription, testExpected, testActual);
        }

        /// <inheritdoc/>
        public override void Report()
        {
            ALMSetData almConnecter = (ALMSetData)InformationObject.TestSetData;
            almConnecter.ConnectToALM();
            almConnecter.ALM.SendExecutionReport();
            almConnecter.ALM.DisconnectFromServer();
        }
    }
}
