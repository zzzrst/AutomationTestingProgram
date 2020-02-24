// <copyright file="CloseBrowser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    /// <summary>
    /// This class executes the action of closing the browser.
    /// </summary>
    public class CloseBrowser : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "CloseBrowser";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            this.Driver.CloseBrowser();
        }
    }
}
