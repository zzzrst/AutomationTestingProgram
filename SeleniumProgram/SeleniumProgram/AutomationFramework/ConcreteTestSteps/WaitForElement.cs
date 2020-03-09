// <copyright file="WaitForElement.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    /// <summary>
    /// This class executes the action of waiting for an elment's status.
    /// </summary>
    public class WaitForElement : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "WaitForElement";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.TestStepInfo.Attributes["xPath"].Value;
            bool invisible = bool.Parse(this.TestStepInfo.Attributes["invisible"].Value);

            SeleniumDriver.ElementState state = invisible ? SeleniumDriver.ElementState.Invisible : SeleniumDriver.ElementState.Visible;

            this.Driver.WaitForElementState(xPath, state);
        }
    }
}
