// <copyright file="PressKey.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to press a key.
    /// </summary>
    public class PressKey : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Press Key";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string keystroke = this.Arguments["object"];
            if (string.IsNullOrEmpty(keystroke))
            {
                keystroke = this.Arguments["value"];
            }

            InformationObject.TestAutomationDriver.SendKeys(keystroke);
        }
    }
}
