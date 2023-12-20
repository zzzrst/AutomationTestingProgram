﻿// <copyright file="VerifyDDLContent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This test step to verify the content of a drop down list.
    /// </summary>
    public class VerifyDDLContent : ActionObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyDDLContent";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "WebList_HTMLTag";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            List<string> expected = this.Arguments["value"].Split(';').ToList();

            this.TestStepStatus.RunSuccessful = InformationObject.TestAutomationDriver.VerifyDropDownContent(expected, this.XPath, this.JsCommand);

            if (this.TestStepStatus.RunSuccessful)
            {
                this.TestStepStatus.Actual = "Successfully verified DDL Content xpath: " + this.XPath;
            }
            else
            {
                this.TestStepStatus.Actual = "Failure in Verifying DDL Content";

                throw new Exception(this.TestStepStatus.Actual);
            }
        }
    }
}
