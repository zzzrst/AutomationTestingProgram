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
    public class XMLStepData : ITestStepData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XMLStepData"/> class.
        /// An implementation of the TestStepData for xml.
        /// </summary>
        /// <param name="xmlLocation">The location of the xml file.</param>
        public XMLStepData(string xmlLocation)
        {
            this.TestArgs = xmlLocation;

            if (File.Exists(this.TestArgs))
            {
                this.XMLDocObj = new XmlDocument();
                this.XMLDocObj.Load(this.TestArgs);
                this.TestFlow = this.XMLDocObj.GetElementsByTagName("TestCaseFlow")[0];

                string dataFile = Environment.GetEnvironmentVariable("dataFile");
                if (dataFile == string.Empty || dataFile == null)
                {
                    if (this.XMLDocObj.GetElementsByTagName("DataFile").Count > 0)
                    {
                        dataFile = this.XMLDocObj.GetElementsByTagName("DataFile")[0].InnerText;
                        if (File.Exists(dataFile))
                        {
                            this.XMLDataFile = new XmlDocument();
                            this.XMLDataFile.Load(dataFile);
                        }
                        else
                        {
                            Logger.Error("XML File could not be found!");
                        }
                    }
                }
            }
            else
            {
                Logger.Error("XML File could not be found!");
            }
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "XML";

        /// <summary>
        /// Gets or sets the information for the test set.
        /// </summary>
        private XmlNode TestFlow { get; set; }

        /// <summary>
        /// Gets or sets the xml file containing the XML Data.
        /// </summary>
        private XmlDocument XMLDataFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the xml file containing the test set/case/steps.
        /// </summary>
        private XmlDocument XMLDocObj { get; set; } = null;

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            // get the list of testSteps
            XmlNode testSteps = this.XMLDocObj.GetElementsByTagName("TestSteps")[0];

            // Find the appropriate test steps
            foreach (XmlNode innerNode in testSteps.ChildNodes)
            {
                if (innerNode.Name != "#comment" && XMLHelper.ReplaceIfToken(innerNode.Attributes["id"].Value, this.XMLDataFile) == testStepName)
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
            string name = XMLHelper.ReplaceIfToken(testStepNode.Attributes["name"].Value, this.XMLDataFile);

            // initial value is respectRunAODAFlag
            // if we respect the flag, and it is not found, then default value is false.
            bool runAODA = InformationObject.RespectRunAODAFlag;
            if (runAODA)
            {
                if (testStepNode.Attributes["runAODA"] != null)
                {
                    runAODA = bool.Parse(testStepNode.Attributes["runAODA"].Value);
                }
                else
                {
                    runAODA = false;
                }
            }

            // populate runAODAPageName. Deault is Not provided.
            string runAODAPageName = "Not provided.";
            if (runAODA)
            {
                if (testStepNode.Attributes["runAODAPageName"] != null)
                {
                    runAODAPageName = XMLHelper.ReplaceIfToken(testStepNode.Attributes["runAODAPageName"].Value, this.XMLDataFile);
                }
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
                    testStepNode.Attributes[index].InnerText = XMLHelper.ReplaceIfToken(testStepNode.Attributes[index].InnerText, this.XMLDataFile);
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
