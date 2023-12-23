// <copyright file="Login.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using NPOI.HSSF.Record;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step to log in.
    /// </summary>
    public class ResetOrg : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ResetOrg";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // org code example: Canadore College (CANA)
            string orgCode = this.Arguments["object"];

            // string value = this.Arguments["value"];
            // string environment = GetEnvironmentVariable(EnvVar.Environment);

            string adminToolkit = "//*[@id='MenuAdminToolkit']";
            string manageOrgs = "//*[@id='ManageOrgs']/a";

            // string curParticpating = "//span[contains(text(), 'Canadore College (CANA)')]/span";

            string setParticipatingButton = $"//span[contains(text(), '{orgCode}')]/following::button[contains(.,'Participating')][1]";
            string setNotParticipatingButton = $"//span[contains(text(), '{orgCode}')]/following::button[contains(.,'Participating')][1]";
            string reviewChangesButton = "//*[@id='ReviewChangesButton']";
            string saveOrgButton = "//*[@id='SaveOrgsButton']";
            string filterNotParticipatingButton = "//*[@id='FilterNonParticipatingButton']";
            string menuHome = "//*[@id='MenuHome']";

            InformationObject.TestAutomationDriver.ClickElement(adminToolkit);
            InformationObject.TestAutomationDriver.ClickElement(manageOrgs);
            InformationObject.TestAutomationDriver.ClickElement(setNotParticipatingButton);
            InformationObject.TestAutomationDriver.ClickElement(reviewChangesButton);
            InformationObject.TestAutomationDriver.ClickElement(saveOrgButton);
            InformationObject.TestAutomationDriver.ClickElement(filterNotParticipatingButton);
            InformationObject.TestAutomationDriver.ClickElement(setParticipatingButton);
            InformationObject.TestAutomationDriver.ClickElement(reviewChangesButton);
            InformationObject.TestAutomationDriver.ClickElement(saveOrgButton);

            InformationObject.TestAutomationDriver.ClickElement(menuHome);

            // wait for the spinner to finish loading
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
        }
    }
}
