// <copyright file="XMLHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Aa helper class for all XML classes.
    /// </summary>
    public class XMLHelper
    {
        /// <summary>
        /// Replaces a string if it is a token and shown.
        /// </summary>
        /// <param name="possibleToken">A string that may be a token.</param>
        /// <param name="xMLDataFile"> The data file containing the value.</param>
        /// <returns>The provided string or value of the token.</returns>
        public static string ReplaceIfToken(string possibleToken, XmlDocument xMLDataFile)
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
