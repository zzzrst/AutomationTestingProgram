// <copyright file="DatabaseCaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using AutomationTestSetFramework;
    using DatabaseConnector;

    /// <summary>
    /// A concrete implementation of the ITestCaseData for databases.
    /// </summary>
    public class DatabaseCaseData : ITestCaseData
    {
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

        /// <inheritdoc/>
        public bool ExistNextTestStep()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestStep GetNextTestStep()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITestCase SetUpTestCase(string testCaseName, bool performAction = true)
        {
            throw new NotImplementedException();
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
            string query = "SELECT T.TESTCASE, T.TESTSTEPDESCRIPTION, T.STEPNUM, T.ACTIONONOBJECT, T.OBJECT, T.VALUE, T.COMMENTS, T.RELEASE, T.LOCAL_ATTEMPTS, T.LOCAL_TIMEOUT, T.CONTROL, T.COLLECTION, T.TEST_STEP_TYPE_ID, T.GOTOSTEP FROM QA_AUTOMATION.TESTCASE T WHERE T.TESTCASE = '" + testcase + "' AND T.COLLECTION = '" + collection + "' AND T.RELEASE = '" + release + "' ORDER BY T.STEPNUM";
            Logger.Info("Querying the following: [" + query + "]");
            var result = this.TestDB.ExecuteQuery(query);
            this.TestDB.Disconnect();
            Logger.Info("Closed connection to database.\n");
            if (result == null || result.Count == 0)
            {
                throw new Exception("Database Test Case Not Found");
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
                        Logger.Info("Connected to database: RVDEV1");
                        break;
                    }

                    count++;
                }
            }

            return database;
        }
    }
}
