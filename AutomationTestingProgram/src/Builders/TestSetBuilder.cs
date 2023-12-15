// <copyright file="TestSetBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.AutomationFramework.Loggers_and_Reporters;
    using AutomationTestingProgram.Builders;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static InformationObject;

    /// <summary>
    /// Creates a new Information Object and returns it.
    /// </summary>
    public class TestSetBuilder
    {
        /// <summary>
        /// The usable places to get data.
        /// </summary>
        public enum TestingDataType
        {
            /// <summary>
            /// Application life cycle managment.
            /// </summary>
            Alm,

            /// <summary>
            /// From a SQL database.
            /// </summary>
            Sql,

            /// <summary>
            /// From an XML File.
            /// </summary>
            XML,

            /// <summary>
            /// From a txt file.
            /// </summary>
            Text,
        }

        /// <summary>
        /// Sets the values for the informaton object.
        /// </summary>
        /// <returns>The test Set to run.</returns>
        public TestSet Build()
        {
            TestSet testSet;

            this.InsantiateTestSetData();
            this.InsantiateTestCaseData();
            this.InsantiateTestStepData();

            testSet = new TestSet();

            return testSet;
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test StepData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestStepData()
        {
            string testStepDataType = GetEnvironmentVariable(EnvVar.TestStepDataType);
            string testStepDataArgs = GetEnvironmentVariable(EnvVar.TestStepDataArgs);
            InformationObject.TestStepData = (ITestStepData)this.GetTestData(2, testStepDataType, testStepDataArgs);
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test CaseData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestCaseData()
        {
            string testCaseDataType = GetEnvironmentVariable(EnvVar.TestCaseDataType);
            string testCaseDataArgs = GetEnvironmentVariable(EnvVar.TestCaseDataArgs);
            InformationObject.TestCaseData = (ITestCaseData)this.GetTestData(1, testCaseDataType, testCaseDataArgs);
        }

        /// <summary>
        /// Uses the ReflectiveGetter method to set the provided test SetData based on the name given.
        /// Name has to match the name variable in the driver.
        /// </summary>
        private void InsantiateTestSetData()
        {
            string testSetDataType = GetEnvironmentVariable(EnvVar.TestSetDataType);
            string testSetDataArgs = GetEnvironmentVariable(EnvVar.TestSetDataArgs);
            InformationObject.TestSetData = (ITestSetData)this.GetTestData(0, testSetDataType, testSetDataArgs);
            InformationObject.TestSetData.SetUpTestSet();
        }

        /// <summary>
        /// The actual method to get the test set/case/step data.
        /// </summary>
        /// <param name="testDataType"> 0 = testSetData, 1 = testCaseData, 2 = testStepData. </param>
        /// <param name="dataTypeName"> Name where to get the data from. </param>
        /// <param name="dataTypeLocation"> Arguments for the data. </param>
        /// <returns> The test data.</returns>
        private ITestData GetTestData(int testDataType, string dataTypeName, string dataTypeLocation)
        {
            ITestData testData = null;

            switch (testDataType)
            {
                case 0:
                    testData = ReflectiveGetter.GetImplementationOfType<ITestSetData>(dataTypeLocation)
                                .Find(x => x.Name.ToLower().Equals(dataTypeName.ToLower()));
                    break;
                case 1:
                    testData = ReflectiveGetter.GetImplementationOfType<ITestCaseData>(dataTypeLocation)
                                .Find(x => x.Name.ToLower().Equals(dataTypeName.ToLower()));
                    break;
                case 2:
                    testData = ReflectiveGetter.GetImplementationOfType<ITestStepData>(dataTypeLocation)
                                .Find(x => x.Name.ToLower().Equals(dataTypeName.ToLower()));
                    break;
                default:
                    throw new Exception("Not a valid testDataType");
            }

            if (testData == null)
            {
                Console.WriteLine($"Sorry we do not currently support reading tests from: {dataTypeName}");
                throw new Exception($"Cannot Find test data type {dataTypeName}");
            }

            testData.SetUp();

            return testData;
        }
    }
}
