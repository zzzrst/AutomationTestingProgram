// <copyright file="Login.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to log in.
    /// </summary>
    public class Login : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Login";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string obj = this.Arguments["object"];
            string value = this.Arguments["value"];
            string environment = GetEnvironmentVariable(EnvVar.Environment);

            // query values from database
            string username = ((DatabaseStepData)TestStepData).QuerySpecialChars(environment, obj).ToString();
            string password = ((DatabaseStepData)TestStepData).QuerySpecialChars(environment, value).ToString();

            this.HTMLWhiteListTag = "WebEdit_HTMLTags";
            string usernameXPath = this.MakeXPath("html id", "username");
            string passwordXPath = this.MakeXPath("html id", "password");

            InformationObject.TestAutomationDriver.NavigateToURL();
            InformationObject.TestAutomationDriver.PopulateElement(usernameXPath, username);
            InformationObject.TestAutomationDriver.PopulateElement(passwordXPath, password);

            this.HTMLWhiteListTag = "WebButton_HTMLTags";
            string signInButtonXPath = this.MakeXPath("html id", "signin");

            InformationObject.TestAutomationDriver.ClickElement(signInButtonXPath);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }

        private string MakeXPath(string attribute, string attributeValue)
        {
            this.Attributes.Clear();
            this.Attributes.Add(attribute, attributeValue);
            this.Attributes.Add("tag", "input");
            return this.XPathBuilder();
        }
    }
}
