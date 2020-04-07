// <copyright file="XMLData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The base class for XMl data.
    /// </summary>
    public class XMLData : ITestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XMLData"/> class.
        /// </summary>
        /// <param name="xmlLocataion">the location of the xml.</param>
        public XMLData(string xmlLocataion)
        {
            this.TestArgs = xmlLocataion;
        }

        /// <inheritdoc/>
        public string Name { get; set; } = "XML";

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <summary>
        /// Gets or sets the information for the test set.
        /// </summary>
        protected XmlNode TestFlow { get; set; }

        /// <summary>
        /// Gets or sets the xml file containing the XML Data.
        /// </summary>
        protected XmlDocument XMLDataFile { get; set; } = null;

        /// <summary>
        /// Gets or sets the xml file containing the test set/case/steps.
        /// </summary>
        protected XmlDocument XMLDocObj { get; set; } = null;

        /// <inheritdoc/>
        public virtual void SetUp()
        {
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

        /// <summary>
        /// Replaces a string if it is a token and shown.
        /// </summary>
        /// <param name="possibleToken">A string that may be a token.</param>
        /// <param name="xMLDataFile"> The data file containing the value.</param>
        /// <returns>The provided string or value of the token.</returns>
        protected string ReplaceIfToken(string possibleToken, XmlDocument xMLDataFile)
        {
            if (possibleToken.Contains("${{") && possibleToken.Contains("}}") && xMLDataFile != null)
            {
                XmlNode tokens = xMLDataFile.GetElementsByTagName("Tokens")[0];
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
