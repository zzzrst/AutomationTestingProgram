// <copyright file="DatabaseCaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestingProgram.Exceptions;
    using AutomationTestingProgram.Helper;
    using AutomationTestSetFramework;
    using DatabaseConnector;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// A concrete implementation of the ITestCaseData for databases.
    /// </summary>
    public class DatabaseCaseData : ITestCaseData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCaseData"/> class.
        /// </summary>
        /// <param name="args">Args passe in.</param>
        public DatabaseCaseData(string args)
        {
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Database";

        /// <summary>
        /// Gets or sets connection established to test database.
        /// </summary>
        private OracleDatabase TestDB { get; set; }

        /// <summary>
        /// Gets or sets connection established to environment database.
        /// </summary>
        private OracleDatabase EnvDB { get; set; }

        /// <summary>
        /// Gets or sets name of the environment.
        /// </summary>
        private string Environment { get; set; }

        /// <summary>
        /// Gets or sets the name of the enviornment database.
        /// </summary>
        private string EnvDBName { get; set; }

        /// <summary>
        /// Gets or sets the name of the test case db.
        /// </summary>
        private string TestDBName { get; set; }

        /// <summary>
        /// Gets or sets list of test steps to run.
        /// </summary>
        private Queue<TestStep> TestSteps { get; set; }

        /// <summary>
        /// Gets the SKIP.
        /// </summary>
        private string SKIP { get; } = "#";

        private string EnviroDBName { get; set; }

        /// <inheritdoc/>
        public void SetUp()
        {
            this.EnvDBName = ConfigurationManager.AppSettings["DBEnvDatabase"].ToString();
            this.TestDBName = ConfigurationManager.AppSettings["DBTestCaseDatabase"].ToString();
        }

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            return this.TestSteps.Count > 0;
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            return this.TestSteps.Dequeue();
        }

        /// <inheritdoc/>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            return this.CreateTestCase(testCaseName);
        }

        /// <summary>
        /// Creates a new test step.
        /// </summary>
        /// <param name="testCaseName">The name of the test step.</param>
        /// <returns>The test case.</returns>
        private ITestCase CreateTestCase(string testCaseName)
        {
            string collection = "0";
            string release = "0";
            try
            {
                this.TestSteps = new Queue<TestStep>();
                List<List<object>> table = this.QueryTestCase(testCaseName, collection, release);

                foreach (List<object> row in table)
                {
                    TestStep testStep = this.CreateTestStep(row);
                    this.TestSteps.Enqueue(testStep);
                }

                // create and return test case
                ITestCase testCase = new TestCase()
                {
                    Name = testCaseName,
                };

                return testCase;
            }
            catch (TestActionNotFound tanf)
            {
                throw tanf;
            }
            catch (Exception e)
            {
                throw new TestCaseCreationFailed(e.ToString());
            }
        }

        /// <summary>
        /// The A Test Step.
        /// </summary>
        /// <param name="row">The row<see cref="T:List{object}"/>.</param>
        /// <returns>The <see cref="ITestStep"/>.</returns>
        private TestStep CreateTestStep(List<object> row)
        {
            TestStep testStep;

            // // ignore TESTCASE
            string testStepDesc = row[1]?.ToString() ?? string.Empty;   // TESTCASEDESCRIPTION
            string action = row[3]?.ToString() ?? string.Empty;         // ACTIONONOBJECT (test action)
            string obj = row[4]?.ToString() ?? string.Empty;    // OBJECT
            string value = row[5]?.ToString() ?? string.Empty;          // VALUE (of the control/field)
            string comment = row[6]?.ToString() ?? string.Empty;      // COMMENTS (selected attribute)

            string stLocAttempts = row[8]?.ToString() ?? "0"; // LOCAL_ATTEMPTS
            string stLocTimeout = row[9]?.ToString() ?? "0";  // LOCAL_TIMEOUT
            string control = row[10]?.ToString() ?? string.Empty;       // CONTROL

            string testStepType = row[12]?.ToString() ?? "0"; // TESTSTEPTYPE (formerly SEVERITY)
            string goToStep = row[13]?.ToString() ?? string.Empty;      // GOTOSTEP

            int localAttempts = int.Parse(string.IsNullOrEmpty(stLocAttempts) ? "0" : stLocAttempts);
            if (localAttempts == 0)
            {
                localAttempts = 1; // alm.AlmGlobalAttempts;
            }

            int localTimeout = int.Parse(string.IsNullOrEmpty(stLocTimeout) ? "0" : stLocTimeout);
            if (localTimeout == 0)
            {
                localTimeout = int.Parse(GetEnvironmentVariable(EnvVar.TimeOutThreshold)); // alm.AlmGlobalTimeOut;
            }

            int testStepTypeId = int.Parse(string.IsNullOrEmpty(testStepType) ? "0" : testStepType);
            if (testStepTypeId == 0)
            {
                testStepTypeId = 1;
            }

            testStep = ReflectiveGetter.GetEnumerableOfType<TestStep>()
                .Find(x => x.Name.Equals(action));

            testStep.Description = testStepDesc;
            testStep.Arguments.Add("object", Helper.Cleanse(obj));
            testStep.Arguments.Add("value", Helper.Cleanse(value));
            testStep.Arguments.Add("comment", Helper.Cleanse(comment));
            testStep.MaxAttempts = localAttempts;
            testStep.ShouldExecuteVariable = control == this.SKIP;

            return testStep;
        }

        /// <summary>
        /// Queries a test case from the test database given the testcase name, collection, and release.
        /// </summary>
        /// <param name="testcase">Name of the testcase.</param>
        /// <param name="collection">Collection that the testcase is part of.</param>
        /// <param name="release">Release of the collection.</param>
        /// <returns>A test case from the test database.</returns>
        private List<List<object>> QueryTestCase(string testcase, string collection, string release)
        {
            this.TestDB = this.ConnectToDatabase(this.TestDB);
            string query = $"SELECT T.TESTCASE, T.TESTSTEPDESCRIPTION, T.STEPNUM, T.ACTIONONOBJECT, T.OBJECT, T.VALUE, T.COMMENTS, T.RELEASE, T.LOCAL_ATTEMPTS, T.LOCAL_TIMEOUT, T.CONTROL, T.COLLECTION, T.TEST_STEP_TYPE_ID, T.GOTOSTEP FROM {this.TestDBName} T WHERE T.TESTCASE = '{testcase}' AND T.COLLECTION = '{collection}' AND T.RELEASE = '{release}' ORDER BY T.STEPNUM";
            Logger.Info("Querying the following: [" + query + "]");
            var result = this.TestDB.ExecuteQuery(query);
            this.TestDB.Disconnect();
            Logger.Info("Closed connection to database.\n");
            if (result == null || result.Count == 0)
            {
                throw new DatabaseTestCaseNotFound("Database Test Case Not Found");
            }

            return result;
        }

        /// <summary>
        /// connects the given database and returns it.
        /// </summary>
        private OracleDatabase ConnectToDatabase(OracleDatabase database)
        {
            if (database == null || !database.IsConnected())
            {
                int count = 0;

                // trys 3 times
                while (count < 3)
                {
                    string host = ConfigurationManager.AppSettings["DBHost"].ToString();
                    string port = ConfigurationManager.AppSettings["DBPort"].ToString();
                    string serviceName = ConfigurationManager.AppSettings["DBServiceName"].ToString();
                    string userID = ConfigurationManager.AppSettings["DBUserId"].ToString();
                    string password = ConfigurationManager.AppSettings["DBPassword"].ToString();
                    database = new OracleDatabase(host, port, serviceName, userID, password);
                    database.Connect();
                    if (database.IsConnected())
                    {
                        Logger.Info($"Connected to database: {serviceName}");
                        break;
                    }

                    count++;
                }
            }

            return database;
        }

        private void ConnectToDatabase(OracleDatabase envDB, string environment)
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
                        Logger.Info("Connected to database: " + this.EnviroDBName);
                        break;
                    }

                    count++;
                }
            }
        }

        /// <summary>
        /// Creates the connection string to connect to the environment database.
        /// </summary>
        /// <param name="environment">Name of environment to connect to.</param>
        /// <returns>The connection string to connect to the environment database.</returns>
        private string CreateEnvironmentConnectionString(string environment)
        {
            List<object> connectionInfo = this.QueryEnvironmentConnectionInformation(environment);
            this.EnviroDBName = connectionInfo[2].ToString();

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
        /// Queries the information needed to build the connection string.
        /// </summary>
        /// <param name="environment">Name of environment to connect to.</param>
        /// <returns>The information needed to build the connection string.</returns>
        private List<object> QueryEnvironmentConnectionInformation(string environment)
        {
            this.ConnectToDatabase(this.TestDB);

            // we add t.is_password_encrypted to be able to check if the password is encrypted or not.
            string query = $"select t.host, t.port, t.db_name, t.username, t.password, t.is_password_encrypted from {this.EnvDBName} t where t.environment = '{environment}'";

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
        /// State of the database password.
        /// </summary>
        private static class DatabasePasswordState
        {
            internal const int IsProtected = 1;

            internal const int IsNotProtected = 0;
        }
    }
}
