// <copyright file="Login.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using TestingDriver;
    using static AutomationTestingProgram.InformationObject;
    using NPOI.SS.UserModel;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using AutomationTestinProgram.Helper;

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

            string url = string.Empty;
            string username = string.Empty;
            string password = string.Empty;
            // determine if we are getting the data from the DB or from CSV (if CSV, put it into Excel case data
            if (System.Configuration.ConfigurationManager.AppSettings["ENV_LOCATION"].ToString().ToUpper() == "DB")
            {
                // query values from database (note that this means that we will always be using the DB to store environment variables)
                DatabaseStepData dbdata = new DatabaseStepData("");
                username = dbdata.QuerySpecialChars(environment, obj).ToString();
                password = dbdata.QuerySpecialChars(environment, value).ToString();

                // navigate to the URL defined in the Environment list page
                url = dbdata.GetEnvironmentURL(environment);
            }
            else if (System.Configuration.ConfigurationManager.AppSettings["ENV_LOCATION"].ToString().ToUpper() == "CSV")
            {
                // if using csv file, then take the env list page.
                url = CSVEnvironmentGetter.GetEnvironmentURL(environment);
                username = obj;
            }
            else
            {
                Logger.Error("App settings value for ENV_LOCATION is not set");
            }

            InformationObject.TestAutomationDriver.NavigateToURL(url);

            // for logins via GoSecure, BPS, or AD
            this.HTMLWhiteListTag = "WebEdit_HTMLTags";
            string usernameXPath = this.MakeXPath("html id", "username") + " | " + "//input[@name='loginfmt']";
            string passwordXPath = this.MakeXPath("html id", "password") + " | " + "//input[@name='passwd']";

            // Sign in button xpaths
            string goSecureSignInButtonXPath = this.MakeXPath("html id", "signin");
            string opsBpsLoginButtonXpath = "//input[@type='submit']";
            string azureADNextButtonXpath = this.MakeXPath("html id", "idSIButton9");
            string concated = goSecureSignInButtonXPath + " | " + opsBpsLoginButtonXpath;

            // set the local timeout to 1 second
            InformationObject.TestAutomationDriver.LocalTimeout = 1;

            // we want to make it full screen, added by Victor
            // InformationObject.TestAutomationDriver.Maximize(); // better to maximize browser when we log on

            // sleep for 1.5 seconds
            Thread.Sleep(1500);

            InformationObject.TestAutomationDriver.PopulateElement(usernameXPath, username);

            bool azureExists = InformationObject.TestAutomationDriver.CheckForElementState(azureADNextButtonXpath, TestingDriver.ITestingDriver.ElementState.Visible);

            Logger.Info("Existence of aad is: " + azureExists);

            if (azureExists)
            {
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
                        // AutomationFramework.

                        // we can try to end the entire process here
                    }
                }

                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
                Thread.Sleep(1500); // wait for 5 seconds for page to load
            }
            else
            {
                // take keychain password
                password = CSVEnvironmentGetter.QueryKeychainAccountPassword(username);
            }

            InformationObject.TestAutomationDriver.PopulateElement(passwordXPath, password);

            this.HTMLWhiteListTag = "WebButton_HTMLTags";

            InformationObject.TestAutomationDriver.ClickElement(concated);

            // here we can add something in case the login failed.
            // We currently just check if any of the pages are reached. If they are, then we will write in the report the appropriate error message.
            // In the near future, we'd like to automate this for each specific flow.
            // we would like to handle the following test cases:
            // 1. If the password did not work, then Reset password via forgot password link and update the password in the spreadsheet
            // 2. Else If migration page is displayed, then Migrate the account and locate the validation email link, and select it after closing the browser and authenticate
            // 3. End if resume Log In again and continue execution.
            string incorrectPasswordOrUsernameXpath = "//*[@class='loginFailed']";
            string accountLockedXpath = "//*[@id='one_column']/div/div/h1";
            string changePasswordScreenXpath = "//div[@class='login-title']";

            if (InformationObject.TestAutomationDriver.VerifyElementText("Authentication failed.", incorrectPasswordOrUsernameXpath) == true ||
                InformationObject.TestAutomationDriver.VerifyElementText("System error. Please re-try your action. If you continue to get this error, please contact the Administrator.", incorrectPasswordOrUsernameXpath) == true)
            {
                string opsBpsForgotPasswordXPath = "//a[@href='https://www.iamu.security.gov.on.ca/goID/access/start.xhtml?next=fyp&lang=en']";

                InformationObject.TestAutomationDriver.ClickElement(opsBpsForgotPasswordXPath);
            }
            else if (InformationObject.TestAutomationDriver.VerifyElementText("An incorrect GO Secure Login ID or Password was specified", incorrectPasswordOrUsernameXpath) == true)
            {
                string goSecureForgotPasswordXPath = "//*[@href='/goID/access/start.xhtml?next=fyp&lang=en']";

                InformationObject.TestAutomationDriver.ClickElement(goSecureForgotPasswordXPath);
            }
            else if (InformationObject.TestAutomationDriver.VerifyElementText("GO Secure Login - Account Locked", accountLockedXpath) == true)
            {
                string passwordRecoveryXPath = "//a[@href='./fyp/index.xhtml?backUrl=https%3A%2F%2Fwww.training.edcs.csc.gov.on.ca%3A443%2FMain%2F']";

                InformationObject.TestAutomationDriver.ClickElement(passwordRecoveryXPath);
            }
            else if (InformationObject.TestAutomationDriver.VerifyElementText("Select a new password:", changePasswordScreenXpath) == true)
            {
                string passwordInput = this.MakeXPath("html id", "passwordForm:password");
                string confirmPassword = this.MakeXPath("html id", "passwordForm:confirmPassword");

                string updatePasswordXpath = this.MakeXPath("html id", "passwordForm:submit");

                InformationObject.TestAutomationDriver.ClickElement(updatePasswordXpath);
            }

            if (InformationObject.TestAutomationDriver.VerifyElementText("Password Recovery - Enter ID", accountLockedXpath) == true)
            {
                // for login failed, max attempts should be always set as 1
                this.MaxAttempts = 1;

                // Navigate to Security Questions
                string goSecureForgotIdXPath = "//*[@href='#']";
                string goSecureEmailXPath = this.MakeXPath("html id", "passwordRecoveryForm:email");
                string goSecureGoNextXPath = this.MakeXPath("html id", "passwordRecoveryForm:submit");

                InformationObject.TestAutomationDriver.ClickElement(goSecureForgotIdXPath);
                InformationObject.TestAutomationDriver.PopulateElement(goSecureEmailXPath, username);
                InformationObject.TestAutomationDriver.ClickElement(goSecureGoNextXPath);

                // handle if security questions not available for this id
                if (InformationObject.TestAutomationDriver.VerifyElementText("Sorry, security questions are not available for this ID.", "//tr[@class='error']/td") == true)
                {
                    Logger.Info("Unabled to reset password, ending program");
                    InformationObject.BlockTestSet = true;
                    // this.ShouldExecuteVariable = false; // stop any more executions
                    this.TestStepStatus.RunSuccessful = false; // rest of try clause is skipped if it fails
                    this.TestStepStatus.Actual = "Password failed to be reset, Security questions not available for ID.";
                }
                else
                {
                    // Answer Security Questions
                    string securityQuestionOneXPath = "//*[@for='passwordRecoveryForm:answerOne']";
                    string securityQuestionTwoXPath = "//*[@for='passwordRecoveryForm:answerTwo']";
                    string securityQuestionThreeXPath = "//*[@for='passwordRecoveryForm:answerThree']";

                    string securityQuestionOne = InformationObject.TestAutomationDriver.GetElementText(securityQuestionOneXPath);
                    string securityQuestionTwo = InformationObject.TestAutomationDriver.GetElementText(securityQuestionTwoXPath);
                    string securityQuestionThree = InformationObject.TestAutomationDriver.GetElementText(securityQuestionThreeXPath);

                    string securityAnswerOneXPath = this.MakeXPath("html id", "passwordRecoveryForm:answerOne");
                    string securityAnswerTwoXPath = this.MakeXPath("html id", "passwordRecoveryForm:answerTwo");
                    string securityAnswerThreeXPath = this.MakeXPath("html id", "passwordRecoveryForm:answerThree");

                    InformationObject.TestAutomationDriver.PopulateElement(securityAnswerOneXPath, this.FillSecurityQuestion(securityQuestionOne));
                    InformationObject.TestAutomationDriver.PopulateElement(securityAnswerTwoXPath, this.FillSecurityQuestion(securityQuestionTwo));
                    InformationObject.TestAutomationDriver.PopulateElement(securityAnswerThreeXPath, this.FillSecurityQuestion(securityQuestionThree));

                    string submitPasswordXPath = "//*[@class='submitButton'][@value=' Continue ']";

                    // Input New Password
                    DateTime dt = DateTime.Now;
                    string date = dt.ToString("MMMddyyyy");
                    date = date.Remove(3, 1); // removes extra . at the end of month abbreviation
                    string newPassword = date + "!";

                    string newPasswordXPath = this.MakeXPath("html id", "passwordRecoveryForm:password");
                    string confirmNewPasswordXPath = this.MakeXPath("html id", "passwordRecoveryForm:confirmPassword");

                    InformationObject.TestAutomationDriver.PopulateElement(newPasswordXPath, newPassword);
                    InformationObject.TestAutomationDriver.PopulateElement(confirmNewPasswordXPath, newPassword);

                    InformationObject.TestAutomationDriver.ClickElement(submitPasswordXPath);

                    // Updating Keychain File only if password change successful
                    string successMessageXPath = "//*[@class='message']";

                    if (InformationObject.TestAutomationDriver.VerifyElementText("You may now login with your new password.", successMessageXPath))
                    {
                        string filepath = @"\\csc.ad.gov.on.ca\DFS$\GrpData\ESIP\ESIP\EDCS QA\QTP\TEST KCQA user accounts\KeychainAccounts2023.xls";
                        try
                        {
                            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite))
                            {
                                IWorkbook workbook = new HSSFWorkbook(fs);
                                ISheet sheet = workbook.GetSheetAt(0);

                                int emailColumnIndex = -1;
                                int passwordColumnIndex = -1;

                                // Find the column indexes for "Email_Account" and "Password" headers
                                IRow headerRow = sheet.GetRow(0);
                                if (headerRow != null)
                                {
                                    for (int cellIndex = 0; cellIndex < headerRow.LastCellNum; cellIndex++)
                                    {
                                        ICell headerCell = headerRow.GetCell(cellIndex);
                                        if (headerCell.StringCellValue == "Email_Account")
                                        {
                                            emailColumnIndex = cellIndex;
                                        }
                                        else if (headerCell.StringCellValue == "Password")
                                        {
                                            passwordColumnIndex = cellIndex;
                                        }
                                        if (emailColumnIndex != -1 && passwordColumnIndex != -1)
                                        {
                                            break;
                                        }
                                    }
                                }

                                // Find the password cell to update and update password
                                for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                                {
                                    IRow row = sheet.GetRow(rowIdx);
                                    if (row != null)
                                    {
                                        ICell usernameCell = row.GetCell(emailColumnIndex);
                                        if (usernameCell.StringCellValue == username)
                                        {
                                            ICell passwordCell = row.GetCell(passwordColumnIndex);
                                            passwordCell.SetCellValue(newPassword);
                                            break;
                                        }
                                    }
                                }

                                // Save changes to Excel file
                                using (FileStream outFile = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                                {
                                    workbook.Write(outFile);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Excel reading/opening error: " + ex.Message);
                            Logger.Info("Unable to save password: " + newPasswordXPath + " for user " + username);
                            Logger.Info("Suggestion: check if the password has been changed or saved correctly");
                        }
                    }
                }

                Logger.Info("Password Reset finished, ending program.");

                // block all remaining tests
                InformationObject.BlockTestSet = true;
                //this.ShouldExecuteVariable = false; // stop any more executions
                // InformationObject.ShouldExecute = false;
                this.TestStepStatus.RunSuccessful = false; // rest of try clause is skipped if it fails
                this.TestStepStatus.Actual = "Password failed on login due to incorrect password or username, resetted password. Please manually rerun after 20 minutes.";
            }

            // wait for the spinner to finish loading
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }

        private string FillSecurityQuestion(string securityQuestion)
        {
            string lastWord = securityQuestion.Split(' ').Last();
            lastWord = lastWord.Remove(lastWord.Length - 1); // remove "?"
            char[] tempArray = lastWord.ToCharArray();
            Array.Reverse(tempArray);
            return new string(tempArray);
        }

        private string MakeXPath(string attribute, string attributeValue)
        {
            this.Attributes.Clear();
            this.Attributes.Add(attribute, attributeValue);
            this.Attributes.Add("tag", "input");
            return this.XPathBuilder();
        }

        /// <summary>
        /// The createJSCommandForAttributeValuePairs.
        /// </summary>
        /// <param name="attriVals">The attriVals<see cref=""/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected string CreateJSCommandForAttributeValuePairs(string key, string value)
        {
            string jSCommand = "var elements = arguments[0];" +
                "for (var i = 0; i < elements.length; i++) {";

            List<string> xPathIgnoreList = new List<string>(ConfigurationManager.AppSettings["XPATH_IGNORE_LIST"].ToString().Split(','));

            // string attributeFoundVariables = string.Empty;
            string ifBuilder = "if (";

            string variableName = $"{value}Found";
            string temp = $" var {variableName} = ";
            temp += $"elements[i].{key} == \"{value}\";";

            // attributeFoundVariables += temp;
            ifBuilder += $"{variableName} && ";

            jSCommand += temp;
            jSCommand += ifBuilder.Substring(0, ifBuilder.Length - 4);

            jSCommand += ") {";
            jSCommand += " return elements[i];";
            jSCommand += "}}";

            return jSCommand;
        }
    }
}
