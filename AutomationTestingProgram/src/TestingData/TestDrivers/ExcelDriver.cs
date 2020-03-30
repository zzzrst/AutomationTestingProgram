// <copyright file="ExcelDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestingData.TestDrivers
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Driver class to interact with and query from Excel files.
    /// </summary>
    public class ExcelDriver
    {
        /*
        /// <summary>
        /// File path to the Keychain accounts spreadsheet.
        /// </summary>
        internal static readonly string KeychainAccountsFilePath = ConfigurationManager.AppSettings["KeychainAccountsFilePath"].ToString();
        private static string tempFilePath = string.Empty;

        /// <summary>
        /// Gets or sets connection to the Excel file.
        /// </summary>
        private static OleDbConnection XLSConnection { get; set; }

        /// <summary>
        /// Queries data from the given Excel file.
        /// </summary>
        /// <param name="filePath">File path to the Excel file.</param>
        /// <param name="query">SQL query to execute.</param>
        /// <returns><code>DataRowCollection</code>, result of the query.</returns>
        public static DataRowCollection QueryExcelFile(string filePath, string query)
        {
            ConnectExcelFile(filePath);
            StringBuilder stbQuery = new StringBuilder();
            stbQuery.Append(query);
            OleDbDataAdapter adp = new OleDbDataAdapter(stbQuery.ToString(), XLSConnection);
            DataTable dataRowsXls = new DataTable();
            adp.Fill(dataRowsXls);
            DisconnectExcelFile();
            return dataRowsXls.Rows;
        }

        /// <summary>
        /// Queries password of a given username from the Keychain accounts spreadsheet.
        /// </summary>
        /// <param name="username">Username to find password of.</param>
        /// <returns>Password of the Keychain account.</returns>
        public static string QueryKeychainAccountPassword(string username)
        {
            string query = $"SELECT Password FROM [UserAccounts$] WHERE EMAIL_ACCOUNT = '{username}'";
            return QueryExcelFile(KeychainAccountsFilePath, query)[0][0].ToString();
        }

        /// <summary>
        /// Compares two excel files and returns whether or not they are the same in content.
        /// </summary>
        /// <param name="filePath1">File path of first Excel file.</param>
        /// <param name="filePath2">File path of second Excel file.</param>
        /// <returns>Whether or not both Excel files match in content.</returns>
        public static bool CompareExcelFiles(string filePath1, string filePath2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects to the Excel file at the given file path.
        /// </summary>
        /// <param name="filePath">File path to the Excel file.</param>
        private static void ConnectExcelFile(string filePath)
        {
            tempFilePath = Path.GetTempFileName().Replace(".tmp", ".xls");
            File.Copy(KeychainAccountsFilePath, tempFilePath);
            XLSConnection = new OleDbConnection($"provider=Microsoft.Jet.OLEDB.4.0;data source={tempFilePath};Extended Properties=Excel 8.0");
        }

        private static void DisconnectExcelFile()
        {
            try
            {
                XLSConnection.Close();
                XLSConnection.Dispose();
                File.Delete(tempFilePath);
            }
            catch
            {
            }
        }*/
    }
}