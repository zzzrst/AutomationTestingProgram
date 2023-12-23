// <copyright file="ChooseCollection.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using System.Collections;
    using TestingDriver;

    /// <summary>
    /// This class executes the action of getting to an organization.
    /// </summary>
    public class ChooseCollection : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Choose Collection";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string collectionDropDown = "//*[@aria-label='Choose a collection activate']";
            string collectionSearchBarXPath = "//*[@aria-label='Choose a collection']";

            string collectionElementXpath;
            string collectionName = string.Empty; // assign the collection name as empty string

            // if collection is defined as an xpath already, then no need to define the collection element xpath again
            if (this.Arguments["comment"] == "xpath")
            {
                collectionElementXpath = this.Arguments["object"];
            }
            else
            {
                collectionName = this.Arguments["object"];

                // if the object is null, then take the value.
                if (collectionName == string.Empty)
                {
                    collectionName = this.Arguments["value"];
                }

                collectionElementXpath = $"//span[contains(text(), \"{collectionName}\")]";
            }

            Logger.Info("Searching for collection element xpath: " + collectionElementXpath);
            InformationObject.TestAutomationDriver.RefreshWebPage();

            // wait for the loading spinner after refreshing
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();

            try
            {
                if (!InformationObject.TestAutomationDriver.CheckForElementState(collectionElementXpath, ITestingDriver.ElementState.Visible))
                {
                    InformationObject.TestAutomationDriver.ClickElement(collectionDropDown);
                    InformationObject.TestAutomationDriver.PopulateElement(collectionSearchBarXPath, collectionName);
                    InformationObject.TestAutomationDriver.ClickElement(collectionElementXpath);
                    InformationObject.TestAutomationDriver.WaitForLoadingSpinner();
                    this.TestStepStatus.Actual = "Successfully selected collection";
                    this.TestStepStatus.RunSuccessful = true; // rest of try clause is skipped if it fails
                }
                else
                {
                    Logger.Error("Failed to choose collection since not Visible, likely wrong xpath");
                }
            }
            catch (Exception ex)
            {
                Logger.Info("Choose collection failed");
                this.ShouldExecuteVariable = true;
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Failure in Choosing Collection";

                this.HandleException(ex);
            }
        }
    }
}
