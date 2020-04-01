﻿namespace AutomationTestingProgram.AutomationFramework.ConcreteTestSteps
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// test step to accept Alerts.
    /// </summary>
    public class AcceptAlert : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "Accept Alert";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();
            throw new NotImplementedException();
            /*this.BrowserDriver.AcceptAlert();*/
        }
    }
}
