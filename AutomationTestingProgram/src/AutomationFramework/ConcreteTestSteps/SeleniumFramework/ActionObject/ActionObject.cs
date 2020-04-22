// <copyright file="ActionObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using OpenQA.Selenium;

    /// <summary>
    /// This test step abstract class for verifying objects.
    /// </summary>
    public abstract class ActionObject : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ActionObject";

        /// <summary>
        /// Gets or sets the attributes of the element provided through the data.
        /// </summary>
        protected IDictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Gets or sets xPath of the element.
        /// </summary>
        protected string XPath { get; set; }

        /// <summary>
        /// Gets or sets any Java script command to use.
        /// </summary>
        protected string JsCommand { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string attribute = this.Arguments["comment"];
            string attribValue = this.Arguments["object"];
            this.Attributes = new Dictionary<string, string>();

            // if it has ; then it is not for browser object
            if (attribute.Contains(";") || attribValue.Contains(";"))
            {
                this.Attributes.Add(attribute, attribValue);
            }
            else
            {
                // following sequence like in UFT
                string attributeValuePairs = attribute + ":=" + attribValue;
                string[] listOfAttributeValue = attributeValuePairs.Split(';');
                foreach (string attributeVal in listOfAttributeValue)
                {
                    string attrib = attributeVal.Substring(0, attributeVal.IndexOf(':'));
                    string val = attributeVal.Substring(attributeVal.IndexOf('=') + 1);
                    this.Attributes.Add(attrib, val);
                }
            }

            this.XPath = this.XPathBuilder();
            this.JsCommand = string.Empty;

            if (this.Attributes.ContainsKey("innertext") || this.Attributes.ContainsKey("innerhtml")
            || this.Attributes.ContainsKey("outertext") || this.Attributes.ContainsKey("outerhtml"))
            {
                this.JsCommand = this.CreateJSCommandForAttributeValuePairs(this.Attributes);
            }
        }

        /// <summary>
        /// Builds the XPath query given a test object's property attribute-value pairs.
        /// </summary>
        /// <param name="htmlWhitelist">Determins whether to use the white list or not.</param>
        /// <returns>The XPath query built from the given property attribute-value pairs.</returns>
        protected string XPathBuilder(bool htmlWhitelist = true)
        {
            const string AND = " and ";
            List<string> htmltagwhiteList = htmlWhitelist ? new List<string>(ConfigurationManager.AppSettings["WebCheckBox_HTMLTags"].ToString().Split(',')) : new List<string>() { };
            List<string> xPathIgnoreList = new List<string>(ConfigurationManager.AppSettings["XPATH_IGNORE_LIST"].ToString().Split(','));
            for (int i = 0; i < xPathIgnoreList.Count; i++)
            {
                xPathIgnoreList[i] = xPathIgnoreList[i].ToLower();
            }

            // early exit if they specify xpath (trust the user to provide a valid xpath)
            if (this.Attributes.ContainsKey("xpath"))
            {
                return this.Attributes["xpath"];
            }

            // initialize xPath
            string xPath = string.Empty;

            // if user provides html tags, then we ignore the htmltagwhilteList and htmltagblackList
            if (this.Attributes.ContainsKey("tag"))
            {
                xPath = $"//{this.Attributes.TryGetValue("tag", out string value)}[";
            }
            else if (this.Attributes.ContainsKey("html tag"))
            {
                xPath = $"//{this.Attributes.TryGetValue("html tag", out string value)}[";
            }
            else if (htmltagwhiteList.Count == 0)
            {
                xPath = "//*[";
            }
            else
            {
                xPath = this.XPathTagHelper(htmltagwhiteList);
            }

            bool hasAttribute = false;

            // loop through the attributes, and generate the xPath
            foreach (string key in this.Attributes.Keys)
            {
                string attribute = this.FixAttribute(key);
                string attribVal = this.Attributes[attribute];

                // ignore attributes in xPathIgnoreList
                if (!xPathIgnoreList.Contains(attribute) && attribute != "tag" && attribute != "html tag")
                {
                    xPath += $"@{attribute.ToLower()} = \"{attribVal}\"" + AND;
                    hasAttribute = true;
                }
            }

            if (hasAttribute)
            {
                xPath = xPath.Substring(0, xPath.Length - AND.Length) + "]";
            }
            else
            {
                // remove the [ that was appended.
                xPath = xPath.Substring(0, xPath.Length - 1);
            }

            return xPath;
        }

        /// <summary>
        /// The createJSCommandForAttributeValuePairs.
        /// </summary>
        /// <param name="attriVals">The attriVals<see cref="T:IDictionary{string, string}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected string CreateJSCommandForAttributeValuePairs(IDictionary<string, string> attriVals)
        {
            string jSCommand = "var elements = arguments[0];" +
                "for (var i = 0; i < elements.length; i++) {";

            List<string> xPathIgnoreList = new List<string>(ConfigurationManager.AppSettings["XPATH_IGNORE_LIST"].ToString().Split(','));

            string attributeFoundVariables = string.Empty;
            string ifBuilder = "if (";

            foreach (string attribute in xPathIgnoreList)
            {
                string variableName = $"{attribute}Found";
                string temp = $" var {variableName} = ";
                temp += attriVals.ContainsKey(attribute.ToLower()) ?
                    $"elements[i].{attribute} == \"{attriVals[attribute.ToLower()]}\";" : "true;";

                attributeFoundVariables += temp;
                ifBuilder += $"{variableName} && ";
            }

            jSCommand += attributeFoundVariables;
            jSCommand += ifBuilder.Substring(0, ifBuilder.Length - 4);

            jSCommand += ") {";
            jSCommand += " return elements[i];";
            jSCommand += "}}";

            return jSCommand;
        }

        /// <summary>
        /// Converts UFT attribute to corresponding Selenium property attribute.
        /// </summary>
        /// <param name="attribute">Property attribute as it appears in UFT.</param>
        /// <returns>The corresponding Selenium attirbute.</returns>
        protected string FixAttribute(string attribute)
        {
            switch (attribute)
            {
                case "html id":
                    attribute = "id";
                    break;
                case "acc_name":
                    attribute = "aria-label";
                    break;
                case "":
                    attribute = "id";
                    break;
            }

            return attribute;
        }

        /// <summary>
        /// The XPathTagHelper.
        /// </summary>
        /// <param name="htmltagwhiteList">The htmltagwhiteList<see cref="T:List{string}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected string XPathTagHelper(List<string> htmltagwhiteList)
        {
            string xPath = "(";
            foreach (string tag in htmltagwhiteList)
            {
                xPath += $"//{tag} | ";
            }

            // remove the last |
            xPath = xPath.Substring(0, xPath.Length - 3);
            xPath += ")[";

            return xPath;
        }

        /// <summary>
        /// Clicks on this browser object. If the object opens a modal box after clicking,. <code>opensModal</code> can be set
        /// to execute a separate JavaScript click call that bypasses the modal's blocking call.
        /// </summary>
        /// <param name="opensModal">Whether to execute the JavaScript click call to bypass modal's blocking call.</param>
        /// <returns><code>true</code> if browser object was successfully clicked.</returns>
        protected bool Click(bool opensModal)
        {
            /*
            try
            {
                // this.Browser.MouseOver(this.Element);
                InformationObject.TestAutomationDriver.WDWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(this.Element));

                // if button opens a modal, use Javascript to bypass blocking call, and try clicking it a second time
                if (opensModal && this.Browser.BrowserType.Contains("ie"))
                {
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)this.Browser.Driver;
                    executor.ExecuteScript("var element=arguments[0]; setTimeout(function() {element.click();}, 100)", this.Element);
                }
                else
                {
                    this.Element.Click();
                }

                return true;
            }
            catch
            {
                return false;
            }
            */
            return false;
        }

        /// <summary>
        /// Clicks on this browser object.
        /// </summary>
        /// <returns><code>true</code> if browser object was successfully clicked.</returns>
        protected bool Click()
        {
            return this.Click(false);
        }
    }
}
