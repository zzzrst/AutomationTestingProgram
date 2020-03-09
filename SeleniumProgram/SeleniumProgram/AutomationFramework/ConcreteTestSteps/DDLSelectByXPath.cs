// <copyright file="DDLSelectByXPath.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    /// <summary>
    /// This class executes the action of selecting a value from the specified dropdownlist.
    /// </summary>
    public class DDLSelectByXPath : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "DDLSelectByXPath";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string xPath = this.TestStepInfo.Attributes["xPath"].Value;
            string selection = this.TestStepInfo.Attributes["selection"].Value;
            this.Driver.SelectValueInElement(xPath, selection);
            this.Driver.WaitForLoadingSpinner();
        }
    }
}
