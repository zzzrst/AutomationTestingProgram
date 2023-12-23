// <copyright file="ClickImage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step to click an image.
    /// </summary>
    public class ClickImage : ClickObject
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Click Image";

        /// <inheritdoc/>
        protected override string HTMLWhiteListTag { get; set; } = "Image_HTMLTags";
    }
}
