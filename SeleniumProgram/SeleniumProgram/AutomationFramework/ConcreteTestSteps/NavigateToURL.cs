// <copyright file="NavigateToURL.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumPerfXML.Implementations
{
    /// <summary>
    /// This class executes the action of opening the browser to the specified site.
    /// </summary>
    public class NavigateToURL : TestStepXml
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "NavigateToURL";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            string url = this.TestStepInfo.Attributes["url"].Value;
            this.Driver.NavigateToURL(url, false);
        }
    }
}
