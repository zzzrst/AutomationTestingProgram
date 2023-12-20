// <copyright file="RunJavaScript.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using AutomationTestingProgram.TestingData;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using NPOI.HSSF.Record;
    using NPOI.SS.Formula.Functions;
    using static AutomationTestingProgram.InformationObject;
    
    /// <summary>
    /// This test step skips to a specific line in the excel. ONLY WORKS FOR EXCEL
    /// </summary>
    public class SkipToStep : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "SkipToStep";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string value = this.Arguments["value"];

            if (value == string.Empty)
            {
                Logger.Error("Wrong use of ActionOnObject, value should be filled");
                this.TestStepStatus.RunSuccessful = false;

                return;
            }

            int skipToLine = int.Parse(value);

            // set go to step as true
            ((ExcelCaseData)TestCaseData).GoToTestStep = 1;
            ((ExcelCaseData)TestCaseData).NextTestStepPass = skipToLine;
            ExcelData.RowIndex = skipToLine;

            this.TestStepStatus.RunSuccessful = true;
            this.TestStepStatus.Actual = "Successfully set next line in excel to: " + skipToLine;

            // string password = ((DatabaseStepData)TestStepData).QuerySpecialChars(environment, value).ToString();
            // InformationObject.TestAutomationDriver.ExecuteJS(this.Arguments["value"]);
        }
    }
}
