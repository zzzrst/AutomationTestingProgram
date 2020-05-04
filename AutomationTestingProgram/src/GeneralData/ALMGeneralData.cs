// <copyright file="ALMGeneralData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.GeneralData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;
    using ALMConnector;
    using Helper;
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
            string password = ConfigurationManager.AppSettings["ALMpassword"];
            string domain = ConfigurationManager.AppSettings["ALMdomain"];
            string project = ConfigurationManager.AppSettings["ALMproject"];

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

            Dictionary<EnvVar, string> parameters = new Dictionary<EnvVar, string>();
            Connector alm = new Connector(username, password, domain, project);
            TestSetInstance testset;

            if (int.TryParse(testArgs, out int uID))
            {
                testset = alm.SetTestSetByUID(uID);
            }
            else
            {
                testset = alm.SetTestSetByPath(testArgs);
            }

            parameters.Add(EnvVar.TimeOutThreshold, testset.GetField("Global TimeOut"));
            parameters.Add(EnvVar.Attempts, testset.GetField("Global Attempts"));
            parameters.Add(EnvVar.Browser, testset.GetField("Browser"));

            string enviornment = testset.GetField("Test Environment");
            parameters.Add(EnvVar.Environment, enviornment);
            try
            {
                parameters.Add(EnvVar.URL, ConfigurationManager.AppSettings[enviornment].ToString());
            }
            catch (Exception)
            {
                Logger.Error("Missing Enviroment URL in Config File");
                throw new Exception("Missing Enviroment URL in Config File");
            }

            alm.DisconnectFromServer();

            return parameters;
        }

        /// <inheritdoc/>
        public bool Verify(string testArgs)
        {
            return true;
        }
    }
}
