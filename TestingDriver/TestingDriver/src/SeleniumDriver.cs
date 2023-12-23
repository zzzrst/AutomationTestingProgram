// <copyright file="SeleniumDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace TestingDriver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Management;
    using System.Reflection;
    using AxeAccessibilityDriver;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Support.Extensions;
    using OpenQA.Selenium.Support.UI;
    using static TestingDriver.ITestingDriver;

    /// <summary>
    /// Driver class for Selenium WebDriver.
    /// </summary>
    public class SeleniumDriver : ITestingDriver
    {
        /// <summary>
        /// Location of the Selenium drivers on the current machine.
        /// </summary>
        private readonly string seleniumDriverLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private IAccessibilityChecker axeDriver = null;
        private WebDriverWait wdWait;
        private Actions Action;

        private int PID = -1;

        private string environment;
        private string url;

        private string screenshotSaveLocation;
        private Browser browserType;
        private TimeSpan timeOutThreshold;
        private TimeSpan actualTimeOut;

        private bool incogMode;
        private bool headless;

        private string browserSize;

        private string remoteHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumDriver"/> class.
        /// </summary>
        /// <param name="browser">The browser to use.</param>
        /// <param name="timeOut">The time out in seconds.</param>
        /// <param name="environment">The environment of the test.</param>
        /// <param name="url">Default url to naivgate to.</param>
        /// <param name="screenshotSaveLocation">Location to save screenshots.</param>
        /// <param name="actualTimeout">Time out limit in minutes.</param>
        /// <param name="loadingSpinner">The xpath for any loading spinners.</param>
        /// <param name="errorContainer">The xpath for any error containers.</param>
        /// <param name="remoteHost">The address of the remote host.</param>
        /// <param name="headless">Indicate whether to run the browser in headless mode.</param>
        /// <param name="incogMode">Indicate whether to run the browser in incognito mode.</param>
        /// <param name="webDriver">Any Web driver to be passed in.</param>
        /// <param name="browserSize">The execution type which indicates how the test will be executed in.</param>
        /// <param name="localTimeout">The timeout indicating how long to wait to find individual elements.</param>
        public SeleniumDriver(
            string browser = "chrome",
            int timeOut = 5,
            string environment = "",
            string url = "",
            string screenshotSaveLocation = "./",
            int actualTimeout = 60,
            string loadingSpinner = "",
            string errorContainer = "",
            string remoteHost = "",
            bool headless = true,
            bool incogMode = true,
            IWebDriver webDriver = null,
            string browserSize = "max",
            int localTimeout = 30)
        {
            this.browserType = this.GetBrowserType(browser);
            this.timeOutThreshold = TimeSpan.FromSeconds(timeOut);
            this.environment = environment;
            this.url = url;
            this.screenshotSaveLocation = screenshotSaveLocation;
            this.actualTimeOut = TimeSpan.FromMinutes(actualTimeout);

            this.incogMode = incogMode;
            this.headless = headless;
            this.browserSize = browserSize;

            this.LocalTimeout = localTimeout;

            if (string.IsNullOrEmpty(loadingSpinner))
            {
                this.LoadingSpinner = "loadingspinner";
            }
            else
            {
                this.LoadingSpinner = loadingSpinner; // added loading spinner default if null or empty
            }

            this.ErrorContainer = errorContainer;
            this.remoteHost = remoteHost;
            this.WebDriver = webDriver;
        }

        /// <inheritdoc/>
        public TestingDriverType Name { get; } = TestingDriverType.Selenium;

        /// <inheritdoc/>
        public string CurrentURL { get => this.WebDriver.Url; }

        /// <inheritdoc/>
        public string LoadingSpinner { get; set; }

        /// <inheritdoc/>
        public string ErrorContainer { get; set; }

        /// <inheritdoc/>
        public int LocalTimeout { get; set; }

        /// <summary>
        /// Gets the web driver in use.
        /// </summary>
        public IWebDriver WebDriver { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the web driver is in an IFrame or not.
        /// </summary>
        private bool InIFrame { get; set; } = false;

        private string IFrameXPath { get; set; } = string.Empty;

        private int CurrentWindow { get; set; } = -1;

        /// <summary>
        /// Returns the webElement at the given xPath.
        /// </summary>
        /// <param name="xPath">The xpath to find the element at.</param>
        /// <param name="jsCommand">any Js Command To use.</param>
        /// <returns>The web element.</returns>
        public IWebElement GetWebElement(string xPath, string jsCommand = "")
        {
            return this.FindElement(xPath, jsCommand);
        }

        /// <inheritdoc/>
        public void AcceptAlert()
        {
            this.WebDriver.SwitchTo().Alert().Accept();
            this.SetActiveTab();
        }

        /// <inheritdoc/>
        public void Back()
        {
            this.WebDriver.Navigate().Back();
        }

        /// <inheritdoc/>
        public bool CheckForElementState(string xPath, ElementState state, string jsCommand = "")
        {
            // wait for the page to be ready before clicking
            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            IWebElement element = null;

            try
            {
                element = this.FindElement(xPath, jsCommand, 3);
            }
            catch (NoSuchElementException)
            {
                // this is expected if we are checking that it is not visible.
            }
            catch (WebDriverTimeoutException)
            {
                // this is expected if we are checking that it is not visible.
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }

            bool isReadOnly;

            switch (state)
            {
                case ElementState.Invisible:
                    return element == null || !element.Displayed;

                case ElementState.Visible:
                    return element != null && element.Displayed;

                case ElementState.Clickable:
                    if (element != null)
                    {
                        isReadOnly = bool.Parse(element.GetAttribute("readonly") ?? "false");
                        return element.Displayed && element.Enabled && !isReadOnly;
                    }
                    else
                    {
                        return false;
                    }

                case ElementState.Disabled:
                    if (element != null)
                    {
                        isReadOnly = bool.Parse(element.GetAttribute("readonly") ?? "false");
                        return element.Displayed && (!element.Enabled || isReadOnly);
                    }
                    else
                    {
                        return false;
                    }

                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets the check box's value to ON.
        /// </summary>
        public void Check(string xPath, bool byJS = false, string jsCommand = "")
        {
            // wait for the page to be ready before clicking
            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            IWebElement element = this.FindElement(xPath, jsCommand);
            this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));

            // set checkbox to 'checked' if element is not yet selected
            if (!element.Selected)
            {
                element.Click();
            }

            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <inheritdoc/>
        /// <summary>
        /// Sets the check box's value to OFF.
        /// </summary>
        public void Uncheck(string xPath, bool byJS = false, string jsCommand = "")
        {
            // wait for the page to be ready before clicking
            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            IWebElement element = this.FindElement(xPath, jsCommand);
            this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));

            // set checkbox to 'unchecked' if element is already selected
            if (element.Selected)
            {
                element.Click();
            }

            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <inheritdoc/>
        public void ClickElement(string xPath, bool byJS = false, string jsCommand = "")
        {
            // we added this to wait for the page to load
            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            IWebElement element = this.FindElement(xPath, jsCommand);
            this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));

            if (byJS)
            {
                element.Click(); // changed by Victor for JS after comparing with SeleniumFramework
                // IJavaScriptExecutor executor = (IJavaScriptExecutor)this.WebDriver;
                // executor.ExecuteScript("var element=arguments[0]; setTimeout(function() {element.click();}, 100)", element);
            }
            else
            {
                element.Click();
            }
            this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        /// <inheritdoc/>
        public void CloseBrowser(bool closeAll)
        {
            // if we are closing all browsers, then run webdriver quit
            if (closeAll)
            {
                this.WebDriver.Quit();
            }
            // if we are closing only the current context browser, then close it and switch to the default context
            else
            {
                this.WebDriver.Close();

                // after closing the browser, switch to the default content
                this.SetActiveTab();
            }
        }

        /// <inheritdoc/>
        public void DismissAlert()
        {
            this.WebDriver.SwitchTo().Alert().Dismiss();
            this.SetActiveTab();
        }

        /// <inheritdoc/>
        public void Forward()
        {
            this.WebDriver.Navigate().Forward();
        }

        /// <inheritdoc/>
        public string GetElementAttribute(string attribute, string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            return element.GetAttribute(attribute);
        }

        /// <inheritdoc/>
        public string GetElementText(string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            return element.Text;
        }

        /// <inheritdoc/>
        public string GetAlertText()
        {
            return this.WebDriver.SwitchTo().Alert().Text;
        }

        /// <inheritdoc/>
        public void Quit()
        {
            try
            {
                if (this.WebDriver != null)
                {
                    // first close
                    //this.WebDriver.Close();

                    // then quit
                    this.WebDriver.Quit();

                    this.WebDriver = null;

                    Logger.Info("Successfully quitted webdriver");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to quit driver " + ex);
            }
            finally
            {
                this.ForceKillWebDriver();
            }
        }

        /// <inheritdoc/>
        public void Maximize()
        {
            // we want to determine what the size should be before maximizing.
            Console.WriteLine("Maximizing browser");

            // maximize the browser first, then do any changes
            this.WebDriver.Manage().Window.Maximize();

            // grab the maximized brwoser size
            Size windowSize = this.WebDriver.Manage().Window.Size;

            // browser Width and browser Height
            switch (this.browserSize)
            {
                case "mobile":
                    windowSize.Width = windowSize.Width / 3;
                    break;
                case "tablet":
                    windowSize.Width = windowSize.Width / 2;
                    break;
                case "desktop":
                    windowSize.Width = 1024;
                    windowSize.Height = 768;
                    break;
                case "extended-desktop":
                    break;
                case "max":
                    // maximize to the max size of the window, which should already be done
                    break;
                default:
                    Logger.Warn("Not implemented error for size");
                    break;
            }

            Console.WriteLine($"Size after maximization: {windowSize.Width} {windowSize.Height}");
            this.WebDriver.Manage().Window.Size = windowSize;
        }

        /// <inheritdoc/>
        public void ForceKillWebDriver()
        {
            try
            {
                if (this.PID != -1)
                {
                    var driverProcessIds = new List<int> { this.PID };

                    var mos = new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={this.PID}");


                    foreach (var mo in mos.Get())
                    {
                        var pid = Convert.ToInt32(mo["ProcessID"]);
                        driverProcessIds.Add(pid);
                    }

                    // Kill all
                    foreach (var id in driverProcessIds)
                    {
                        Process.GetProcessById(id).Kill();
                        Console.WriteLine($"We just tried killing process {id}");
                    }

                    this.PID = -1;
                }
                Logger.Info("Successfully force killed items");
            }
            catch (Exception e)
            {
                Logger.Info("Failed to force kill webdriver");
                Logger.Warn("Error message: " + e);
            }
            finally
            {
                this.PID = -1;
            }
        }

        /// <inheritdoc/>
        public void GenerateAODAResults(string folderLocation)
        {
            try
            {
                this.axeDriver.LogResults(folderLocation);
            }
            catch (Exception ex)
            {
                Logger.Error("Could not generate AODA results " + ex);
            }
        }

        /// <inheritdoc/>
        public List<string> GetAllLinksURL()
        {
            this.WaitForLoadingSpinner();
            var allElements = this.WebDriver.FindElements(By.TagName("a"));
            List<string> result = new List<string>();
            foreach (IWebElement link in allElements)
            {
                string url = link.GetAttribute("href");
                if (!url.Contains("javascript"))
                {
                    result.Add(url);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool NavigateToURL(string url = "", bool instantiateNewDriver = true)
        {
            try
            {
                if (url == string.Empty)
                {
                    url = this.url;
                }

                if (instantiateNewDriver)
                {
                    this.InstantiateSeleniumDriver();
                }

                this.WebDriver.Url = url;
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Something went wrong while navigating to url: {e.ToString()}");
                return false;
            }
        }

        /// <inheritdoc/>
        public bool LaunchNewTab(string url = "", bool instantiateNewDriver = false)
        {
            // launch new tab has not been tested extensively.
            Logger.Info("Only works for non-incognito mode for launching tabs. ");
            try
            {
                if (url == string.Empty)
                {
                    url = this.url;
                }

                this.WebDriver.SwitchTo().NewWindow(WindowType.Tab);
                this.WebDriver.Url = url;
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"Something went wrong while launching tab to url: {e.ToString()}");
                return false;
            }
        }

        /// <inheritdoc/>
        public void PopulateElement(string xPath, string value, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(element));
            element.Click();
            element.Clear();
            element.SendKeys(value);
        }

        /// <inheritdoc/>
        public void RefreshWebPage()
        {
            this.WebDriver.Navigate().Refresh();
        }

        /// <inheritdoc/>
        public void RunAODA(string providedPageTitle)
        {
            // add if this.axedriver is null (especially for AAD authentication)
            if (this.Action == null || this.axeDriver == null)
            {
                // skipped validating Axe Driver since a non-action
                Logger.Info("Skipped verifying Axe Driver");
                return;
            }
            try
            {
                this.WebDriver.SwitchTo().DefaultContent();
                this.axeDriver.CaptureResult(providedPageTitle);
                if (this.InIFrame)
                {
                    this.SwitchToIFrame(this.IFrameXPath, string.Empty);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Axe Driver failed to capture results. Stack trace: {e}");
            }
        }

        /// <inheritdoc/>
        public void SendKeys(string keystroke)
        {
            // appears to only be stable for F5 and reloading
            Logger.Info("Send keys is currently only stable for F5 and reloading");

            Actions action = new Actions(this.WebDriver);
            if (keystroke == "{ENTER}")
            {
                //action.SendKeys(Keys.Enter);
                action.SendKeys(OpenQA.Selenium.Keys.Enter);
            }
            else if (keystroke == "{TAB}")
            {
                action.SendKeys(OpenQA.Selenium.Keys.Tab);
            }
            else
            {
                // replace white space
                //keystroke = keystroke.Replace("}", "");
                //keystroke = keystroke.Replace("{", "");
                keystroke = keystroke.Replace(" ", "");

                if (keystroke == "{CTRL+F5}" || keystroke == "{F5+CTRL}" || keystroke == "{F5}")
                {
                    // note that we are refreshing page
                    Logger.Info("Refreshing page using WebDriver.Navigate.Refresh");
                    WebDriver.Navigate().Refresh();
                }
                //else if(keystroke == "F5")
                //{
                //    action.SendKeys(Keys.F5);
                //}
                else
                {
                    action.SendKeys(keystroke);
                }
                //action.SendKeys("F5");
            }
        }

        /// <inheritdoc/>
        public void SelectValueInElement(string xPath, string value, string jsCommand)
        {
            // remove extra spaces at the start and end of the string and new line characters
            value = value.Trim();

            IWebElement ddlElement = this.FindElement(xPath, jsCommand);

            SelectElement ddl;
            try
            {
                // we added this to wait for the page to load
                this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                ddl = new SelectElement(ddlElement);
            }
            catch
            {
                Logger.Warn("Select was not a select object, will now be clicking instead");
                this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(ddlElement));
                ddlElement.Click();
                return;
            }

            Logger.Info("Object was a select object, will now select");
            // select by text if there is no problem selecting object
            this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(ddlElement));
            ddl.SelectByText(value);
        }

        /// <inheritdoc/>
        public void SetTimeOutThreshold(string seconds)
        {
            this.wdWait = new WebDriverWait(this.WebDriver, TimeSpan.FromSeconds(Convert.ToDouble(seconds)));
        }

        /// <inheritdoc/>
        public void SwitchToIFrame(string xPath, string jsCommand)
        {
            this.SetActiveTab();
            this.WebDriver.SwitchTo().DefaultContent();

            if (xPath == "root")
            {
                this.InIFrame = false;
            }
            else
            {
                this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(xPath)));
                this.InIFrame = true;
                this.IFrameXPath = xPath;
            }
        }

        /// <inheritdoc/>
        public void SwitchToTab(int tab)
        {
            var tabs = this.WebDriver.WindowHandles;
            this.WebDriver.SwitchTo().Window(tabs[tab]);
        }

        /// <inheritdoc/>
        public void TakeScreenShot(string fileName)
        {
            try
            {
                Screenshot screenshot = this.WebDriver.TakeScreenshot();
                screenshot.SaveAsFile(fileName); // convert to jpeg
            }
            catch
            {
            }
        }

        /// <inheritdoc/>
        /// Returns true only if screenshot was successfully created
        public bool TakeEntireScreenshot(string fileName, bool isMobile = false)
        {
            var pixelRatio = 1;

            if (isMobile)
            {
                pixelRatio = (int)(long)((IJavaScriptExecutor)this.WebDriver).ExecuteScript("return window.devicePixelRatio");
            }

            // Size of page
            var totalWidth = Convert.ToInt32((int)(long)((IJavaScriptExecutor)this.WebDriver).ExecuteScript("return document.body.offsetWidth") * pixelRatio);
            var totalHeight = Convert.ToInt32((int)(long)((IJavaScriptExecutor)this.WebDriver).ExecuteScript("return  document.body.parentNode.scrollHeight") * pixelRatio);

            // Size of the viewport
            var viewportWidth = Convert.ToInt32((int)(long)((IJavaScriptExecutor)this.WebDriver).ExecuteScript("return document.body.clientWidth") * pixelRatio);
            var viewportHeight = Convert.ToInt32((int)(long)((IJavaScriptExecutor)this.WebDriver).ExecuteScript("return window.innerHeight") * pixelRatio);


            var screenshot = (ITakesScreenshot)this.WebDriver;
            ((IJavaScriptExecutor)this.WebDriver).ExecuteScript("window.scrollTo(0, 0)");

            if (totalWidth <= viewportWidth && totalHeight <= viewportHeight)
            {
                // take a screenshot without having to work with bitmap
                screenshot.GetScreenshot().SaveAsFile(fileName); // convert to jpeg
                return true;
            }

            var rectangles = new List<Rectangle>();
            // Loop until the totalHeight is reached
            for (var y = 0; y < totalHeight; y += viewportHeight)
            {
                var newHeight = viewportHeight;

                // Fix if the height of the element is too big
                if (y + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - y;
                }

                // Loop until the totalWidth is reached
                for (var x = 0; x < totalWidth; x += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    // Fix if the Width of the Element is too big
                    if (x + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - x;
                    }

                    // Create and add the Rectangle
                    rectangles.Add(new Rectangle(x, y, newWidth, newHeight));
                }
            }

            Bitmap stitchedImage;
            try
            {
                stitchedImage = new Bitmap(totalWidth, totalHeight, PixelFormat.Format16bppRgb555);
            }
            catch
            {
                Logger.Error($"Creating a bitmap of width {totalWidth} and height {totalHeight} failed");
                // var stichedImage = new Bitmap(totalWidth, totalHeight, PixelFormat.)
                return false;
            }

            var previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    ((IJavaScriptExecutor)this.WebDriver).ExecuteScript($"window.scrollBy({(rectangle.Right - previous.Right) / pixelRatio}, {(rectangle.Bottom - previous.Bottom) / pixelRatio})");
                }

                // Copy the Image
                using (var graphics = Graphics.FromImage(stitchedImage))
                {
                    screenshot.GetScreenshot().SaveAsFile(fileName); // save the screenshot at the specified file name
                    Image newImage = Image.FromFile(fileName); // get the same screeenshot as an image

                    int newImgWidthCut = 0;
                    int newImgHeightCut = 0;

                    // calculate percentage to cut from the newImage
                    if (viewportHeight != rectangle.Height)
                    {
                        newImgHeightCut = newImage.Height - (rectangle.Height * (newImage.Height / viewportHeight));
                    }
                    if (viewportWidth != rectangle.Width)
                    {
                        newImgWidthCut = newImage.Width - (rectangle.Width * (newImage.Width / viewportWidth));
                    }

                    graphics.DrawImage(newImage, rectangle, newImgWidthCut, newImgHeightCut, newImage.Width, newImage.Height, GraphicsUnit.Pixel);
                    newImage.Dispose(); // delete the image
                }

                previous = rectangle;
            }
            // save stiched image
            stitchedImage.Save(fileName); // save the screenshot at the specified file name
            stitchedImage.Dispose();

            return true;
        }

        /// <inheritdoc/>
        public void WaitForElementState(string xPath, ElementState state, string jsCommand = "")
        {
            switch (state)
            {
                case ElementState.Invisible:

                    this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(By.XPath(xPath)));
                    break;

                case ElementState.Visible:
                    this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath(xPath)));
                    break;

                case ElementState.Clickable:
                    this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(xPath)));
                    break;
            }
        }

        /// <inheritdoc/>
        public void WaitForLoadingSpinner()
        {
            try
            {
                this.SetActiveTab();

                if (this.LoadingSpinner != string.Empty)
                {
                    // wait by ID like SeleniumFrammework.
                    this.wdWait.Until(
                        SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(
                            By.Id(this.LoadingSpinner)));

                    // wait for the page to be ready before continuing
                    this.wdWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
            }
            catch (Exception)
            {
                // we want to do nothing here
            }
        }

        /// <inheritdoc/>
        public void Wait(int seconds)
        {
            this.WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
        }

        /// <inheritdoc/>
        public bool VerifyAttribute(string attribute, string expectedValue, string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            attribute = attribute.ToLower();
            return element.GetAttribute(attribute) == expectedValue;
        }

        /// <inheritdoc/>
        public bool VerifyElementText(string expected, string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand, -1);
            // if the element cannot be found, return false
            if (element == null)
            {
                return false;
            }

            bool result = expected == element.Text;
            return result;
        }

        /// <inheritdoc/>
        public bool VerifyFieldValue(string expected, string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            // if the element cannot be found, return false
            if (element == null)
            {
                return false;
            }
            // TODO: analyse verify field value forerrors
            if (element.GetAttribute("value") == null)
            {
                Logger.Info("Returning early since value attrbute is null in VerifyFieldValue");
                return false;
            }
            else
            {
                // return whether the string returned is 
                return expected == element.GetAttribute("value");
            }
        }

        /// <inheritdoc/>
        public bool VerifyElementSelected(string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            return element.Selected;
        }

        /// <inheritdoc/>
        public bool VerifyDropDownContent(List<string> expected, string xPath, string jsCommand = "")
        {
            IWebElement element = this.FindElement(xPath, jsCommand);
            SelectElement selectElement = new SelectElement(element);

            List<string> actualValue = new List<string>();
            foreach (IWebElement e in selectElement.Options)
            {
                actualValue.Add(e.Text);
            }

            return expected.All(e => actualValue.Contains(e));
        }

        /// <inheritdoc/>
        public void CheckErrorContainer()
        {
            if (this.ErrorContainer != string.Empty)
            {
                try
                {
                    IWebElement errorContainer = this.WebDriver.FindElement(By.XPath(this.ErrorContainer));
                    Logger.Error($"Found the following in the error container: {errorContainer.Text}");
                }
                catch (Exception)
                {
                    // we do nothing if we don't find it.
                }
            }
        }

        /// <inheritdoc/>
        public void ExecuteJS(string jsCommand)
        {
            ((IJavaScriptExecutor)this.WebDriver).ExecuteScript(jsCommand);
        }

        /// <summary>
        /// Finds the first IWebElement By XPath.
        /// </summary>
        /// <param name="xPath">The xpath to find the web element.</param>
        /// <returns> The first IWebElement whose xpath matches. </returns>
        private IWebElement GetElementByXPath(string xPath)
        {
            this.WaitForLoadingSpinner();
            return this.wdWait.Until(driver => driver.FindElement(By.XPath(xPath)));
        }

        /// <summary>
        /// Finds the first IWebElement By XPath.
        /// </summary>
        /// <param name="xPath">The xpath to find the web element.</param>
        /// <param name="tries"> The amount in seconds to wait for.</param>
        /// <returns> The first IWebElement whose xpath matches. </returns>
        private IWebElement GetElementByXPath(string xPath, int tries)
        {
            this.WaitForLoadingSpinner();
            IWebElement element = null;
            for (int i = 0; i < tries; i++)
            {
                try
                {
                    element = this.WebDriver.FindElement(By.XPath(xPath));
                    break;
                }
                catch
                {
                }
            }

            return element;
        }

        /// <summary>
        /// Executes JS command on this element.
        /// </summary>
        /// <param name="jsCommand">command.</param>
        /// <param name="webElement">Elemnt to interact with.</param>
        private void ExecuteJS(string jsCommand, IWebElement webElement)
        {
            ((IJavaScriptExecutor)this.WebDriver).ExecuteScript(jsCommand, webElement);
        }

        /// <summary>
        /// Moves the mouse to the given element.
        /// </summary>
        /// <param name="element">Web element to mouse over.</param>
        private void MouseOver(IWebElement element)
        {
            this.Action.MoveToElement(element).Build().Perform();
        }

        /// <summary>
        /// The FindElementByJs.
        /// </summary>
        /// <param name="jsCommand">The jsCommand<see cref="string"/>.</param>
        /// <param name="webElements">The webElements<see cref="List{IWebElement}"/>.</param>
        /// <returns>The <see cref="IWebElement"/>.</returns>
        private IWebElement FindElementByJs(string jsCommand, List<IWebElement> webElements)
        {
            this.SetActiveTab();
            var element = ((IJavaScriptExecutor)this.WebDriver).ExecuteScript(jsCommand, webElements);
            return (IWebElement)element;
        }

        private Browser GetBrowserType(string browserName)
        {
            Browser browser;
            if (browserName.ToLower().Contains("chrome"))
            {
                if (browserName.ToLower().Contains("remote"))
                {
                    browser = Browser.RemoteChrome;
                }
                else
                {
                    browser = Browser.Chrome;
                }
            }
            else if (browserName.ToLower().Contains("ie"))
            {
                browser = Browser.IE;
            }
            else if (browserName.ToLower().Contains("firefox"))
            {
                browser = Browser.Firefox;
            }
            else if (browserName.ToLower().Contains("edge"))
            {
                browser = Browser.Edge;
            }
            else
            {
                Logger.Error($"Sorry we do not currently support the browser: {browserName}");
                throw new Exception("Unsupported Browser.");
            }

            return browser;
        }

        /// <summary>
        /// Finds the web element of the corresponding test object under the given timeout duration and trys.
        /// </summary>
        /// <param name="xPath">The xPath of the element.</param>.
        /// <param name="jsCommand">Optional. Any java script commands to use.</param>
        /// <param name="trys">Optional. Number of trys before giving up.</param>
        /// <returns>The web element of the corresponding test object.</returns>
        private IWebElement FindElement(string xPath, string jsCommand = "", int trys = -1)
        {
            IWebElement webElement = null;
            //double timeout = this.timeOutThreshold.TotalSeconds;
            bool errorThrown = false;

            // wait for browser to finish loading before finding the object
            this.WaitForLoadingSpinner();

            // wait for timeout or until object is found
            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();
            var start = stopWatch.Elapsed.TotalSeconds;

            // set the timeout to the local timeout defined
            double timeout = this.LocalTimeout;

            // print out the local timeout
            // Console.WriteLine("Local timeout is set to: " + timeout);

            while ((stopWatch.Elapsed.TotalSeconds - start) < timeout && webElement == null && trys != 0)
            {
                try
                {
                    List<IWebElement> webElements = this.WebDriver.FindElements(By.XPath(xPath)).ToList();
                    if (jsCommand != string.Empty)
                    {
                        webElement = (IWebElement)((IJavaScriptExecutor)this.WebDriver).ExecuteScript(jsCommand, webElements);
                    }
                    else
                    {
                        if (webElements.Count > 0)
                        {
                            // explicit wait until first element is displayed
                            WebDriverWait wait = new WebDriverWait(this.WebDriver, TimeSpan.FromSeconds(5));
                            wait.Until(d => webElements[0].Displayed);

                            webElement = webElements[0];
                        }
                    }
                }
                catch (StaleElementReferenceException)
                {
                    // do nothing, since we didn't find the element.
                }
                catch (Exception e)
                {
                    if (!errorThrown)
                    {
                        errorThrown = true;
                        Logger.Error(e.ToString());
                    }
                }

                if (trys > 0)
                {
                    trys--;
                }
            }

            stopWatch.Stop();
            return webElement;
        }

        private void InstantiateSeleniumDriver()
        {
            try
            {
                this.Quit();

                this.WebDriver = null;
                //this.CurrentURL = "https://www.google.com/";

                ChromeOptions chromeOptions;
                ChromeDriverService service;

                // create local var to determine whether to enable incog mode
                string pathToNewFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "temporary_files");

                Logger.Info("Browser type is: " + this.browserType);

                switch (this.browserType)
                {
                    // remote chrome is used for Selenium Grid
                    case Browser.RemoteChrome:

                        chromeOptions = new ChromeOptions
                        {
                            UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                        };

                        // check if in incog mode, if it is, then we launch incog mode
                        if (this.incogMode)
                        {
                            chromeOptions.AddArgument("--incognito");
                        }

                        chromeOptions.AddArgument("no-sandbox");
                        chromeOptions.AddArgument("--log-level=3");
                        chromeOptions.AddArgument("--silent");

                        this.WebDriver = new RemoteWebDriver(new Uri(this.remoteHost), chromeOptions.ToCapabilities(), this.actualTimeOut);

                        break;
                    case Browser.Chrome:

                        string chromiumFolderLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\chromium";

                        chromeOptions = new ChromeOptions
                        {
                            UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                        };

                        // check if in incog mode, if it is, then we launch incog mode
                        if (this.incogMode)
                        {
                            chromeOptions.AddArgument("--incognito");
                        }

                        // enable headless mode
                        if (this.headless)
                        {
                            Logger.Info("Started a headless session");
                            chromeOptions.AddArgument("--headless=new");
                        }

                        chromeOptions.AddArgument("--start-maximized");

                        chromeOptions.AddArgument("no-sandbox");
                        chromeOptions.AddArgument("--log-level=3");
                        chromeOptions.AddArgument("--silent");
                        chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
                        chromeOptions.AddUserProfilePreference("download.default_directory", pathToNewFolder);
                        chromeOptions.AddUserProfilePreference("disable-popup-blocking", true);
                        chromeOptions.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                        chromeOptions.BinaryLocation = $"{chromiumFolderLocation}\\chrome.exe";

                        // we want to find all the files under the location of chromium\Extensions and add them in.
                        if (Directory.Exists(chromiumFolderLocation + "\\Extensions"))
                        {
                            foreach (string extension in Directory.GetFiles(chromiumFolderLocation + "\\Extensions"))
                            {
                                chromeOptions.AddExtension(extension);
                            }
                        }

                        service = ChromeDriverService.CreateDefaultService(this.seleniumDriverLocation);
                        service.SuppressInitialDiagnosticInformation = true;

                        this.WebDriver = new ChromeDriver(this.seleniumDriverLocation, chromeOptions, this.actualTimeOut);

                        // check for maximization
                        this.Maximize(); // maximize the webdriver

                        this.PID = service.ProcessId;
                        Logger.Info($"Chrome Driver service PID is: {this.PID}");

                        break;
                    case Browser.Edge:
                        // this.webDriver = new EdgeDriver(this.seleniumDriverLocation, null, this.actualTimeOut);
                        // This is to test Micrsoft Edge (Chromium Based)
                        EdgeOptions options = new EdgeOptions
                        {
                            UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                        };

                        // check if in incog mode, if it is, then we launch incog mode
                        if (this.incogMode)
                        {
                            // options.AddArgument("--incognito");
                            // options.AddAdditionalEdgeOption("InPrivate", true);
                            options.AddArgument("InPrivate");
                        }

                        options.AddArgument("no-sandbox");
                        options.AddArgument("--log-level=3");
                        options.AddArgument("--silent");
                        options.AddUserProfilePreference("download.prompt_for_download", false);
                        options.AddUserProfilePreference("download.default_directory", pathToNewFolder);
                        options.AddUserProfilePreference("disable-popup-blocking", true);
                        options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
                        string edgeFolderLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\edge";
                        options.BinaryLocation = edgeFolderLocation + "\\msedge.exe";

                        // we want to find all the files under the location of edge\Extensions and add them in.
                        if (Directory.Exists(edgeFolderLocation + "\\Extensions"))
                        {
                            foreach (string extension in Directory.GetFiles(edgeFolderLocation + "\\Extensions"))
                            {
                                options.AddExtension(extension);
                            }
                        }

                        EdgeDriverService edgeService = EdgeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "msedgedriver.exe");

                        edgeService.SuppressInitialDiagnosticInformation = true;
                        this.WebDriver = new EdgeDriver(edgeService, options, this.actualTimeOut);
                        this.PID = edgeService.ProcessId;
                        Logger.Info($"Edge Driver service PID is: {this.PID}");

                        break;
                    case Browser.Firefox:

                        FirefoxOptions fireFoxOptions = new FirefoxOptions();

                        // Default for edge is incog mode

                        fireFoxOptions.SetPreference("browser.download.folderList", 2);
                        fireFoxOptions.SetPreference("browser.download.dir", pathToNewFolder);
                        fireFoxOptions.SetPreference("browser.download.manager.alertOnEXEOpen", false);
                        fireFoxOptions.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/msword, application/csv, application/ris, text/csv, image/png, application/pdf, text/html, text/plain, application/zip, application/x-zip, application/x-zip-compressed, application/download, application/octet-stream");
                        fireFoxOptions.SetPreference("browser.download.manager.showWhenStarting", false);
                        fireFoxOptions.SetPreference("browser.download.manager.focusWhenStarting", false);
                        fireFoxOptions.SetPreference("browser.download.useDownloadDir", true);
                        fireFoxOptions.SetPreference("browser.helperApps.alwaysAsk.force", false);
                        fireFoxOptions.SetPreference("browser.download.manager.alertOnEXEOpen", false);
                        fireFoxOptions.SetPreference("browser.download.manager.closeWhenDone", true);
                        fireFoxOptions.SetPreference("browser.download.manager.showAlertOnComplete", false);
                        fireFoxOptions.SetPreference("browser.download.manager.useWindow", false);
                        fireFoxOptions.SetPreference("services.sync.prefs.sync.browser.download.manager.showWhenStarting", false);
                        fireFoxOptions.SetPreference("pdfjs.disabled", true);

                        string firefoxFolderLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\firefox";
                        fireFoxOptions.BrowserExecutableLocation = firefoxFolderLocation + "\\firefox.exe";

                        FirefoxDriverService fireFoxService = FirefoxDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "geckodriver.exe");

                        // FirefoxDriverService fireFoxService = FirefoxDriverService.CreateDefaultService(this.seleniumDriverLocation);
                        fireFoxService.SuppressInitialDiagnosticInformation = true;

                        this.WebDriver = new FirefoxDriver(fireFoxService, fireFoxOptions, this.actualTimeOut);
                        this.PID = fireFoxService.ProcessId;
                        break;
                    case Browser.IE:

                        // clean session => clear cache and cookies
                        // native events set to true => allow clicking buttons and links when JS is disabled.
                        // Ignore zoom level to be true since having it as the default per resolution has a better result. (ALM #24960)
                        InternetExplorerOptions ieOptions = new InternetExplorerOptions
                        {
                            IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                            IgnoreZoomLevel = true,
                            EnsureCleanSession = true,
                            EnableNativeEvents = true,
                            UnhandledPromptBehavior = UnhandledPromptBehavior.Accept,
                            RequireWindowFocus = true,

                            // for the old framwork.
                            EnablePersistentHover = true,
                            PageLoadStrategy = PageLoadStrategy.Normal,
                        };
                        InternetExplorerDriverService ieService = InternetExplorerDriverService.CreateDefaultService(this.seleniumDriverLocation);
                        ieService.SuppressInitialDiagnosticInformation = true;

                        try
                        {
                            this.WebDriver = new InternetExplorerDriver(ieService, ieOptions, this.actualTimeOut);
                            this.PID = ieService.ProcessId;
                            Logger.Info($"Internet Driver service PID is: {this.PID}");
                        }
                        catch (InvalidOperationException ioe)
                        {
                            Logger.Error("Please ensure that protected mode is either all on / off on all zones inside internet options. Exception found was: ");
                            Logger.Error($"{ioe.Message}");
                        }

                        // (ALM #24960) Shortkey to set zoom level to default in IE.
                        IWebElement element = this.WebDriver.FindElement(By.TagName("body"));
                        element.SendKeys(Keys.Control + "0");
                        break;
                    case Browser.Safari:

                        Logger.Info("We currently do not deal with Safari yet!");

                        break;
                    default:
                        Logger.Error("Browser Type is null");
                        break;
                }

                this.wdWait = new WebDriverWait(this.WebDriver, this.timeOutThreshold);
                this.Action = new Actions(this.WebDriver);

                if (this.axeDriver == null)
                {
                    this.axeDriver = new AxeDriver(this.WebDriver);
                }
                else
                {
                    // Make sure to update the driver to the new one.
                    this.axeDriver.Driver = this.WebDriver;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"While trying to instantiate Selenium drivers, we were met with the following: {e.ToString()}");
                this.Quit();
            }
        }

        /// <summary>
        /// Sets the tab to be the active one.
        /// </summary>
        private void SetActiveTab()
        {
            if (!this.InIFrame)
            {
                var windows = this.WebDriver.WindowHandles;
                int windowCount = windows.Count;

                // save the current window / tab we are on. Only focus the browser when a new page / tab actually is there.
                if (windowCount != this.CurrentWindow)
                {
                    this.CurrentWindow = windowCount;
                    this.WebDriver.SwitchTo().Window(windows[windowCount - 1]);
                }
            }
            else // future implementation for InIFrame
            {
                // this.wdWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath(this.IFrameXPath)));
            }
        }
    }
}
