// <copyright file="LaunchBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Security.Policy;
    using System.Threading; // added by Victor
    using AutomationTestingProgram.TestingData.TestDrivers;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class LaunchBrowser : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Launch Browser";

        /// <inheritdoc/>
        public override void Execute()
        {
            Logger.Info($"Launching Browser");
            base.Execute();

            // this section was edited by Victor
            // if the value is defined as @, it should query the DB, otherwise keep the value
            // string url = this.Arguments.ContainsKey("value") ? this.Arguments["value"] : string.Empty;
            string url = string.Empty; // this was configured in original TAP

            if (this.Arguments.ContainsKey("value"))
            {
                // we will edit this to follow the SeleniumFramework
                if (this.Arguments["value"] == "@")
                {
                    // we assume that the type must be DB
                    string environment = GetEnvironmentVariable(EnvVar.Environment);

                    DatabaseStepData dbdata = new DatabaseStepData("");
                    url = dbdata.GetEnvironmentURL(environment);
                    // url = ((DatabaseStepData)TestStepData).GetEnvironmentURL(environment);
                }
                else
                {
                    // if it's hard coded, then we will use it
                    url = this.Arguments["value"];
                }

                if (url == string.Empty)
                {
                    this.TestStepStatus.RunSuccessful = false;
                    Logger.Error("URL is empty");
                    return;
                }
            }

            try
            {
                InformationObject.TestAutomationDriver.NavigateToURL(url);

                // we want to make it full screen, added by Victor
                // InformationObject.TestAutomationDriver.Maximize();

                // we should wait for the page to load
                Thread.Sleep(5 * 1000); // sleep for 2 seconds, can be changed for testing purposes

                // we want to make sure that the browser is loaded be fore proceeding
                InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
            }
            catch (System.NullReferenceException)
            {
                this.TestStepStatus.RunSuccessful = false;
                Logger.Error("Missing Chromium Folder");
            }

            // handle exception for failing to launch browser
            catch (Exception e)
            {
                Logger.Error("Failed to launch browser");
                base.HandleException(e);
            }

            Logger.Info($"Launch browser to url {url}");
        }
    }
}
