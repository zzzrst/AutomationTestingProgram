// <copyright file="FilePathResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestinProgram.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutomationTestingProgram;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Class to get environment information from a csv file
    /// </summary>
    public class CSVEnvironmentGetter
    {
        private const int ENVIRONMENT_NAME_COL = 0;
        private const int HOST_COL = 1;
        private const int PORT_COL = 2;
        private const int DB_NAME_COL = 3;
        private const int USERNAME_COL = 4;
        private const int PASSWORD_COL = 5;
        private const int URL_COL = 6;
        private const int EMAIL_NOTIFICATION_FOLDER_COL = 7;
        private const int APP_INETPUB_LOG_COL = 8;
        private const int WEB_TIER_LOG_COL = 9;
        private const int APP_TIER_LOG_COL = 10;
        private const int IS_ENCRYPTED_COL = 11;
        private const int DB_TYPE_COL = 12;

        /// <summary>
        /// The path to the keychain account.
        /// </summary>
        private static readonly string KeychainAccountsFilePath = ConfigurationManager.AppSettings["KeychainAccountsFilePath"].ToString();

        /// <summary>
        /// Tries to grab the Environment URL from the environment url csv file
        /// </summary>
        /// <returns>The provided URL for the environment given.</returns>
        public static string GetEnvironmentURL(string env)
        {
            string url = GetColumnValue(env, URL_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment url value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Tries to grab the host from the environment csv file.
        /// </summary>
        /// <returns>The provided password for the environment given.</returns>
        public static string GetHost(string env)
        {
            string url = GetColumnValue(env, HOST_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment db username value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Tries to grab the port from the environment url csv file.
        /// </summary>
        /// <returns>The provided password for the environment given.</returns>
        public static string GetPort(string env)
        {
            string url = GetColumnValue(env, PORT_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment port value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Tries to determine if encrypted from the environment list csv file.
        /// </summary>
        /// <returns>The provided password for the environment given.</returns>
        public static string GetIsEncrypted(string env)
        {
            string url = GetColumnValue(env, IS_ENCRYPTED_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment port value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Tries to grab the username from the environment url csv file.
        /// </summary>
        /// <returns>The provided password for the environment given.</returns>
        public static string GetUsername(string env)
        {
            string url = GetColumnValue(env, USERNAME_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment db username value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Tries to grab the db name from the environment url csv file.
        /// </summary>
        /// <returns>The provided password for the environment given.</returns>
        public static string GetDBName(string env)
        {
            string url = GetColumnValue(env, DB_NAME_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment db username value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Tries to grab the passowrd from the environment url csv file.
        /// </summary>
        /// <returns>The provided password for the environment given.</returns>
        public static string GetPassword(string env)
        {
            string url = GetColumnValue(env, PASSWORD_COL);

            // error checking
            if (url == string.Empty)
            {
                Logger.Error("Could not get environment db value for env " + env);
            }

            return url;
        }

        /// <summary>
        /// Queries password of a given username from the Keychain accounts spreadsheet.
        /// </summary>
        /// <param name="username">Username to find password of.</param>
        /// <returns>Password of the Keychain account.</returns>
        public static string QueryKeychainAccountPassword(string username)
        {
            string password = string.Empty;
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"]);
            string tempFile = Path.Combine(path, "temp_keychain.xls");

            // copy the keychain file to a new folder
            try
            {
                // third param is for overwrite
                File.Copy(KeychainAccountsFilePath, tempFile, true);
            }
            catch (IOException iox)
            {
                Logger.Error(iox.Message);
            }

            Console.WriteLine("Comparing for " + username);

            // read the temp file
            using (FileStream fileStream = new FileStream(tempFile, FileMode.Open, FileAccess.Read))
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
                            // added by Victor: trim whitespace from both username and password
                            if (sheet.GetRow(row).GetCell(col)?.StringCellValue.Trim().ToLower() == username.ToLower())
                            {
                                // changed from row + 1 to col + 1 to get the correct password
                                password = sheet.GetRow(row).GetCell(col + 1).StringCellValue.Trim();

                                break;
                            }
                        }
                    }
                }
            }

            // delete the temp file
            try
            {
                File.Delete(tempFile);
            }
            catch (IOException iox)
            {
                Logger.Error(iox.Message);
            }

            // if the password cannot be found, then we will throw an exception
            if (password == string.Empty)
            {
                Logger.Error("Password could not be found");
                throw new Exception("Password could not be found in Keychain file");
            }

            return password;
        }

        /// <summary>
        /// Returns the column value for the given environment name and column
        /// </summary>
        /// <returns>The provided column for the environment.</returns>
        private static string GetColumnValue(string environment, int columnIndex)
        {
            string fileName = System.Configuration.ConfigurationManager.AppSettings["ENVIRONMENTS_LIST_TABLE"];

            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + fileName;

            // Check if the provided file exists. Even if it's at the K drive, it could exist if mapped.
            if (!File.Exists(filePath))
            {
                Logger.Error($"Environments list url at path {filePath} does not ");
                return string.Empty;
            }

            // open a stream reading to read the environments list
            StreamReader reader = new StreamReader(filePath);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                List<string> values = line.Split(',').ToList();

                if (values.Count > columnIndex)
                {
                    if (values[0].Trim() == environment)
                    {
                        Logger.Info("Value at index " + values[columnIndex]);
                        return values[columnIndex];
                    }
                }
                else
                {
                    Logger.Warn("GetColumnValue() not enough values available to index");
                }
            }

            Logger.Error("Could not retrieve value in environments table for env " + environment);
            return string.Empty;
        }

    }


}
