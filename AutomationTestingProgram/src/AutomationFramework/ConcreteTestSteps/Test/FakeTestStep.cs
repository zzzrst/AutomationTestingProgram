// <copyright file="FakeTestStep.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class FakeTestStep : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "FakeTestStep";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
        }
    }
}
