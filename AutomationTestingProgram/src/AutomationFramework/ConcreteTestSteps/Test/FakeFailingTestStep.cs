// <copyright file="FakeFailingTestStep.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;

    /// <summary>
    /// This test step will always fail.
    /// </summary>
    public class FakeFailingTestStep : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "FakeFailingTestStep";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            throw new Exception("Fake Test Step Failed!");
        }
    }
}
