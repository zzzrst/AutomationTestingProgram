// <copyright file="MaximizeBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    /// <summary>
    /// This class executes the action of closing the current tab.
    /// </summary>
    public class MaximizeBrowser : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "MaximizeBrowser";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            this.Driver.Maximize();
        }
    }
}
