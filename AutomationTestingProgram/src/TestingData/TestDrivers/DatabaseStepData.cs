// <copyright file="DatabaseStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;

    /// <summary>
    /// A dummy class that isn't used.
    /// </summary>
    internal class DatabaseStepData : ITestStepData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStepData"/> class.
        /// A mandatory constructor that won't do anything.
        /// </summary>
        /// <param name="args">args.</param>
        public DatabaseStepData(string args)
        {
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Database";

        /// <summary>
        /// Gets or sets a value indicating whether flag for querying special characters.
        /// MAy REmove.
        /// </summary>
        private bool SpecialCharFlag { get; set; } = false;

        /// <inheritdoc/>
        public void SetArguments(TestStep testStep)
        {
            // run for at most number of local attempts, until test action passes
            // update object and value by querying special characters
            try
            {
                this.QueryObjectAndArguments(testStep);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                string testStepName = "Attempt 1/" + testStep.MaxAttempts;
                string testStepDesc = "Error while trying to query the database.";
                string testStepExp = "Execute " + testStep.Description + " successfully";
                string testStepAct = "Failure in executing " + testStep.Description + "!\n" + e.ToString();
                /*this.Alm.AddTestStep(testStepName, "Failed", testStepDesc, testStepExp, testStepAct);
                passed = false;
                message = testStepDesc;
                attempts = this.LocalAttempts + 1;*/
            }
        }

        /// <inheritdoc/>
        public void SetUp()
        {
        }

        /// <inheritdoc/>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If object identifier or arguments begin with special characters in ['!', '@', '##', '$$'], then
        /// they are replaced by their respective value in the database.
        /// </summary>
        /// <param name="testStep">The Test step to query arguments.</param>.
        public void QueryObjectAndArguments(TestStep testStep)
        {
            string environment = "";/*this.Alm.AlmEnvironment;

            // query to update each of the test object's attribute value
            foreach (string attribute in this.TestObject.GetAttributes())
            {
                string attribVal = this.TestObject.GetAttributeValue(attribute);
                string queried = this.DbDriver.QuerySpecialChars(environment, attribVal) as string;
                this.TestObject.SetAttribute(attribute, queried);
            }*/

            Dictionary<string, string> arguments = new Dictionary<string, string>();

            // query to update each of the test action's values
            foreach (string key in testStep.Arguments.Keys)
            {
                arguments.Add(key, this.QuerySpecialChars(environment, testStep.Arguments[key]) as string);
            }

            this.SpecialCharFlag = false;
        }

        /// <summary>
        /// If the original string begins with special characters in ['!', '@', '##', '$$'], then
        /// it is replaced by the respective value in the database.
        /// </summary>
        /// <param name="environment">Environment database to query from (if applicable).</param>
        /// <param name="original">Original string.</param>
        /// <returns>The respective value in the database, or the original string itself.</returns>
        private object QuerySpecialChars(string environment, string original)
        {
            string msg = string.Empty;
            string token = "$(environment)";
            if (original.Contains(token))
            {
                original = original.Replace(token, environment);
                msg = $"String contained token '{token}' which was replaced with {environment}";
            }

            object result = original;

            if (original.Length >= 7 && original.Substring(0, 7).ToLower() == "!select")
            {
                // query from RVDEV1 database
                this.ConnectToDatabase(this.TestDB);
                result = this.TestDB.ExecuteQuery(original.Substring(1))[0][0];
                msg += $"Query replaced with: {result}.";
            }
            else if (original.Length >= 7 && original.Substring(0, 7).ToLower() == "@select")
            {
                // query from test environment database
                if (original.Trim() == "@")
                {
                    result = this.GetEnvironmentURL(environment);
                }
                else
                {
                    result = this.ProcessEnvironmentQuery(environment, original.Substring(1));
                    msg += $"Query replaced with: {result}.";
                }
            }
            else if (original.Length >= 2 && original.Substring(0, 2) == "##")
            {
                // query password from Keychain accounts spreadsheet
                string username = original.Substring(2);
                //result = ExcelDriver.QueryKeychainAccountPassword(username);
                msg += $"Query of {original} replaced password with: {result}.";
            }
            else if (original.Length >= 2 && original.Substring(0, 2) == "$$")
            {
                // query from Excel file
                int split = original.IndexOf(';');
                string filepath = original.Substring(2, split);
                string query = original.Substring(split + 1);
                //result = ExcelDriver.QueryExcelFile(filepath, query);
            }

            // display console log
            if (this.SpecialCharFlag = !string.IsNullOrEmpty(msg) || this.SpecialCharFlag)
            {
                Logger.Info(msg);
            }

            return result;
        }

        /// <summary>
        /// Returns the URL to access the environment.
        /// </summary>
        /// <param name="environment">The enviornment we want to connect to.</param>
        /// <returns>The URL to access the environment.</returns>
        private string GetEnvironmentURL(string environment)
        {
            this.ConnectToDatabase(this.TestDB);
            string query = $"select t.url from {this.EnvDBName} t where t.environment = '{environment}'";
            List<List<object>> result = this.TestDB.ExecuteQuery(query);
            if (result.Count == 0)
            {
                throw new Exception($"Environment provided '{environment}' is not in table!");
            }

            return result[0][0].ToString();
        }

        /// <summary>
        /// Executes the given query in an environment database.
        /// </summary>
        /// <param name="environment">Name of environment to execute in.</param>
        /// <param name="query">Query to execute.</param>
        /// <returns>The data queried from the database.</returns>
        private string ProcessEnvironmentQuery(string environment, string query)
        {
            this.ConnectToDatabase(this.EnvDB, environment);
            return this.EnvDB.ExecuteQuery(query)[0][0].ToString();
        }
    }
}
