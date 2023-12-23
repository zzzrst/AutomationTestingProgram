// <copyright file="LoginAD.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Threading;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using NPOI.HSSF.Record;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to log in.
    /// </summary>
    public class EnterAADCredentials : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Enter AAD Credentials";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string obj = this.Arguments["object"];
            string value = this.Arguments["value"];
            string environment = GetEnvironmentVariable(EnvVar.Environment);

            // query values from database (note that this means that we will always be using the DB to store environment variables)
            DatabaseStepData dbdata = new DatabaseStepData("");
            string username = dbdata.QuerySpecialChars(environment, obj).ToString();
            string password = dbdata.QuerySpecialChars(environment, value).ToString();

            // Sign in button xpaths
            string azureADNextButtonXpath = "//*[@id='idSIButton9']";
            string loginXpath = "//input[@name='loginfmt']";
            string passwordXpath = "//input[@name='passwd']";
            string submitXpath = "//input[@type='submit']";

            bool azureExists = InformationObject.TestAutomationDriver.CheckForElementState(azureADNextButtonXpath, TestingDriver.ITestingDriver.ElementState.Visible);

            Logger.Info("Existence of aad is: " + azureExists);

            if (azureExists)
            {
                InformationObject.TestAutomationDriver.PopulateElement(loginXpath, username);

                // click next button
                InformationObject.TestAutomationDriver.ClickElement(azureADNextButtonXpath);

                // if we are in azure, we can try to get the password provided by the Secure token to input as the password
                // try to use key vault values provided
                string azurePass = GetEnvironmentVariable(EnvVar.SecretInformation).ToString();

                if (azurePass != string.Empty)
                {
                    // provided password using Agent execution
                    password = azurePass;
                    Logger.Info("Changed password to the provided password");
                }
                else
                {
                    // here we will want to use the local machine to authenticate into DevOps and grab the token for this password
                    Logger.Info("Did not change password to provided password, we will auth into DevOps keyvault using local machine");

                    // we will split the username into just the user name. For instance, uft@ontario.ca should be split into just uft.
                    string kvSplitSecretName = username.Split("@")[0];

                    // we will add "-credentials to the name"
                    kvSplitSecretName += "-credentials";

                    Logger.Info("KV secret name is: " + kvSplitSecretName);

                    string keyVaultName = "qa-dev-sc";
                    string kvUri = $"https://{keyVaultName}.vault.azure.net";

                    try
                    {
                        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
                        password = client.GetSecretAsync(kvSplitSecretName).Result.Value.Value;
                        // password = pass.ToString();
                    }
                    catch
                    {
                        Logger.Error("Cannot authenticate DefaultAzureCredential or kv value does not exist");
                    }
                }

                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
                Thread.Sleep(1500); // wait for 5 seconds for page to load

                InformationObject.TestAutomationDriver.PopulateElement(passwordXpath, password);

                InformationObject.TestAutomationDriver.ClickElement(submitXpath);

                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
            }
            else
            {
                Logger.Error("Not an Azure AAD environment, incorrect usage of Enter AAD Credentials");
            }

        }
    }
}
