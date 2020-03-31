// <copyright file="XMLGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using AutomationTestingProgram.Helper;
    using static InformationObject;

    /// <summary>
    /// The implementation of the general data for XML.
    /// </summary>
    public class XMLGeneralData : ITestGeneralData
    {
        /// <inheritdoc/>
        public string Name { get; } = "XML";

        /// <inheritdoc/>
        public Dictionary<EnvVar, string> ParseParameters(string xmlFile, string dataFile)
        {
            Dictionary<EnvVar, string> parameters = new Dictionary<EnvVar, string>();
            XmlDocument xmlDocObj = new XmlDocument();
            XmlDocument xmlDataFile = new XmlDocument();

            if (File.Exists(xmlFile))
            {
                xmlDocObj.Load(xmlFile);
            }
            else
            {
                Logger.Error($"XML File {xmlFile} could not be found!");
            }

            // Must parse data file first since values above it can be a token.
            if (dataFile == string.Empty || dataFile == null)
            {
                if (xmlDocObj.GetElementsByTagName("DataFile").Count > 0)
                {
                    dataFile = xmlDocObj.GetElementsByTagName("DataFile")[0].InnerText;
                    if (File.Exists(dataFile))
                    {
                        xmlDataFile.Load(dataFile);
                    }
                    else
                    {
                        Logger.Error("XML File could not be found!");
                    }
                }
            }

            // URL has precedence over the environment.
            // Passed in parameters overide what is in the XML.
            if (xmlDocObj.GetElementsByTagName("URL").Count > 0)
            {
                parameters.Add(EnvVar.URL, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("URL")[0].InnerText, xmlDataFile));
            }
            else
            {
                string enviornment = this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("Environment")[0].InnerText, xmlDataFile);
                parameters.Add(EnvVar.Environment, enviornment);
                parameters.Add(EnvVar.URL, ConfigurationManager.AppSettings[enviornment].ToString());
            }

            parameters.Add(EnvVar.Browser, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("Browser")[0].InnerText, xmlDataFile));
            if (xmlDocObj.GetElementsByTagName("RespectRepeatFor").Count > 0)
            {
                parameters.Add(EnvVar.RespectRepeatFor, xmlDocObj.GetElementsByTagName("RespectRepeatFor")[0].InnerText);
            }

            if (xmlDocObj.GetElementsByTagName("RespectRunAODAFlag").Count > 0)
            {
                parameters.Add(EnvVar.RespectRunAODAFlag, xmlDocObj.GetElementsByTagName("RespectRunAODAFlag")[0].InnerText);
            }

            parameters.Add(EnvVar.TimeOutThreshold, xmlDocObj.GetElementsByTagName("TimeOutThreshold")[0].InnerText);
            parameters.Add(EnvVar.WarningThreshold, xmlDocObj.GetElementsByTagName("WarningThreshold")[0].InnerText);
            parameters.Add(EnvVar.CsvSaveFileLocation, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("CSVSaveLocation")[0].InnerText, xmlDataFile));

            if (xmlDocObj.GetElementsByTagName("LogSaveLocation").Count > 0)
            {
                parameters.Add(EnvVar.LogSaveFileLocation, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("LogSaveLocation")[0].InnerText, xmlDataFile));
            }

            if (xmlDocObj.GetElementsByTagName("reportSaveFileLocation").Count > 0)
            {
                parameters.Add(EnvVar.ReportSaveFileLocation, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("ReportSaveFileLocation")[0].InnerText, xmlDataFile));
            }

            if (xmlDocObj.GetElementsByTagName("screenshotSaveLocation").Count > 0)
            {
                parameters.Add(EnvVar.ScreenshotSaveLocation, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("ScreenshotSaveLocation")[0].InnerText, xmlDataFile));
            }

            // Special Elements
            if (xmlDocObj.GetElementsByTagName("loadingSpinner").Count > 0)
            {
                parameters.Add(EnvVar.LoadingSpinner, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("LoadingSpinner")[0].InnerText, xmlDataFile));
            }
            else
            {
                parameters.Add(EnvVar.LoadingSpinner, ConfigurationManager.AppSettings["LoadingSpinner"].ToString());
            }

            if (xmlDocObj.GetElementsByTagName("errorContainer").Count > 0)
            {
                parameters.Add(EnvVar.ErrorContainer, this.ReplaceIfToken(xmlDocObj.GetElementsByTagName("ErrorContainer")[0].InnerText, xmlDataFile));
            }
            else
            {
                parameters.Add(EnvVar.ErrorContainer, ConfigurationManager.AppSettings["ErrorContainer"].ToString());
            }

            return parameters;
        }

        /// <inheritdoc/>
        public bool Verify(string xmlFile)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://qa/SeleniumPerf", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\src\\GeneralData\\DataVerifier\\SeleniumPerf.xsd");
                settings.ValidationType = ValidationType.Schema;

                XmlReader reader = XmlReader.Create(xmlFile, settings);
                XmlDocument document = new XmlDocument();
                document.Load(reader);

                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

                // the following call to Validate succeeds.
                document.Validate(eventHandler);
            }
            catch (XmlSchemaValidationException e)
            {
                Logger.Error($"Line: {e.LineNumber} Message: {e.Message}");
                return false;
            }

            return true;
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Logger.Error($"XML validation error: {e.Message} on Line: {e.Exception.LineNumber}");
                    break;
                case XmlSeverityType.Warning:
                    Logger.Warn($"XML validation warning: {e.Message} on Line: {e.Exception.LineNumber}");
                    break;
            }
        }

        /// <summary>
        /// Replaces a string if it is a token and shown.
        /// </summary>
        /// <param name="possibleToken">A string that may be a token.</param>
        /// <param name="xMLDataFile"> The data file containing the value.</param>
        /// <returns>The provided string or value of the token.</returns>
        private string ReplaceIfToken(string possibleToken, XmlDocument xMLDataFile)
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
