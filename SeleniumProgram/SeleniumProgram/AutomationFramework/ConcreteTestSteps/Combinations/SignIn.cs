// <copyright file="SignIn.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of signing in.
    /// </summary>
    public class SignIn : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SignIn";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string usernameXPath = "//input[@id='username']";
            string username = this.TestStepInfo.Attributes["username"].Value;

            string passwordXPath = "//input[@id='password']";
            string password = this.TestStepInfo.Attributes["password"].Value;

            string signInButtonXPath = "//input[@id='signin']";

            this.Driver.NavigateToURL();
            this.Driver.PopulateElement(usernameXPath, username);
            this.Driver.PopulateElement(passwordXPath, password);
            this.Driver.ClickElement(signInButtonXPath);
            this.Driver.WaitForLoadingSpinner();
        }
    }
}
