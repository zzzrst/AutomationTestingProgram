// <copyright file="AutoUpdaterWrapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutoUpdater
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Konsole;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using AutomationTestingProgram;

    /// <summary>
    /// Adapted From https://starbeamrainbowlabs.com/blog/article.php?article=posts/156-Autoupdate-CSharp.html.
    /// </summary>
    public class AutoUpdater
    {
        private static WebClient client;
        private static ProgressBar progress = new ProgressBar(100);
        private static Dictionary<string, string> fileToHash = new Dictionary<string, string>();

        /// <summary>
        /// Runs then re runs PrefXML.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            DownloadUpdates();
            Logger.Info("Download compelete, Restarting....");
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                FileName = "AutomationTestingProgram.exe",
                Arguments = string.Join(" ", args),
            };

            p.StartInfo = startInfo;
            p.Start();
        }

        /// <summary>
        /// Sets up a proxy. https://stackoverflow.com/questions/1938990/c-sharp-connecting-through-proxy.
        /// </summary>
        /// <param name="address">Address to connect to.</param>
        /// <param name="port">port to connect to.</param>
        /// <param name="requestString">Destination of your request.</param>
        public static void SetProxy(string address, int port, string requestString)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            WebProxy myproxy = new WebProxy(address, port);
            myproxy.BypassProxyOnLocal = false;
            request.Proxy = myproxy;
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Checks to see if there is any update avalible.
        /// </summary>
        /// <param name="program">Name of the program to check</param>.
        /// <returns>true if there are updates.</returns>
        public static bool CheckForUpdates(string program)
        {
            Version currentReleaseVersion = new Version(FileVersionInfo.GetVersionInfo(program).ProductVersion);

            // get the release version
            Version latestReleaseVersion = new Version(GetLatestReleaseVersion("https://github.com/zzzrst/SeleniumPerfXML/releases/latest"));

            Logger.Info($"Current Version: {currentReleaseVersion}");

            if (latestReleaseVersion != currentReleaseVersion)
            {
                Logger.Info($"Program is out of date! Version {latestReleaseVersion} is avaliable.");
                return true;
            }

            return false;
        }

        private static string GetLatestReleaseVersion(string url)
        {
            WebClient wc = new WebClient();
            string result = wc.DownloadString(url);
            Regex rx = new Regex("v[0-9]*[.][0-9]*[.][0-9]*");
            return rx.Match(result).Value.Substring(1);
        }

        /// <summary>
        /// Downloads the update
        /// </summary>
        /// <returns>true if successful.</returns>
        public static bool DownloadUpdates()
        {
            Logger.Info("Downloading updates");
            // string pathToChrome = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "chromedriver.exe");
            string chromiumVersion = GetDependancyVersion("Selenium.WebDriver.ChromeDriver");

            try
            {
                GetKeyPairHash();
                DownloadAndUnzip("Release.zip");

                // check if chromium has been updated, if it is, update chrome
                string latestChromeVersion = GetDependancyVersion("Selenium.WebDriver.ChromeDriver");

                Logger.Info($"Current ChromeDriver Version: {chromiumVersion}");

                if (chromiumVersion != latestChromeVersion)
                {
                    Logger.Info($"ChromeDriver is out of date! Version {latestChromeVersion} is being used. Chromium will be updated.");

                    try
                    {
                        DownloadAndUnzip("chromium.zip");
                        Directory.Move(Path.Combine(Path.GetTempPath(), "chromium"), Path.Combine(TempPathToExe(), "chromium"));
                    }
                    catch(Exception e)
                    {
                        Logger.Info("Chromium may not be in the release assets.");
                        Logger.Warn(e.Message);
                    }
 }
                else
                {
                    Logger.Info("Chromedriver is up to date!");
                }

                Logger.Info("Moving Files to Source");
                CopyFiles();
                return true;
            }
            catch (Exception e)
            {
                Logger.Info("Something when wrong when trying to update.");
                Logger.Info(e.Message);
                Logger.Info(e.StackTrace);
                return false;
            }
        }

        private static string GetDependancyVersion(string dependancy)
        {
            string result = string.Empty;
            using (StreamReader r = new StreamReader("SeleniumPerfXML.deps.json"))
            {
                string json = r.ReadToEnd();
                JObject jobj = JObject.Parse(json);
                foreach (var item in jobj.Properties())
                {
                    item.Value = item.Value.ToString().Replace("v1", "v2");
                }
                result = jobj.ToString();
                result = result.Substring(result.IndexOf(dependancy));
                result = result.Substring(0, result.IndexOf(','));
                result = result.Substring(result.IndexOf(':') + 1);
                result = result.Trim(new char[] { '\\', ' ', '"' });
                r.Close();
            }

            return result;
        }

        private static void CopyFiles()
        {
            string sourcePath = TempPathToExe();
            string destinationPath = "./";

            Logger.Info("Creating all Directory");

            // Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                try
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }
                catch (Exception)
                {
                    Logger.Info($"Directory {dirPath} already exists. Skipping...");
                }
            }

            Logger.Info("Copying all Files");

            // Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
                }
                catch (Exception)
                {
                    Logger.Info($"Directory {newPath} cannot be copyied. Skipping...");
                }
            }
        }

        private static void GetKeyPairHash()
        {
            WebClient wc = new WebClient();
            string result = wc.DownloadString("https://github.com/zzzrst/SeleniumPerfXML/releases/latest/download/Contents");
            foreach (string line in result.Split("\n"))
            {
                if (line != string.Empty)
                {
                    string[] keyPair = line.Split("\t");
                    fileToHash.Add(keyPair[0], keyPair[1]);
                }
            }
        }

        /// <summary>
        /// Gets the SHA3 hash from file.
        /// Adapted from https://stackoverflow.com/a/16318156/1460422.
        /// </summary>
        /// <param name="fileName">The filename to hash.</param>
        /// <returns>The SHA3 hash from file.</returns>
        private static string GetSHA3HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            SHA384 sha3 = new SHA384CryptoServiceProvider();
            byte[] byteHash = sha3.ComputeHash(file);
            file.Close();

            StringBuilder hashString = new StringBuilder();
            for (int i = 0; i < byteHash.Length; i++)
            {
                hashString.Append(byteHash[i].ToString("x2"));
            }

            return hashString.ToString();
        }

        private static void DownloadAndUnzip(string name)
        {
            string path = Path.Combine(Path.GetTempPath(), name);
            string folderPath = Path.GetFileNameWithoutExtension(path);
            Logger.Info(folderPath);
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
            }

            DownloadFile(name);
            if (ValidateDownload(path, fileToHash[name]))
            {
                ZipFile.ExtractToDirectory($"{path}", $"{Path.GetTempPath()}", true);
                File.Delete($"{path}");
            }
            else
            {
                throw new Exception("File may have been compromised.");
            }
        }

        private static bool ValidateDownload(string downloadDestination, string expectedHash)
        {
            Logger.Info("Validating download - ");
            string downloadHash = GetSHA3HashFromFile(downloadDestination);
            if (downloadHash.Trim().ToLower() != expectedHash.Trim().ToLower())
            {
                // The downloaded file looks bad!
                // Destroy it quick before it can do any more damage!
                File.Delete(downloadDestination);

                // Tell the user about what has happened
                Logger.Info("Fail!");
                Logger.Info($"Expected {expectedHash}, but actually got {downloadHash}).");
                Logger.Info("The downloaded update may have been modified by an attacker in transit!");
                Logger.Info("Nothing has been changed, and the downloaded file deleted.");
                return false;
            }
            else
            {
                Logger.Info("ok.");
            }

            return true;
        }

        private static string TempPathToExe()
        {
            string path = Path.Combine(Path.GetTempPath(), "Release");
            string folder = Directory.GetDirectories(path)[0];
            return Path.Combine(path, folder);
        }

        private static void DownloadFile(string fileName)
        {
            string path = Path.Combine(Path.GetTempPath(), fileName);
            Logger.Info($"Downloading {fileName} to {path}");

            // If the client is already downloading something we don't start a new download
            if (client != null && client.IsBusy)
            {
                return;
            }

            // We only create a new client if we don't already have one
            if (client == null)
            {
                client = new WebClient(); // Create a new client here
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadProgressChanged += Client_DownloadProgressChanged; // Add new event handler for updating the progress bar
            }

            client.DownloadFileTaskAsync("https://github.com/zzzrst/SeleniumPerfXML/releases/latest/download/" + fileName, path).Wait();
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) // This is our new method!
        {
            Logger.Info("File has been downloaded!");
            if (client != null)
            {
                client.Dispose(); // We have to delete our client manually when we close the window or whenever you want
            }
        }

        private static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) // NEW
        {
            progress.Refresh(e.ProgressPercentage, string.Empty);
        }
    }
}