// <copyright file="Login.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to log in.
    /// </summary>
    public class Login : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Login";

        /// <inheritdoc/>
        public override void Execute()
        {
            throw new System.NotImplementedException();
            base.Execute();
            string usernameXPath = "//input[@id='username']";
            string username = this.Arguments["username"];

            string passwordXPath = "//input[@id='password']";
            string password = this.Arguments["password"];

            string signInButtonXPath = "//input[@id='signin']";

            InformationObject.TestAutomationDriver.NavigateToURL();
            InformationObject.TestAutomationDriver.PopulateElement(usernameXPath, username);
            InformationObject.TestAutomationDriver.PopulateElement(passwordXPath, password);
            InformationObject.TestAutomationDriver.ClickElement(signInButtonXPath);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();

        }
    }
}
