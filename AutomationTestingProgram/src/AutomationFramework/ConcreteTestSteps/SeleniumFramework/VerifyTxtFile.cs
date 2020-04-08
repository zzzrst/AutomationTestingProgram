// <copyright file="VerifyTxtFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
/*
namespace AutomationTestingProgram.AutomationFramework
{
    using System;
    using TextInteractor;

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

            string option = this.Arguments["comments"];
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
            this.TestStatus.Pass = expectedTxtFile.Compare(txtfile, resultFileName, ignoreWhitespace: true, caseInsensitive: true);
            expectedTxtFile.Close();

            // CR: Attach excel files that were compared. Attach resulting file if there were differences.
            this.Alm.AddTestCaseAttachment(fileName);
            this.Alm.AddTestCaseAttachment(this.TestObject.Object);
            if (!this.TestStatus.Pass)
            {
                this.Alm.AddTestCaseAttachment(resultFileName);
            }

            txtfile.Close();
        }

        private void VerifyAgainstString()
        {
            TextInteractor txtfile = new TextInteractor(this.Arguments["object"], Logger.GetLog4NetLogger());
            if (this.Value.Contains(Seperator))
            {
                int lineNumber = 0;
                string expectedString = string.Empty;

                lineNumber = int.Parse(this.Value.Substring(0, this.Value.IndexOf(Seperator)));
                expectedString = this.Value.Substring(this.Value.IndexOf(Seperator) + 3);

                expectedString = this.DbDriver.QuerySpecialChars(this.Alm.AlmEnvironment, expectedString).ToString();

                this.TestStatus.Pass = txtfile.LineExactMatch(expectedString, lineNumber);
                this.TestStatus.Message = this.TestStatus.Pass ? $"Successfully found {expectedString} on line {lineNumber}" : $"Could not find {expectedString} on line {lineNumber}";
            }
            else
            {
                this.TestStatus.Pass = false;
                this.TestStatus.Message = "Specified to verify against string. However the value that was passed in did not fit the syntax.";
            }
            txtfile.Close();
        }

        private void FindString()
        {
            TextInteractor txtfile = new TextInteractor(this.Arguments["object"], Logger.GetLog4NetLogger());
            if (this.Value.Contains(Seperator))
            {
                string expectedString = string.Empty;

                expectedString = this.Value.Substring(0, this.Value.IndexOf(Seperator));
                expectedString = this.DbDriver.QuerySpecialChars(this.Alm.AlmEnvironment, expectedString).ToString();

                var argument = this.Value.Substring(this.Value.IndexOf(Seperator) + 3);
                int amountOfTimes = -1;
                if (int.TryParse(argument, out amountOfTimes))
                {
                    this.TestStatus.Pass = txtfile.FindAndCount(expectedString) == amountOfTimes;
                }
                else
                {
                    if (argument.ToLower() == "exist")
                    {
                        this.TestStatus.Pass = txtfile.Find(expectedString);
                    }
                    else if (argument.ToLower() == "dne")
                    {
                        this.TestStatus.Pass = !txtfile.Find(expectedString);
                    }
                    else
                    {
                        this.TestStatus.Pass = false;
                        this.TestStatus.Message = "For the value argument, did not pass Exist, DNE or a number.";
                    }

                    if (this.TestStatus.Message == string.Empty)
                    {
                        this.TestStatus.Message = this.TestStatus.Pass ? $"Successfully found {expectedString}." : $"Could not find {expectedString}.";
                    }
                }
            }
            else
            {
                string expectedString = this.Value;
                this.TestStatus.Pass = txtfile.Find(expectedString);
                this.TestStatus.Message = this.TestStatus.Pass ? $"Successfully found {expectedString}" : $"Could not find {expectedString}";
            }

            txtfile.Close();
        }
    }
}
*/