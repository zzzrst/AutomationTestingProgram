// <copyright file="VerifyTxtFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using AutomationTestingProgram.TestingData.TestDrivers;
    using TextInteractor;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// This test step is the same as testStepXml.
    /// </summary>
    public class VerifyTxtFile : TestStep
    {
        private const string Seperator = "];[";

        /// <inheritdoc/>
        public override string Name { get; set; } = "VerifyTxtFile";

        /// <inheritdoc/>
        public override void Execute()
        {
            base.Execute();

            string option = this.Arguments["comment"];
            if (option.ToLower() == "againsttextfile" || option == "0")
            {
                this.VerifyAgainstTextFile();
            }
            else if (option.ToLower() == "againststring" || option == "1")
            {
                this.VerifyAgainstString();
            }
            else if (option.ToLower() == "findString" || option == "2")
            {
                this.FindString();
            }
            else
            {
                Logger.Warn($"{option} is not an option for verify txt file");
            }
        }

        private void VerifyAgainstTextFile()
        {
            TextInteractor txtfile = new TextInteractor(this.Arguments["object"], Logger.GetLog4NetLogger());

            // value can be either file.
            string fileName = this.Arguments["value"];
            string resultFileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + DateTime.Now.ToString("Run_dd-MM-yyyy_HH-mm-ss") + ".log";
            TextInteractor expectedTxtFile = new TextInteractor(fileName, Logger.GetLog4NetLogger());
            this.TestStepStatus.RunSuccessful = expectedTxtFile.Compare(txtfile, resultFileName, ignoreWhitespace: true, caseInsensitive: true);
            expectedTxtFile.Close();

            // CR: Attach files that were compared. Attach resulting file if there were differences.
            InformationObject.TestSetData.AddAttachment(fileName);
            InformationObject.TestSetData.AddAttachment(this.Arguments["object"]);
            if (!this.TestStepStatus.RunSuccessful)
            {
                InformationObject.TestSetData.AddAttachment(resultFileName);
            }

            txtfile.Close();
        }

        private void VerifyAgainstString()
        {
            string value = this.Arguments["value"];
            TextInteractor txtfile = new TextInteractor(this.Arguments["object"], Logger.GetLog4NetLogger());
            if (value.Contains(Seperator))
            {
                int lineNumber = 0;
                string expectedString = string.Empty;

                lineNumber = int.Parse(value.Substring(0, value.IndexOf(Seperator)));
                expectedString = value.Substring(value.IndexOf(Seperator) + 3);

                if (InformationObject.TestCaseData is DatabaseStepData database)
                {
                    expectedString = database.QuerySpecialChars(GetEnvironmentVariable(EnvVar.Environment), expectedString).ToString();
                }

                this.TestStepStatus.RunSuccessful = txtfile.LineExactMatch(expectedString, lineNumber);
                this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? $"Successfully found {expectedString} on line {lineNumber}" : $"Could not find {expectedString} on line {lineNumber}";
            }
            else
            {
                this.TestStepStatus.RunSuccessful = false;
                this.TestStepStatus.Actual = "Specified to verify against string. However the value that was passed in did not fit the syntax.";
            }

            txtfile.Close();
        }

        private void FindString()
        {
            string value = this.Arguments["value"];
            TextInteractor txtfile = new TextInteractor(this.Arguments["object"], Logger.GetLog4NetLogger());
            if (value.Contains(Seperator))
            {
                string expectedString = value.Substring(0, value.IndexOf(Seperator));
                string argument = value.Substring(value.IndexOf(Seperator) + 3);
                int amountOfTimes = -1;

                if (InformationObject.TestCaseData is DatabaseStepData database)
                {
                    expectedString = database.QuerySpecialChars(GetEnvironmentVariable(EnvVar.Environment), expectedString).ToString();
                }

                if (int.TryParse(argument, out amountOfTimes))
                {
                    this.TestStepStatus.RunSuccessful = txtfile.FindAndCount(expectedString) == amountOfTimes;
                }
                else
                {
                    if (argument.ToLower() == "exist")
                    {
                        this.TestStepStatus.RunSuccessful = txtfile.Find(expectedString);
                    }
                    else if (argument.ToLower() == "dne")
                    {
                        this.TestStepStatus.RunSuccessful = !txtfile.Find(expectedString);
                    }
                    else
                    {
                        this.TestStepStatus.RunSuccessful = false;
                        this.TestStepStatus.Actual = "For the value argument, did not pass Exist, DNE or a number.";
                    }

                    if (this.TestStepStatus.Actual == string.Empty)
                    {
                        this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? $"Successfully found {expectedString}." : $"Could not find {expectedString}.";
                    }
                }
            }
            else
            {
                string expectedString = value;
                this.TestStepStatus.RunSuccessful = txtfile.Find(expectedString);
                this.TestStepStatus.Actual = this.TestStepStatus.RunSuccessful ? $"Successfully found {expectedString}" : $"Could not find {expectedString}";
            }

            txtfile.Close();
        }
    }
}
