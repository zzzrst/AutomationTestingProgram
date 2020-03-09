﻿// <copyright file="WaitInSeconds.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.Threading;

    /// <summary>
    /// This class executes the action of waiting for x seconds.
    /// </summary>
    public class WaitInSeconds : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "WaitInSeconds";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            int seconds = int.Parse(this.TestStepInfo.Attributes["seconds"].Value);
            Thread.Sleep(seconds);
        }
    }
}
