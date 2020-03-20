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

    /// <summary>
    /// The implementation of the general data for XML.
    /// </summary>
    public class XMLGeneralData : ITestGeneralData
    {
        /// <inheritdoc/>
        public string Name { get; } = "XML";

        /// <inheritdoc/>
        public Dictionary<string, string> ParseParameters(string xmlFile, string dataFile)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
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
                parameters.Add("url", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("URL")[0].InnerText, xmlDataFile));
            }
            else
            {
                string enviornment = XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("Environment")[0].InnerText, xmlDataFile);
                parameters.Add("enviornment", enviornment);
                parameters.Add("url", ConfigurationManager.AppSettings[enviornment].ToString());
            }

            parameters.Add("browser", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("Browser")[0].InnerText, xmlDataFile));
            if (xmlDocObj.GetElementsByTagName("RespectRepeatFor").Count > 0)
            {
                parameters.Add("respectRepeatFor", xmlDocObj.GetElementsByTagName("RespectRepeatFor")[0].InnerText);
            }

            if (xmlDocObj.GetElementsByTagName("RespectRunAODAFlag").Count > 0)
            {
                parameters.Add("respectRunAODAFlag", xmlDocObj.GetElementsByTagName("RespectRunAODAFlag")[0].InnerText);
            }

            parameters.Add("timeOutThreshold", xmlDocObj.GetElementsByTagName("TimeOutThreshold")[0].InnerText);
            parameters.Add("warningThreshold", xmlDocObj.GetElementsByTagName("WarningThreshold")[0].InnerText);
            parameters.Add("csvSaveFileLocation", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("CSVSaveLocation")[0].InnerText, xmlDataFile));

            if (xmlDocObj.GetElementsByTagName("LogSaveLocation").Count > 0)
            {
                parameters.Add("logSaveFileLocation", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("LogSaveLocation")[0].InnerText, xmlDataFile));
            }

            if (xmlDocObj.GetElementsByTagName("reportSaveFileLocation").Count > 0)
            {
                parameters.Add("reportSaveFileLocation", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("ReportSaveFileLocation")[0].InnerText, xmlDataFile));
            }

            if (xmlDocObj.GetElementsByTagName("screenshotSaveLocation").Count > 0)
            {
                parameters.Add("screenshotSaveLocation", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("ScreenshotSaveLocation")[0].InnerText, xmlDataFile));
            }

            // Special Elements
            if (xmlDocObj.GetElementsByTagName("loadingSpinner").Count > 0)
            {
                parameters.Add("loadingSpinner", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("LoadingSpinner")[0].InnerText, xmlDataFile));
            }
            else
            {
                parameters.Add("loadingSpinner", ConfigurationManager.AppSettings["LoadingSpinner"].ToString());
            }

            if (xmlDocObj.GetElementsByTagName("errorContainer").Count > 0)
            {
                parameters.Add("errorContainer", XMLHelper.ReplaceIfToken(xmlDocObj.GetElementsByTagName("ErrorContainer")[0].InnerText, xmlDataFile));
            }
            else
            {
                parameters.Add("errorContainer", ConfigurationManager.AppSettings["ErrorContainer"].ToString());
            }

            return parameters;
        }

        /// <inheritdoc/>
        public bool Verify(string xmlFile)
        {
            return true;
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
    }
}
