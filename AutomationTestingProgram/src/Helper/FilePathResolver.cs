// <copyright file="FilePathResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestinProgram.Helper
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using AutomationTestingProgram;

    /// <summary>
    /// Class to resolve the path for the file paths provided. Throws an exception FileNotFoundException if file path cannot be found.
    /// </summary>
    public class FilePathResolver
    {
        // cache network path
        private static string networkPath = string.Empty;

        /// <summary>
        /// Tries to resolve the file paths based on what is provided.
        /// </summary>
        /// <param name="filePath">Relative / Absolute file path.</param>
        /// <returns>Absolute File Path and also changed network drives to include full network path.</returns>
        public static string Resolve(string filePath)
        {
            // Check if the provided file exists. Even if it's at the K drive, it could exist if mapped.
            if (File.Exists(filePath))
            {
                return filePath;
            }

            if (filePath.Contains("\""))
            {
                Logger.Info("Replacing double quotes with spaces in file path");
                // remove all quotes (if there were any provided)
                filePath = filePath.Replace("\"", "");
            }

            // Check if the file is using a network drive.
            if (filePath.StartsWith("K:"))
            {
                if (networkPath == string.Empty)
                {
                    networkPath = ConfigurationManager.AppSettings["NetworkPath"].ToString();
                }

                string newFilePath = filePath.Replace("K:\\", networkPath);
                if (File.Exists(newFilePath))
                {
                    return newFilePath;
                }

                throw new FileNotFoundException($"Could not find provided file using path '{filePath}' or '{newFilePath}'");
            }

            // here we determine if the excel is empty and whether we could
            else if (!filePath.Contains("\\") || !File.Exists(filePath))
            {
                Logger.Info("Filepath is a relative file path");

                // if the file is a relative path, then add to the location of the rel path
                // filePath = Path.GetDirectoryName(InformationObject.);
                string builtFilePath = InformationObject.OriginalTestSetDirectoryName +
                        "\\" +
                        ConfigurationManager.AppSettings["DATA_FILES_FOLDER"] +
                        "\\" +
                        filePath;
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ConfigurationManager.AppSettings["TEMPORARY_FILES_FOLDER"]);
                string builtFilePath2 = Path.Combine(path, filePath + ".xlsx");

                Logger.Info($"Built file path is: {builtFilePath}");
                Logger.Info($"Second built file path is: {builtFilePath2}");

                // if the file exists
                if (File.Exists(builtFilePath))
                {
                    return builtFilePath;
                }
                else if (File.Exists(builtFilePath2))
                {
                    return builtFilePath2;
                }

                throw new FileNotFoundException($"Could not find provided file using path '{filePath}' or '{builtFilePath}' or '{builtFilePath2}'");
            }
            else
            {
                Logger.Info("File not resolved");
            }

            throw new FileNotFoundException($"Could not find provided file using path '{filePath}'");
        }
    }
}
