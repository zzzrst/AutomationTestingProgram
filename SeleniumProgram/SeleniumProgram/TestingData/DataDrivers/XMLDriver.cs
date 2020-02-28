// <copyright file="XMLDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.DataDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using AutomationTestSetFramework;

    /// <summary>
    /// The XML Driver to get data from an xml.
    /// </summary>
    public class XMLDriver : ITestSetData, ITestCaseData, ITestStepData
    {
        /// <summary>
        /// The stack to read/excecute for the test set/case.
        /// </summary>
        private readonly Stack<XmlNode> testStack = new Stack<XmlNode>();

        /// <summary>
        /// Determines if the current stack layer for test set/case should execute.
        /// </summary>
        private readonly Stack<bool> performStack = new Stack<bool>();

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
            }
            else
            {
                Logger.Error("XML File could not be found!");
            }
        }

        /// <inheritdoc/>
        public bool ExistNextTestCase()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestCase GetNextTestCase()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GetArguments()
        {
            throw new NotImplementedException();
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
