// <copyright file="TestingDriverBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestingDriver
{
    using System;
    using System.Configuration;
    using static TestingDriver.ITestingDriver;

    /// <summary>
    /// Builds a new selenium Driver based on the given variables.
    /// </summary>
    public class TestingDriverBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestingDriverBuilder"/> class.
        /// Testing Driver builder object.
        /// </summary>
        /// <param name="type">What type of driver to use.</param>
        /// <param name="args">arguments to be passed in.</param>
        public TestingDriverBuilder(TestingDriverType type, params object[] args)
        {
            this.DriverType = type;
            this.Args = args;
        }

        /// <summary>
        /// Gets or sets the type of testing driver to use.
        /// </summary>
        private TestingDriverType DriverType { get; set; }

        private object[] Args { get; set; }

        /// <summary>
        /// Builds a new test automation driver.
        /// </summary>
        /// <returns>A New Testing Driver with the given parameters.</returns>
        public ITestingDriver Build()
        {
            SeleniumDriver driver = new SeleniumDriver();
            ITestingDriver automationDriver = ReflectiveGetter.GetImplementationOfType<ITestingDriver>(this.Args)
                                .Find(x => x.Name.Equals(this.DriverType));
            if (automationDriver == null)
            {
                Logger.Error($"Sorry we do not currently support the testing application: {this.DriverType.ToString()}");
            }

            return automationDriver;
        }
    }
}
