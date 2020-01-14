// <copyright file="SeleniumDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumUI
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;

    /// <summary>
    /// The driver for Selenium web Driver.
    /// </summary>
    public class SeleniumDriver
    {
        /// <summary>
        /// Location of the Selenium drivers on the current machine.
        /// </summary>
        private readonly string seleniumDriverLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Driver variable.
        /// </summary>
        private IWebDriver webDriver;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumDriver"/> class.
        /// </summary>
        public SeleniumDriver()
        {
        }

        /// <summary>
        /// Opens a new browser based on what browser is set.
        /// </summary>
        /// <param name="browser">What browser to use.</param>
        public void OpenBrowser(string browser)
        {
            switch (browser)
            {
                case "Chrome":

                    ChromeOptions chromeOptions = new ChromeOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };

                    chromeOptions.AddArgument("no-sandbox");
                    chromeOptions.AddArgument("--log-level=3");
                    chromeOptions.AddArgument("--silent");
                    chromeOptions.BinaryLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\chromium\\chrome.exe";

                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(this.seleniumDriverLocation);
                    service.SuppressInitialDiagnosticInformation = true;

                    this.webDriver = new ChromeDriver(this.seleniumDriverLocation, chromeOptions);

                    break;
                case "Internet Explorer":

                    InternetExplorerOptions ieOptions = new InternetExplorerOptions
                    {
                        IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                        IgnoreZoomLevel = true,
                        EnsureCleanSession = true,
                        EnableNativeEvents = true, // not sure if this is true or false
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                        RequireWindowFocus = true,
                    };
                    InternetExplorerDriverService ieService = InternetExplorerDriverService.CreateDefaultService(this.seleniumDriverLocation);
                    ieService.SuppressInitialDiagnosticInformation = true;
                    this.webDriver = new InternetExplorerDriver(ieService, ieOptions);

                    break;
                case "Chromium": // TODO: need to add

                    ChromeOptions chromiumOptions = new ChromeOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };

                    chromiumOptions.AddArgument("no-sandbox");
                    chromiumOptions.AddArgument("--log-level=3");
                    chromiumOptions.AddArgument("--silent");
                    chromiumOptions.BinaryLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\chromium\\chrome.exe";

                    ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService(this.seleniumDriverLocation);
                    chromeService.SuppressInitialDiagnosticInformation = true;

                    this.webDriver = new ChromeDriver(this.seleniumDriverLocation, chromiumOptions);


                    break;
                case "Firefox":

                    FirefoxOptions fireFoxOption = new FirefoxOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };
                    fireFoxOption.BrowserExecutableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\firefox\\firefox.exe";
                    this.webDriver = new FirefoxDriver(this.seleniumDriverLocation,fireFoxOption);

                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Closes the currently open browser and ends session.
        /// </summary>
        public void CloseBrowser()
        {
            this.webDriver.Close();
            this.webDriver.Quit();
            this.webDriver.Dispose();
        }
    }
}
