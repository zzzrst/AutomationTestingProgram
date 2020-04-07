// <copyright file="RunJavaScript.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to run a java script.
    /// </summary>
    public class RunJavaScript : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "RunJavaScript";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            InformationObject.TestAutomationDriver.ExecuteJS(this.Arguments["value"]);
        }
    }
}
