// <copyright file="ITestingDriver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AutomationTestingProgram.TestAutomationDriver
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The Interface for the Testing Driver software.
    /// </summary>
    public interface ITestAutomationDriver
    {
        /// <summary>
        /// Different browsers that are supported.
        /// </summary>
        public enum Browser
        {
            /// <summary>
            /// Represents the Chrome browser.
            /// </summary>
            Chrome,

            /// <summary>
            /// Represents the Microsoft Edge Browser.
            /// </summary>
            Edge,

            /// <summary>
            /// Represents the Firefox Browser.
            /// </summary>
            Firefox,

            /// <summary>
            /// Represents the Internet Explorer browser.
            /// </summary>
            IE,

            /// <summary>
            /// Represents the Safari Browser.
            /// </summary>
            Safari,
        }

        /// <summary>
        /// Different states of the element.
        /// </summary>
        public enum ElementState
        {
            /// <summary>
            /// Element cannot be found / seen.
            /// </summary>
            Invisible,

            /// <summary>
            /// Element can be seen.
            /// </summary>
            Visible,

            /// <summary>
            /// Element can be clicked.
            /// </summary>
            Clickable,
        }

        /// <summary>
        /// Gets the name of the testing driver.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the url of the page the webdriver is focued on.
        /// </summary>
        public string CurrentURL { get; set; }

        /// <summary>
        /// Gets or sets the loadiong spinner that appears on the website.
        /// </summary>
        public string LoadingSpinner { get; set; }

        /// <summary>
        /// Gets or sets the error container to check if any errors are shown on the UI.
        /// </summary>
        public string ErrorContainer { get; set; }

        /// <summary>
        /// Checks for an element state.
        /// </summary>
        /// <param name="xPath"> The xpath to find the web element. </param>
        /// <param name="state"> The state of the web element to wait for. </param>
        /// <returns> If the element state is as wanted.</returns>
        public bool CheckForElementState(string xPath, ElementState state);

        /// <summary>
        /// Performs the actions of clicking the specified element. Uses Selenium binding by default.
        /// </summary>
        /// <param name="xPath">The xpath to find the specified element.</param>
        /// <param name="byJS"> Whether to use JS to perform the click / not. </param>
        public void ClickElement(string xPath, bool byJS = false);

        /// <summary>
        /// Closes the current window. It will quit the browser if it is the last window opened.
        /// </summary>
        public void CloseBrowser();

        /// <summary>
        /// Quits the webdriver. Call this when you want the driver to be closed.
        /// </summary>
        public void Quit();

        /// <summary>
        /// Maximizes the browser.
        /// </summary>
        public void Maximize();

        /// <summary>
        /// Generates the AODA results.
        /// </summary>
        /// <param name="folderLocation"> The folder to generate AODA results in. </param>
        public void GenerateAODAResults(string folderLocation);

        /// <summary>
        /// Tells the browser to navigate to the provided url.
        /// </summary>
        /// <param name="url">URL for the browser to navigate to.</param>
        /// <param name="instantiateNewDriver">Instantiates a new selenium driver.</param>
        /// <returns> <code>true</code> if the navigation was successful. </returns>
        public bool NavigateToURL(string url = "", bool instantiateNewDriver = true);

        /// <summary>
        /// Performs the action of populating a value.
        /// </summary>
        /// <param name="xPath"> The xpath to use to identify the element. </param>
        /// <param name="value"> The value to populate.</param>
        public void PopulateElement(string xPath, string value);

        /// <summary>
        /// Refreshes the webpage.
        /// </summary>
        public void RefreshWebPage();

        /// <summary>
        /// Method to run aoda on the current web page.
        /// </summary>
        /// <param name="providedPageTitle"> Title of the web page the user provides. </param>
        public void RunAODA(string providedPageTitle);

        /// <summary>
        /// Performs the action of selecting a value in an element.
        /// </summary>
        /// <param name="xPath"> The xpath to use to identify the element. </param>
        /// <param name="value"> The value to select in the element.</param>
        public void SelectValueInElement(string xPath, string value);

        /// <summary>
        /// Switches to appropriate IFrame.
        /// </summary>
        /// <param name="xPath"> xPath to find the iFrame.</param>
        public void SwitchToIFrame(string xPath);

        /// <summary>
        /// The SwitchToTab.
        /// </summary>
        /// <param name="tab">The tab<see cref="int"/>.</param>
        public void SwitchToTab(int tab);

        /// <summary>
        /// Takes a screenshot of the browser. Screenshot will have the datestamp as its name. Year Month Date Hour Minutes Seconds (AM/PM).
        /// </summary>
        public void TakeScreenShot();

        /// <summary>
        /// Waits for an element state.
        /// </summary>
        /// <param name="xPath"> The xpath to find the web element. </param>
        /// <param name="state"> The state of the web element to wait for. </param>
        public void WaitForElementState(string xPath, ElementState state);

        /// <summary>
        /// Waits until the loading spinner disappears.
        /// </summary>
        public void WaitForLoadingSpinner();

        /// <summary>
        /// Checks if there are any errors in the error container.
        /// </summary>
        public void CheckErrorContainer();
    }
}
