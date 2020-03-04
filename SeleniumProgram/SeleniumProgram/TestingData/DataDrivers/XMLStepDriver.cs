// <copyright file="XMLStepDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.DataDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.TestingDriver;
    using AutomationTestSetFramework;

    /// <summary>
    /// The XML Driver to get data from an xml.
    /// </summary>
    public class XMLStepDriver : ITestStepData
    {
        /// <inheritdoc/>
        public string InformationLocation { get; set; }

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
        public void SetUp()
        {
            if (File.Exists(this.InformationLocation))
            {
                this.XMLDocObj = new XmlDocument();
                this.XMLDocObj.Load(this.InformationLocation);
                this.TestFlow = this.XMLDocObj.GetElementsByTagName("TestCaseFlow")[0];
            }
            else
            {
                Logger.Error("XML File could not be found!");
            }
        }

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepName, bool performAction)
        {
            // get the list of testSteps
            XmlNode testSteps = this.XMLDocObj.GetElementsByTagName("TestSteps")[0];

            // Find the appropriate test steps
            foreach (XmlNode innerNode in testSteps.ChildNodes)
            {
                if (innerNode.Name != "#comment" && this.ReplaceIfToken(innerNode.Attributes["id"].Value) == testStepName)
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
            string name = this.ReplaceIfToken(testStepNode.Attributes["name"].Value);

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
                    runAODAPageName = this.ReplaceIfToken(testStepNode.Attributes["runAODAPageName"].Value);
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
                    testStepNode.Attributes[index].InnerText = this.ReplaceIfToken(testStepNode.Attributes[index].InnerText);
                    testStep.Arguments.Add(testStepNode.Attributes[index].Name, testStepNode.Attributes[index].InnerText);
                }

                testStep.Name = name;
                testStep.ShouldLog = log;
                testStep.ShouldExecuteVariable = performAction;
                testStep.RunAODA = runAODA;
                testStep.RunAODAPageName = runAODAPageName;
            }

            return testStep;
        }

        /// <summary>
        /// Replaces a string if it is a token and shown.
        /// </summary>
        /// <param name="possibleToken">A string that may be a token.</param>
        /// <returns>The provided string or value of the token.</returns>
        private string ReplaceIfToken(string possibleToken)
        {
            if (possibleToken.Contains("${{") && possibleToken.Contains("}}") && this.XMLDataFile != null)
            {
                XmlNode tokens = this.XMLDataFile.GetElementsByTagName("Tokens")[0];
                string tokenKey = possibleToken.Substring(possibleToken.IndexOf("${{") + 3);
                tokenKey = tokenKey.Substring(0, tokenKey.IndexOf("}}"));

                // Find the appropriate token
                foreach (XmlNode token in tokens.ChildNodes)
                {
                    if (token.Attributes["key"] != null && token.Attributes["key"].InnerText == tokenKey && token.Attributes["value"] != null)
                    {
                        return possibleToken.Replace("${{" + $"{tokenKey}" + "}}", token.Attributes["value"].InnerText);
                    }
                }
            }

            return possibleToken;
        }
    }
}
