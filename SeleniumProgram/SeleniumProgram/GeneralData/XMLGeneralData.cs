// <copyright file="XMLGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;

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
            Dictionary<string, string> paramenters = new Dictionary<string, string>();
            XmlDocument xmlDocObj = new XmlDocument();
            xmlDocObj.Load(xmlFile);
            XmlDocument xMLDataFile;

            // Must parse data file first since values above it can be a token.
            if (dataFile == string.Empty)
            {
                if (xmlDocObj.GetElementsByTagName("DataFile").Count > 0)
                {
                    dataFile = xmlDocObj.GetElementsByTagName("DataFile")[0].InnerText;
                    if (File.Exists(dataFile))
                    {
                        xMLDataFile = new XmlDocument();
                        xMLDataFile.Load(dataFile);
                    }
                    else
                    {
                        Logger.Error("XML File could not be found!");
                    }
                }
            }


            return paramenters;
        }

        /// <inheritdoc/>
        public bool Verify(string xmlFile)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add("http://qa/SeleniumPerf", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "GeneralData\\DataVerifier\\SeleniumPerf.xsd");
                settings.ValidationType = ValidationType.Schema;

                XmlReader reader = XmlReader.Create(xmlFile, settings);
                XmlDocument document = new XmlDocument();
                document.Load(reader);

                ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

                // the following call to Validate succeeds.
                document.Validate(eventHandler);
            }
            catch (XmlSchemaValidationException)
            {
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
