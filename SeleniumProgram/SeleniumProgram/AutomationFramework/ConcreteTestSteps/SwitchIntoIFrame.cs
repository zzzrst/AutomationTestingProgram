// <copyright file="SwitchIntoIFrame.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    /// <summary>
    /// This class executes the action of switching context into IFrame.
    /// </summary>
    public class SwitchIntoIFrame : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SwitchIntoIFrame";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.TestStepInfo.Attributes["xPath"].Value;
            this.Driver.SwitchToIFrame(xPath);
        }
    }
}
