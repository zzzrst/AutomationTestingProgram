// <copyright file="XMLStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.Helper;
    using AutomationTestSetFramework;

    /// <summary>
    /// The XML Driver to get data from an xml.
    /// </summary>
    public class XMLStepData : XMLData, ITestStepData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XMLStepData"/> class.
        /// An implementation of the TestStepData for xml.
        /// </summary>
        /// <param name="xmlLocation">The location of the xml file.</param>
        public XMLStepData(string xmlLocation)
            : base(xmlLocation)
        {
        }

        /// <inheritdoc/>
        public void SetArguments(TestStep testStep)
        {
        }

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            // get the list of testSteps
            XmlNode testSteps = this.XMLDocObj.GetElementsByTagName("TestSteps")[0];

            // Find the appropriate test steps
            foreach (XmlNode innerNode in testSteps.ChildNodes)
            {
                if (innerNode.Name != "#comment" && this.ReplaceIfToken(innerNode.Attributes["id"].Value, this.XMLDataFile) == testStepName)
                {
                    ITestStep testStep = this.BuildTestStep(innerNode, performAction);
                    return testStep;
                }
            }

            Logger.Warn($"Sorry, we didn't find a test step that matched the provided ID: {testStepName}");

            return null;
        }

        private ITestStep BuildTestStep(XmlNode testStepNode, bool performAction = true)
        {
            TestStep testStep = null;
            string name = this.ReplaceIfToken(testStepNode.Attributes["name"].Value, this.XMLDataFile);

            // initial value is respectRunAODAFlag
            // if we respect the flag, and it is not found, then default value is false.
            bool runAODA = InformationObject.RespectRunAODAFlag;
            runAODA = runAODA && bool.Parse(testStepNode.Attributes["runAODA"]?.Value ?? "false");

            // populate runAODAPageName. Deault is Not provided.
            string runAODAPageName = "Not provided.";
            if (runAODA)
            {
                runAODAPageName = this.ReplaceIfToken(testStepNode.Attributes["runAODAPageName"]?.Value ?? "Not provided.", this.XMLDataFile);
            }

            // log is true by default.
            bool log = true;
            if (testStepNode.Attributes["log"] != null)
            {
                log = bool.Parse(testStepNode.Attributes["log"].Value);
            }

            Logger.Debug($"Test step '{name}': runAODA->{runAODA} runAODAPageName->{runAODAPageName} log->{log}");

            testStep = ReflectiveGetter.GetEnumerableOfType<TestStep>()
                .Find(x => x.Name.Equals(testStepNode.Name));

            if (testStep == null)
            {
                Logger.Error($"Was not able to find the provided test action '{testStepNode}'.");
            }
            else
            {
                for (int index = 0; index < testStepNode.Attributes.Count; index++)
                {
                    testStepNode.Attributes[index].InnerText = this.ReplaceIfToken(testStepNode.Attributes[index].InnerText, this.XMLDataFile);
                    string key = Resource.Get(testStepNode.Attributes[index].Name);
                    if (key == null)
                    {
                        key = testStepNode.Attributes[index].Name;
                    }

                    testStep.Arguments.Add(key, testStepNode.Attributes[index].InnerText);
                }

                testStep.Name = name;
                testStep.ShouldLog = log;
                testStep.ShouldExecuteVariable = performAction;
                testStep.RunAODA = runAODA;
                testStep.RunAODAPageName = runAODAPageName;
            }

            return testStep;
        }
    }
}
