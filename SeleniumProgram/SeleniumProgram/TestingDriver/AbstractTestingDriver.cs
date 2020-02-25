namespace AutomationTestingProgram.TestingDriver
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The Interface for the Testing Driver software.
    /// </summary>
    public abstract class AbstractTestingDriver
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
        /// Gets or sets the url of the page the webdriver is focued on.
        /// </summary>
        public string CurrentURL { get; set; }

        /// <summary>
        /// Gets or sets the loadiong spinner that appears on the website.
        /// </summary>
        public string LoadingSpinner { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error container to check if any errors are shown on the UI.
        /// </summary>
        public string ErrorContainer { get; set; } = string.Empty;


        /// <summary>
        /// Checks for an element state.
        /// </summary>
        /// <returns>True if the element is in that state. false otherwise.</returns>
        public abstract bool CheckForElementState();

        /// <summary>
        /// Performs the actions of clicking the specified element.
        /// </summary>
        public abstract void ClickElement();
    }
}
