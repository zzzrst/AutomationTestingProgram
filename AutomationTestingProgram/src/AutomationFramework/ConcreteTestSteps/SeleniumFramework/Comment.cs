// <copyright file="Comment.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    /// <summary>
    /// This test step is a comment.
    /// </summary>
    public class Comment : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Comment";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
        }
    }
}
