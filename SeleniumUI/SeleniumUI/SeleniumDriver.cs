// <copyright file="SeleniumDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumUI
{
    using System;
    using System.Threading;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using System.ComponentModel;

    /// <summary>
    /// The driver for Selenium web Driver.
    /// </summary>
    public class SeleniumDriver : INotifyPropertyChanged
    {
        private bool isValid;

        /// <summary>
        /// Location of the Selenium drivers on the current machine.
        /// </summary>
        private readonly string seleniumDriverLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumDriver"/> class.
        /// </summary>
        public SeleniumDriver()
        {
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets driver variable.
        /// </summary>
        public IWebDriver WebDriver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the model is in a valid state or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.isValid;
            }

            set
            {
                if (this.isValid != value)
                {
                    this.isValid = value;
                    // this.OnPropertyChange("IsValid");
                }
            }
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

                    this.WebDriver = new ChromeDriver(this.seleniumDriverLocation, chromeOptions);

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
                    this.WebDriver = new InternetExplorerDriver(ieService, ieOptions);

                    break;
                case "Chromium":

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

                    this.WebDriver = new ChromeDriver(this.seleniumDriverLocation, chromiumOptions);

                    break;
                case "Firefox":

                    FirefoxOptions fireFoxOption = new FirefoxOptions
                    {
                        UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                    };
                    fireFoxOption.BrowserExecutableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\firefox\\firefox_.exe";
                    this.WebDriver = new FirefoxDriver(this.seleniumDriverLocation, fireFoxOption);

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
            this.WebDriver.Close();
            this.WebDriver.Quit();
            this.WebDriver.Dispose();
        }

        /// <summary>
        /// Sets the value of isValid.
        /// </summary>
        private void SetIsValid()
        {
            throw new NotImplementedException();
        }
    }
}
