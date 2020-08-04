// <copyright file="SqlDatabaseData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using AutomationTestSetFramework;
    using DatabaseConnector;

    /// <summary>
    /// SqlDatabase Base Object.
    /// </summary>
    public abstract class SqlDatabaseData : ITestData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDatabaseData"/> class.
        /// A Base Implementation of the SqlDatabaseData
        /// Using The DatabaseData as a Base.
        /// </summary>
        /// <param name="args">arguments to pass in.</param>
        public SqlDatabaseData(string args)
        {
        }

        /// <inheritdoc/>
        public string TestArgs { get; set; }

        /// <inheritdoc/>
        public virtual string Name { get; } = "Sql";

        /// <summary>
        /// Gets or sets the name of the test case db.
        /// </summary>
        protected string TestDBName { get; set; }

        /// <summary>
        /// Gets or sets connection established to test database.
        /// </summary>
        protected SQLDatabase TestDB { get; set; }

        /// <summary>
        /// Gets or sets connection established to environment database.
        /// </summary>
        protected SQLDatabase EnvDB { get; set; }

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
        protected virtual SQLDatabase ConnectToDatabase(SQLDatabase database)
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

                    database = new SQLDatabase(host, port, serviceName, userID, password, Logger.GetLog4NetLogger());
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
