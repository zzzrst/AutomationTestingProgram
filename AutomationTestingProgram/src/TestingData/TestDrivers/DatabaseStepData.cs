// <copyright file="DatabaseStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.Exceptions;
    using AutomationTestingProgram.Helper;
    using AutomationTestSetFramework;
    using DatabaseConnector;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// The test step for the database data.
    /// </summary>
    internal class DatabaseStepData : DatabaseData, ITestStepData
    {
        /// <summary>
        /// The path to the keychain account.
        /// </summary>
        internal static readonly string KeychainAccountsFilePath = ConfigurationManager.AppSettings["KeychainAccountsFilePath"].ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStepData"/> class.
        /// A mandatory constructor that won't do anything.
        /// </summary>
        /// <param name="args">args.</param>
        public DatabaseStepData(string args)
            : base(args)
        {
        }

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
            this.QueryObjectAndArguments(testStep);
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
            string environment = GetEnvironmentVariable(EnvVar.Environment);
            /*

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

            testStep.Arguments = arguments;

            this.SpecialCharFlag = false;
        }

        /// <summary>
        /// The GetEnvironmentEmailNotificationFolder.
        /// </summary>
        /// <param name="globalVariableName">Name of the global variable.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetGlobalVariableValue(string globalVariableName)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);

            // Get test_environments table.
            string query = "SELECT T.ROWID, T.GLOBAL_VARIABLE_NAME, T.GLOBAL_VARIABLE_VALUE, T.VARIABLE_VALUE_IS_ENCRYPTED from QA_AUTOMATION.GLOBAL_VARIABLES t " +
                $"WHERE T.GLOBAL_VARIABLE_NAME = '{globalVariableName}'";

            List<List<object>> globalVariableValueTable = this.TestDB.ExecuteQuery(query);

            if (globalVariableValueTable.Count == 0)
            {
                throw new Exception($"Global variable provided '{globalVariableName}' is not in table!");
            }

            List<object> variable = globalVariableValueTable[0];

            int isVariableValueEncrypted = int.Parse(variable[3].ToString());

            string result = variable[2].ToString();

            if (isVariableValueEncrypted == DatabasePasswordState.IsProtected)
            {
                string rowid = variable[0].ToString();

                // Encrypt password by dbName and username.
                result = Helper.DecryptString(result, globalVariableName + rowid);
            }

            return result;
        }

        /// <summary>
        /// The GetEnvironmentEmailNotificationFolder.
        /// </summary>
        /// <param name="environment">The environment<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetEnvironmentEmailNotificationFolder(string environment)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);
            string query = $"select t.EMAIL_NOTIFICATION_FOLDER from QA_AUTOMATION.test_environments t where t.environment = '{environment}'";
            List<List<object>> result = this.TestDB.ExecuteQuery(query);
            if (result.Count == 0)
            {
                throw new Exception($"Environment provided '{environment}' is not in table!");
            }

            return result[0][0].ToString();
        }

        /// <summary>
        /// If the original string begins with special characters in ['!', '@', '##', '$$'], then
        /// it is replaced by the respective value in the database.
        /// </summary>
        /// <param name="environment">Environment database to query from (if applicable).</param>
        /// <param name="original">Original string.</param>
        /// <returns>The respective value in the database, or the original string itself.</returns>
        public object QuerySpecialChars(string environment, string original)
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
                // query from database
                this.TestDB = this.ConnectToDatabase(this.TestDB);
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
                result = this.QueryKeychainAccountPassword(username);
                msg += $"Query of {original} replaced password with: {result}.";
            }
            else if (original.Length >= 2 && original.Substring(0, 2) == "$$")
            {
                // query from Excel file
                int split = original.IndexOf(';');
                string filepath = original.Substring(2, split);
                string query = original.Substring(split + 1);
                Logger.Error($"Sorry, {original} is not implemented yet. or does not work on the current program.");

                // result = ExcelDriver.QueryExcelFile(filepath, query);
            }

            // display console log
            if (this.SpecialCharFlag = !string.IsNullOrEmpty(msg) || this.SpecialCharFlag)
            {
                Logger.Info(msg);
            }

            return result;
        }

        /// <summary>
        /// Executes the given query in the test database.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>The data queried from the database.</returns>
        public List<List<object>> ProcessQADBSelectQuery(string query)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);
            List<List<object>> result = this.TestDB.ExecuteQuery(query);
            this.TestDB.Disconnect();
            return result;
        }

        /// <summary>
        /// Executes the given query in an environment database.
        /// </summary>
        /// <param name="environment">Name of environment to execute in.</param>
        /// <param name="query">Query to execute.</param>
        /// <returns>The data queried from the database.</returns>
        public List<List<object>> ProcessEnvironmentSelectQuery(string environment, string query)
        {
            this.ConnectToEnvDatabase(environment);
            return this.EnvDB.ExecuteQuery(query);
        }

        /// <summary>
        /// Executes a query in an environment database that does not return any data.
        /// </summary>
        /// <param name="environment">Name of environment to execute in.</param>
        /// <param name="sql">Query to execute.</param>
        /// <returns><code>true</code> if query was successfully executed.</returns>
        internal bool ExecuteEnvironmentNonQuery(string environment, string sql)
        {
            this.ConnectToEnvDatabase(environment);
            string cmd = this.CreateSQLPlusCommand(environment, sql);
            return this.EnvDB.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Creates the SQL*Plus command that can be executed in SQL*Plus command-line.
        /// </summary>
        /// <param name="environment">Name of environment to execute in.</param>
        /// <param name="file">SQL file to execute.</param>
        /// <returns>The SQL*Plus command to be executed.</returns>
        private string CreateSQLPlusCommand(string environment, string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFound("File doesn't exist");
            }

            List<object> connectionInfo = this.QueryEnvironmentConnectionInformation(environment);
            this.EnvDBName = connectionInfo[2].ToString();
            return OracleDatabase.CreateCommandHelper(
                connectionInfo[0].ToString(),
                connectionInfo[1].ToString(),
                connectionInfo[2].ToString(),
                connectionInfo[3].ToString(),
                connectionInfo[4].ToString(),
                file);
        }

        /// <summary>
        /// Have the environment database connect to the given environment.
        /// </summary>
        /// <param name="environment">the name of the environment.</param>
        private void ConnectToEnvDatabase(string environment)
        {
            if (this.EnvDB == null || !this.EnvDB.IsConnected() || (this.Environment != string.Empty && this.Environment != environment))
            {
                int count = 0;
                while (count < 3)
                {
                    this.EnvDB = new OracleDatabase(this.CreateEnvironmentConnectionString(environment), Logger.GetLog4NetLogger());
                    this.EnvDB.Connect();
                    this.Environment = environment;
                    if (this.EnvDB.IsConnected())
                    {
                        Logger.Info("Connected to database: " + this.EnvDBName);
                        break;
                    }

                    count++;
                }
            }
        }

        /// <summary>
        /// Queries the information needed to build the connection string.
        /// </summary>
        /// <param name="environment">Name of environment to connect to.</param>
        /// <returns>The information needed to build the connection string.</returns>
        private List<object> QueryEnvironmentConnectionInformation(string environment)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);

            // we add t.is_password_encrypted to be able to check if the password is encrypted or not.
            string query = $"select t.host, t.port, t.db_name, t.username, t.password, t.is_password_encrypted from {this.EnvDBName} t where t.environment = '{environment}'";
            Logger.Debug($"Querying for QueryEnvironmentConnectionInformation : [{query}]");

            // decrypt password if needed.
            List<List<object>> result = this.TestDB.ExecuteQuery(query);

            if (result.Count == 0)
            {
                throw new Exception($"Environment provided '{environment}' is not in table!");
            }

            List<object> connectionInfo = result[0];
            string t_host = connectionInfo[0].ToString();
            string t_port = connectionInfo[1].ToString();
            string t_db_name = connectionInfo[2].ToString();
            string t_username = connectionInfo[3].ToString();
            string t_password = connectionInfo[4].ToString();
            int t_isPasswordEncrypted = int.Parse(connectionInfo[5].ToString());

            switch (t_isPasswordEncrypted)
            {
                case DatabasePasswordState.IsProtected:
                    connectionInfo[4] = Helper.DecryptString(t_password, t_db_name + t_username);
                    break;

                case DatabasePasswordState.IsNotProtected:
                    // do nothing;
                    break;

                default:
                    // something went wrong! We have constraint so that the value must be 0 or 1.
                    break;
            }

            return connectionInfo;
        }

        /// <summary>
        /// Creates the connection string to connect to the environment database.
        /// </summary>
        /// <param name="environment">Name of environment to connect to.</param>
        /// <returns>The connection string to connect to the environment database.</returns>
        private string CreateEnvironmentConnectionString(string environment)
        {
            List<object> connectionInfo = this.QueryEnvironmentConnectionInformation(environment);
            this.EnvDBName = connectionInfo[2].ToString();

            string t_host = connectionInfo[0].ToString();
            string t_port = connectionInfo[1].ToString();
            string t_db_name = connectionInfo[2].ToString();
            string t_username = connectionInfo[3].ToString();
            string t_password = connectionInfo[4].ToString();

            return OracleDatabase.CreateConnectionString(
                t_host,
                t_port,
                t_db_name,
                t_username,
                t_password);
        }

        /// <summary>
        /// Queries password of a given username from the Keychain accounts spreadsheet.
        /// </summary>
        /// <param name="username">Username to find password of.</param>
        /// <returns>Password of the Keychain account.</returns>
        private string QueryKeychainAccountPassword(string username)
        {
            string password = string.Empty;

            using (FileStream fileStream = new FileStream(KeychainAccountsFilePath, FileMode.Open, FileAccess.Read))
            {
                // open both XLS and XLSX
                IWorkbook excel = WorkbookFactory.Create(fileStream);
                ISheet sheet = excel.GetSheetAt(0);
                for (int col = 0; col < sheet.GetRow(0).LastCellNum; col++)
                {
                    // Find the username column
                    if (sheet.GetRow(0).GetCell(col).StringCellValue == "Email_Account")
                    {
                        for (int row = 1; row < sheet.LastRowNum; row++)
                        {
                            if (sheet.GetRow(row).GetCell(col)?.StringCellValue == username)
                            {
                                password = sheet.GetRow(row + 1).GetCell(col).StringCellValue;
                                break;
                            }
                        }
                    }
                }
            }

            return password;
        }

        /// <summary>
        /// Returns the URL to access the environment.
        /// </summary>
        /// <param name="environment">The enviornment we want to connect to.</param>
        /// <returns>The URL to access the environment.</returns>
        private string GetEnvironmentURL(string environment)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);
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
            this.ConnectToEnvDatabase(environment);
            return this.EnvDB.ExecuteQuery(query)[0][0].ToString();
        }

        /// <summary>
        /// State of the database password.
        /// </summary>
        private static class DatabasePasswordState
        {
            internal const int IsProtected = 1;

            internal const int IsNotProtected = 0;
        }
    }
}
