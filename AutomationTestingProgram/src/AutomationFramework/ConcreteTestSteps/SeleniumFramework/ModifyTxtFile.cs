// <copyright file="ModifyTxtFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using TextInteractor;

    /// <summary>
    /// This test step to modify txt file.
    /// </summary>
    public class ModifyTxtFile : TestStep
    {
        /// <inheritdoc/>
        public override string Name { get; set; } = "ModifyTxtFile";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string txt1 = this.Arguments["object"];
            string comments = this.Arguments["comment"];
            string value = this.Arguments["value"];
            bool pass = false;
            TextInteractor txtfile = new TextInteractor(txt1, Logger.GetLog4NetLogger());

            string seperator = "];[";
            string toReplace = value.Substring(0, value.IndexOf(seperator));
            string replaceWith = value.Substring(value.IndexOf(seperator) + 3);

            switch (comments.ToLower())
            {
                case "0": case "replaceonce":
                    pass = txtfile.ReplaceOccurances(toReplace, replaceWith, numberOfTimes: 1);
                    break;
                case "1": case "replaceall":
                    pass = txtfile.ReplaceOccurances(toReplace, replaceWith);
                    break;
                case "2": case "replaceline":
                    string[] stringLines = toReplace.Split(';');
                    int[] lines = Array.ConvertAll(stringLines, x => int.Parse(x));
                    pass = txtfile.ReplaceLine(lines, replaceWith);
                    break;
                default:
                    Logger.Warn($"{comments} is not a valid option");
                    this.TestStepStatus.Actual = $"{comments} is not a valid option";
                    break;
            }

            this.TestStepStatus.RunSuccessful = pass;
            this.TestStepStatus.Actual = pass ? "File successfully modified." : "File was not successfully modified.";
        }
    }
}
