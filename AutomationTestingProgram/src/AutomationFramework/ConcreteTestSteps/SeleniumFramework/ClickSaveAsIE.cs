// <copyright file="ClickSaveAsIE.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
/*
namespace AutomationTestingProgram.AutomationFramework
{
    using AutoIt;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class ClickSaveAsIE : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ClickSaveAsIE";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            if (GetEnvironmentVariable(EnvVar.Browser).ToLower().Contains("ie"))
            {
                string tabName = TestAutomationDriver.WebDriver.Title;

                tabName = tabName + " - Internet Explorer";

                this.ActivateWindow(tabName, 30);
                AutoItX.ControlFocus(tabName, string.Empty, "[CLASS: Internet Explorer_Server; INSTANCE: 1]");
                AutoItX.ControlClick(tabName, string.Empty, "[CLASS: Internet Explorer_Server; INSTANCE: 1]");
                AutoItX.Send("!N");
                AutoItX.Sleep(300);
                AutoItX.Send("{DOWN}");
                AutoItX.Sleep(300);
                AutoItX.Send("{DOWN}");
                AutoItX.Sleep(300);
                AutoItX.Send("{ENTER}");
                AutoItX.Sleep(300);
            }
        }

        /// <summary>
        /// Activates (gives focus to) a window under the duration of the timeout. If the window is a dialog modal,
        /// then this method also the selects the input box.
        /// </summary>
        /// <param name="windowName">Name or title of the window.</param>
        /// <param name="timeout">Duration of timeout to activate the window.</param>
        /// <returns>True if the window is activated.</returns>
        private bool ActivateWindow(string windowName, int timeout)
        {
            AutoItX.Sleep(500);
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            stopWatch.Start();
            var start = stopWatch.Elapsed.TotalSeconds;
            while ((stopWatch.Elapsed.TotalSeconds - start) < timeout && AutoItX.WinActive(windowName) == 0)
            {
                AutoItX.WinActivate(windowName);
            }

            return AutoItX.WinActive(windowName) != 0;
        }
    }
}
*/