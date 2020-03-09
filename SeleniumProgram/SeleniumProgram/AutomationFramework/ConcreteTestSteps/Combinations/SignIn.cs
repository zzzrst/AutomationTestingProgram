// <copyright file="SignIn.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of signing in.
    /// </summary>
    public class SignIn : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SignIn";

        /// <inheritdoc/>
        public override void Execute()
        {
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
