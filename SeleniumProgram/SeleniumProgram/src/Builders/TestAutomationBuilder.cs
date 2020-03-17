// <copyright file="TestAutomationBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Builders
{
    using System;
    using AutomationTestingProgram.TestAutomationDriver;
    using static AutomationTestingProgram.TestAutomationDriver.ITestAutomationDriver;

    /// <summary>
    /// Builds a new selenium Driver based on the given variables.
    /// </summary>
    public class TestAutomationBuilder
    {
        /// <summary>
        /// The usable testing applications.
        /// </summary>
        public enum TestingDriverType
        {
            /// <summary>
            /// Selenium program.
            /// </summary>
            Selenium,
        }

        /// <summary>
        /// Builds a new test automation driver.
        /// </summary>
        public void Build()
        {
            string testingDriver = Environment.GetEnvironmentVariable("testAutomationDriver");
            ITestAutomationDriver automationDriver = ReflectiveGetter.GetImplementationOfType<ITestAutomationDriver>()
                                .Find(x => x.Name.Equals(testingDriver));
            if (automationDriver == null)
            {
                Logger.Error($"Sorry we do not currently support the testing application: {testingDriver}");
            }
            else
            {
                automationDriver.SetUp();
            }

            InformationObject.TestAutomationDriver = automationDriver;
        }
    }
}
