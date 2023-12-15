// <copyright file="DatabaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using DatabaseConnector;

    /// <summary>
    /// The base implementation of the Database Data.
    /// </summary>
    public class DatabaseData : ITestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseData"/> class.
        /// Base constructor for the database Data.
        /// </summary>
        /// <param name="args">args to be passed in.</param>
        public DatabaseData(string args)
        {
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public string Name { get; } = "Database";

        /// <summary>
        /// Gets or sets the name of the test case db.
        /// </summary>
        protected string TestDBName { get; set; }

        /// <summary>
        /// Gets or sets connection established to test database.
        /// </summary>
        protected OracleDatabase TestDB { get; set; }

        /// <summary>
        /// Gets or sets connection established to environment database.
        /// </summary>
        protected OracleDatabase EnvDB { get; set; }

        /// <summary>
        /// Gets or sets name of the environment.
        /// </summary>
        protected string Environment { get; set; }

        /// <inheritdoc/>
        public void SetUp()
        {
            this.TestDBName = ConfigurationManager.AppSettings["DBTestCaseDatabase"].ToString();
        }

        /// <summary>
        /// connects the given database and returns it.
        /// </summary>
        /// <param name="database">The database to connect to.</param>
        /// <returns>The same database.</returns>
        protected OracleDatabase ConnectToDatabase(OracleDatabase database)
        {
            if (database == null || !database.IsConnected())
            {
                int count = 0;
                int maxTries = 2;

                // trys 2 times
                while (count < maxTries)
                {
                    string host = ConfigurationManager.AppSettings["DBHost"].ToString();
                    string port = ConfigurationManager.AppSettings["DBPort"].ToString();
                    string serviceName = ConfigurationManager.AppSettings["DBServiceName"].ToString();
                    string userID = ConfigurationManager.AppSettings["DBUserId"].ToString();
                    string password = ConfigurationManager.AppSettings["DBPassword"].ToString();

                    Logger.Info($"Attempting to connect to host:{host}, port:{port}, service:{serviceName}, user:{userID}, password:{password}");

                    database = new OracleDatabase(host, port, serviceName, userID, password, Logger.GetLog4NetLogger());
                    database.Connect();
                    if (database.IsConnected())
                    {
                        Logger.Info($"Connected to database: {serviceName}");
                        break;
                    }

                    count++;
                }

                if (count > maxTries)
                {
                Logger.Warn($"Failed to Connected to database");
                }
            }

            return database;
        }
    }
}
