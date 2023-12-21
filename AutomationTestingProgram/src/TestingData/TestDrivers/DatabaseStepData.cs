// <copyright file="DatabaseStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.Exceptions;
    using AutomationTestingProgram.Helper;
    using AutomationTestinProgram.Helper;
    using AutomationTestSetFramework;
    using DatabaseConnector;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;
    using TDAPIOLELib;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// The test step for the database data.
    /// </summary>
    public class DatabaseStepData : DatabaseData, ITestStepData
    {
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

        /// <summary>
        /// Gets or sets the name of the enviornment database.
        /// </summary>
        private string EnvDBName { get; set; }

        /// <inheritdoc/>
        public void SetArguments(TestStep testStep)
        {
            // run for at most number of
            // attempts, until test action passes
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

            /* Note that this might work? Was here before
            if (testStep is ActionObject)
            {
                // query to update each of the test object's attribute value
                foreach (string attribute in ((ActionObject)testStep).Attributes.Keys.ToList())
                {
                    if (((ActionObject)testStep).Attributes.TryGetValue(attribute, out string value))
                    {
                        string attribVal = value;
                        string queried = this.QuerySpecialChars(environment, attribVal) as string;
                        ((ActionObject)testStep).Attributes.Add(attribute, queried);
                    }
                }
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
                try
                {
                    result = Helper.DecryptString(result, globalVariableName + rowid);
                }
                catch
                {
                    Logger.Info("Error with decrypting string");
                }
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
        /// Returns the URL to access the environment.
        /// </summary>
        /// <param name="environment">The enviornment we want to connect to.</param>
        /// <returns>The URL to access the environment.</returns>
        public string GetEnvironmentURL(string environment)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);

            // Updated by Victor to reflect Selenium
            // string query = $"select t.url from {this.EnvDBName} t where t.environment = '{environment}'";
            string query = $"select t.url from QA_AUTOMATION.test_environments t where t.environment = '{environment}'";
            List<List<object>> result = this.TestDB.ExecuteQuery(query);
            if (result.Count == 0)
            {
                Logger.Info($"Environment provided '{environment}' is not in DB table, querying App.Config!");
                //result = new List<List<object>>();
                // return the value if it exists in the app seetings folder.
                return ConfigurationManager.AppSettings[environment];
            }

            return result[0][0].ToString();
        }

        /// <summary>
        /// Gets the environment sharepoint mapping from the database.
        /// </summary>
        /// <param name="environment">The environment it is associated with.</param>
        /// <param name="mappingName">Name of the sharepoint mapping.</param>
        /// <returns>4 required info used by Sharepoint to get information.</returns>
        public List<string> GetEnvironmentSharePointMapping(string environment, string mappingName)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);

            // Get SharePoint mapping info.
            string query = "SELECT T.SP_MAP_ROOT_SITE, T.SP_MAP_LIST, T.SP_MAP_TARGET_COL_FIELD_REF, " +
                                   "M.SP_MAP_TRGT_COL_VAL, T.SP_MAP_COL_FIELD_REF " +
                            "FROM SP_MAP_TYPE T " +
                            "NATURAL JOIN(ENV_SP_MAP M) " +
                           $"WHERE T.SP_MAP_TYPE_NAME = '{mappingName}' " +
                           $"AND M.ENVIRONMENT = '{environment}'";

            List<List<object>> mappingInfoList = this.TestDB.ExecuteQuery(query);

            if (mappingInfoList.Count == 0)
            {
                throw new Exception($"Provided environment '{environment}' and mapping {mappingName} does not exist.");
            }

            List<string> result = new List<string>();
            foreach (string info in mappingInfoList[0])
            {
                result.Add(info);
            }

            return result;
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

            // Not certain taht the environment variable is set correctly
            // string token = "$(environment)";
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
                string username = original.Substring(2); // we remove the two hashes

                result = CSVEnvironmentGetter.QueryKeychainAccountPassword(username);

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

            // call on parameter to replace content between parameter
            else if (original.Contains("{") && original.Contains("}"))
            {
                //int instances;
                string currString;
                Console.WriteLine("Original is: " + original);

                while (original.Length - original.Replace("{", string.Empty).Length > 0 &&
                       original.Length - original.Replace("}", string.Empty).Length > 0)
                {
                    currString = original.Substring(0); // make a copy of the string

                    int indexBegin = currString.IndexOf("{");
                    int indexEnd = currString.IndexOf("}");

                    string keyPair = currString.Substring(indexBegin + 1, indexEnd - indexBegin - 1);

                    if (InformationObject.RunParameters.ContainsKey(keyPair))
                    {
                        string newValue = InformationObject.RunParameters[keyPair];
                        // replace all instances
                        original = original.Replace(currString.Substring(indexBegin, indexEnd - indexBegin + 1), newValue);
                        Console.WriteLine("Successfully queried for value: " + newValue);
                    }
                    else
                    {
                        // could be not successfully queried because of Press Key using same format
                        Console.WriteLine("Not successfully queried");
                        break;
                    }
                    // set the next substring and continue
                    //original = currString.Substring(indexEnd, currString.Length - 1);
                }
                // make a copy of original that has been modified as the final result
                result = original.Substring(0);
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
        internal bool ExecuteEnvironmentNonQuery(string environment, string sql, bool fromFile)
        {
            this.ConnectToEnvDatabase(environment);

            if (fromFile)
            {
                string cmd = this.CreateSQLPlusCommand(environment, sql);

                Logger.Info($"Executing following sql in {environment}: {sql}");

                // Logger.Info("Command " +  cmd); // this is the command that is executed

                return this.EnvDB.ExecuteNonQuery(cmd);
            }
            else
            {
                return this.EnvDB.ExecuteNonQuery(sql);
            }
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
            List<object> connectionInfo = new List<object>();
            int t_isPasswordEncrypted;
            string t_password;
            string t_username;
            string t_db_name;

            // if we are using the DB to get the environment info, then continue, otherwise use CSV file.
            if (System.Configuration.ConfigurationManager.AppSettings["ENV_LOCATION"].ToUpper() == "DB")
            {
                this.TestDB = this.ConnectToDatabase(this.TestDB);

                // we add t.is_password_encrypted to be able to check if the password is encrypted or not.
                string query = $"select t.host, t.port, t.db_name, t.username, t.password, t.is_password_encrypted from {ConfigurationManager.AppSettings["DBEnvDatabase"]} t where t.environment = '{environment}'";
                Logger.Info($"Querying for QueryEnvironmentConnectionInformation : [{query}]");

                // decrypt password if needed.
                List<List<object>> result = this.TestDB.ExecuteQuery(query);

                if (result.Count == 0)
                {
                    throw new Exception($"Environment provided '{environment}' is not in table!");
                }

                connectionInfo = result[0];
                string t_host = connectionInfo[0].ToString();
                string t_port = connectionInfo[1].ToString();
                t_db_name = connectionInfo[2].ToString();
                t_username = connectionInfo[3].ToString();
                t_password = connectionInfo[4].ToString();
                t_isPasswordEncrypted = int.Parse(connectionInfo[5].ToString());
            }
            else if (System.Configuration.ConfigurationManager.AppSettings["ENV_LOCATION"].ToUpper() == "CSV")
            {
                connectionInfo.Add(CSVEnvironmentGetter.GetHost(environment));
                Logger.Info("Got port value" + connectionInfo[0]);
                connectionInfo.Add(CSVEnvironmentGetter.GetPort(environment));
                Logger.Info("Got port value" + connectionInfo[0]);

                connectionInfo.Add(CSVEnvironmentGetter.GetDBName(environment));
                connectionInfo.Add(CSVEnvironmentGetter.GetUsername(environment));
                connectionInfo.Add(CSVEnvironmentGetter.GetPassword(environment));
                connectionInfo.Add(CSVEnvironmentGetter.GetIsEncrypted(environment));

                t_db_name = connectionInfo[2].ToString();
                t_username = connectionInfo[3].ToString();
                t_password = connectionInfo[4].ToString();
                t_isPasswordEncrypted = int.Parse(connectionInfo[5].ToString());
            }
            else
            {
                Logger.Error("App.config value ENV_LOCATION not set");
                return null;
            }

            try
            {
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
            }
            catch (Exception ex)
            {
                Logger.Error("Exception caught in decrypting password: " + ex.Message);
                Logger.Error("Decrypting password is incorrect, check whether already decrypted!");
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
        /// Executes the given query in an environment database.
        /// </summary>
        /// <param name="environment">Name of environment to execute in.</param>
        /// <param name="query">Query to execute.</param>
        /// <returns>The data queried from the database.</returns>
        private string ProcessEnvironmentQuery(string environment, string query)
        {
            this.ConnectToEnvDatabase(environment);

            string queryString;
            try
            {
                queryString = this.EnvDB.ExecuteQuery(query)[0][0].ToString();
            }
            catch (Exception e)
            {
                Logger.Error("Environment query failed because no results returned");
                throw;
            }

            return queryString;
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
