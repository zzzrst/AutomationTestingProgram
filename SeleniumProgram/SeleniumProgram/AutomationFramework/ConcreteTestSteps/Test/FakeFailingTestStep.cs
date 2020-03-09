// <copyright file="FakeFailingTestStep.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXMLNUnitTest
{
    using System;
    using SeleniumPerfXML.Implementations;

    /// <summary>
    /// This test step will always fail.
    /// </summary>
    public class FakeFailingTestStep : TestStepXml
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
