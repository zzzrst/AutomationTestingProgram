// <copyright file="ExcelStepData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using AutomationTestingProgram.AutomationFramework;
    using AutomationTestSetFramework;
    using NPOI.SS.Formula.Atp;
    using NPOI.SS.UserModel;
    using static AutomationTestingProgram.InformationObject;

    /// <summary>
    /// The interface to get the test step data.
    /// </summary>
    public class ExcelStepData : ExcelData, ITestStepData
    {
        /// <summary>
        /// The path to the keychain account.
        /// </summary>
        internal static readonly string KEYCHAINACCOUNTFILEPATH = ConfigurationManager.AppSettings["KeychainAccountsFilePath"].ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelStepData"/> class.
        /// </summary>
        /// <param name="args">The arguments to be passed in.</param>
        public ExcelStepData(string args)
           : base(args)
        {
        }

        /// <summary>
        /// Runs when getting the test step from the test case.
        /// </summary>
        /// <param name="testStepName">The name of the test Step.</param>
        /// <param name="performAction">Determins if the test step should run.</param>
        /// <returns>The Test Step to run.</returns>
        public ITestStep SetUpTestStep(string testStepName, bool performAction = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets any arguments at runtime.
        /// </summary>
        /// <param name="testStep">Test steps to get the arguments for.</param>
        public void SetArguments(TestStep testStep)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            // query to update each of the test action's values
            foreach (string key in testStep.Arguments.Keys)
            {
                arguments.Add(key, this.QuerySpecialChars(testStep.Arguments[key]) as string);
            }

            testStep.Arguments = arguments;
        }

        /// <summary>
        /// If the original string begins with special characters in ['##'], then
        /// it is replaced by the respective value in the excel file.
        /// </summary>
        /// <param name="original">Original string.</param>
        /// <returns>The respective value in the excel file, or the original string itself.</returns>
        public object QuerySpecialChars(string original)
        {
            object result = original;

            if (original.Length >= 2 && original.Substring(0, 2) == "##")
            {
                // query password from Keychain accounts spreadsheet
                string username = original.Substring(2);
                result = this.QueryKeychainAccountPassword(username);
                Logger.Info($"Query of {original} replaced password with: {result}.");
            }

            return result;
        }

        /// <summary>
        /// Queries password of a given username from the Keychain accounts spreadsheet.
        /// </summary>
        /// <param name="username">Username to find password of.</param>
        /// <returns>Password of the Keychain account.</returns>
        private string QueryKeychainAccountPassword(string username)
        {
            string password = string.Empty;

            using (FileStream fileStream = new FileStream(KEYCHAINACCOUNTFILEPATH, FileMode.Open, FileAccess.Read))
            {
                // open both XLS and XLSX
                IWorkbook excel = WorkbookFactory.Create(fileStream);
                ISheet sheet = excel.GetSheetAt(0);
                for (int col = 0; col < sheet.GetRow(0).LastCellNum; col++)
                {
                    // Find the username column
                    if (sheet.GetRow(0).GetCell(col).StringCellValue == "Email_Account")
                    {
                        for (int row = 1; row < sheet.LastRowNum; row++)
                        {
                            if (sheet.GetRow(row).GetCell(col)?.StringCellValue == username)
                            {
                                password = sheet.GetRow(row + 1).GetCell(col).StringCellValue;
                                break;
                            }
                        }
                    }
                }
            }

            return password;
        }
    }
}
