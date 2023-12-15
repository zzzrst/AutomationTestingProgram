// <copyright file="ChooseCollectionOld.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
/*
namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class ChooseCollectionOld : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Choose Collection Old";

        /// <summary>
        /// Gets or sets the attributes of the element provided through the data.
        /// </summary>
        private IDictionary<string, string> Attributes { get; set; }

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string obj = this.Arguments["object"];
            string property = this.Arguments["comment"];
            string collection = ((DatabaseStepData)TestStepData).QuerySpecialChars(GetEnvironmentVariable(EnvVar.Environment), obj).ToString();

            string attribute = this.Arguments["comments"];
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

            Dictionary<string, string> attributes = new Dictionary<string, string>();
            attributes.Add("acc_name", "Choose a collection activate");
            attributes.Add("tag", "span");
            ClickWebElement clkCollectionDdl = new ClickWebElement();

            clkCollectionDdl.Arguments.Add(key, testStepNode.Attributes[index].InnerText);
            clkCollectionDdl.Name = "Collection DDL";
            clkCollectionDdl.ShouldLog = this.ShouldLog;
            clkCollectionDdl.ShouldExecuteVariable = this.ShouldExecute();

            if ((this.TestStatus = clkCollectionDdl.Execute()).Pass)
            {
                // find and click collection item
                Test_Object objCollectionLnk = new Test_Object("Element to select from DDL", property, collection);
                objCollectionLnk.AddAttribute("tag", "div");
                ClickLink clkCollectionLnk = new ClickLink(objCollectionLnk, this.Timeout);
                this.TestStatus = clkCollectionLnk.Execute();
            }
        }
    }
}
*/