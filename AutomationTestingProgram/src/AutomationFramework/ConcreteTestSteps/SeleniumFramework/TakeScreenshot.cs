// <copyright file="TakeScreenshot.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System.IO;
    using System.Reflection;
    using System;

    /// <summary>
    /// This test step to compare pdfs.
    /// </summary>
    public class TakeScreenshot : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "TakeScreenshot";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string fileName = $"{DateTime.Now:yyyy_MM_dd-hh_mm_ss_tt}.png";

            string path_ex = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            InformationObject.TestAutomationDriver.TakeEntireScreenshot($"{path_ex}\\Log\\NEW_{fileName}", false);

            InformationObject.Reporter.AddTestStepScreenshot("Screenshot.png", $"{path_ex}\\Log\\NEW_{fileName}", "screenshot after taking screenshot");
        }
    }
}
