// <copyright file="SelectDDL.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This class executes the action of selecting a value from the specified dropdownlist.
    /// </summary>
    /// I think this was configured incorrectly, changed from Test Step to ActionObject
    // public class SelectDDL : ActionObject
    public class SelectDDL : ActionObject
    {
        /// <inheritdoc/>e
        public override string Name { get; set; } = "Select DDL";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebList_HTMLTag";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            // this is possibly not an xpath and rather an html id
            // string xPath = this.Arguments["object"];
            string selection = this.Arguments["value"];

            Console.WriteLine("xpath value: " + this.XPath);
            Console.WriteLine("selection: " + selection);

            // InformationObject.TestAutomationDriver.SelectValueInElement(xPath, selection);
            InformationObject.TestAutomationDriver.SelectValueInElement(this.XPath, selection);
            InformationObject.TestAutomationDriver.WaitForLoadingSpinner();

            this.TestStepStatus.Actual += " successfully clicked " + selection + " using xpath" + this.XPath;
            this.TestStepStatus.RunSuccessful = true;
        }
    }
}
