// <copyright file="ALMGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using ALMConnector;
    using AutomationTestingProgram.Helper;
    using TDAPIOLELib;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// An implementation of the General Data for ALM.
    /// </summary>
    public class ALMGeneralData : ITestGeneralData
    {
        /// <inheritdoc/>
        public string Name { get; } = "ALM";

        /// <inheritdoc/>
        public Dictionary<EnvVar, string> ParseParameters(string testArgs, string dataFile)
        {
            string username = ConfigurationManager.AppSettings["ALMusername"];
            string domain = ConfigurationManager.AppSettings["ALMdomain"];
            string project = ConfigurationManager.AppSettings["ALMproject"];
            string password = this.DecriptPassword(ConfigurationManager.AppSettings["ALMpassword"]);

            Dictionary<EnvVar, string> parameters = new Dictionary<EnvVar, string>();
            Connector alm = new Connector(username, password, domain, project);
            TestSetInstance testset = this.GetTestSetInstance(testArgs, alm);

            parameters.Add(EnvVar.TimeOutThreshold, testset.GetField("Global TimeOut"));
            parameters.Add(EnvVar.Attempts, testset.GetField("Global Attempts"));
            parameters.Add(EnvVar.Browser, testset.GetField("Browser"));

            string enviornment = testset.GetField("Test Environment");
            parameters.Add(EnvVar.Environment, enviornment);

            string collection = testset.GetField("Application Collection");
            string release = testset.GetField("Test Case Version");

            // note this was added by Victor and doesn't appear to work. Currently only able to read from command line.
            if (TestSetInstance.buildNumber == "my build number")
            {
                TestSetInstance.buildNumber = testset.GetField("Build Number");
                parameters.Add(EnvVar.BuildNumber, testset.GetField("Build Number"));
            }

            parameters.Add(EnvVar.TestCaseDataArgs, $"{collection},{release}");

            // note that this currently defaults to the App.Config file, but ideally the default is DB then the Config File.
            if (ConfigurationManager.AppSettings.AllKeys.Contains(enviornment))
            {
                parameters.Add(EnvVar.URL, ConfigurationManager.AppSettings[enviornment].ToString());
            }
            else
            {
                Logger.Info($"Missing Enviroment URL for {enviornment} in Config File");
            }

            alm.DisconnectFromServer();

            return parameters;
        }

        /// <inheritdoc/>
        public bool Verify(string testArgs)
        {
            return true;
        }

        private TestSetInstance GetTestSetInstance(string testArgs, Connector alm)
        {
            TestSetInstance testset;
            if (int.TryParse(testArgs, out int uID))
            {
                testset = alm.SetTestSetByUID(uID);
            }
            else
            {
                testset = alm.SetTestSetByPath(testArgs);
            }

            return testset;
        }

        private string DecriptPassword(string password)
        {
            // check if password is encrypted
            try
            {
                if (bool.Parse(ConfigurationManager.AppSettings["passwordEncrypted"]))
                {
                    password = Helper.DecryptString(password, Environment.MachineName);
                }
            }
            catch (Exception)
            {
                Logger.Warn("Configuration passwordEncrypted has the wrong value. Use true or false as values");
            }

            return password;
        }
    }
}
