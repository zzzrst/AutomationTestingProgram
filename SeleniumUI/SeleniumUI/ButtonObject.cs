namespace SeleniumUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// Object holding all of the button properties.
    /// </summary>
    public class ButtonObject : INotifyPropertyChanged
    {
        private bool startEnabled;
        private bool closeEnabled;
        private bool stopEnabled;
        private bool browserSelectEnable;
        private bool recEnabled;
        private bool playEnabled;
        private bool isValid;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the browser dropdown is enabled.
        /// </summary>
        public bool IsBrowserSelectEnabled
        {
            get
            {
                return this.browserSelectEnable;
            }

            set
            {
                if (this.browserSelectEnable != value)
                {
                    this.browserSelectEnable = value;
                    this.OnPropertyChange(value.ToString());
                    this.SetIsValid();
                }
            }
        }

        /// <summary>
        /// Gets or sets Selenium driver.
        /// </summary>
        public SeleniumDriver Driver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether close button is enabled.
        /// </summary>
        public bool IsCloseEnabled
        {
            get
            {
                return this.closeEnabled;
            }

            set
            {
                if (this.closeEnabled != value)
                {
                    this.closeEnabled = value;
                    this.OnPropertyChange(value.ToString());
                    this.SetIsValid();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether start button is enabled.
        /// </summary>
        public bool IsStartEnabled
        {
            get
            {
                return this.startEnabled;
            }

            set
            {
                if (this.startEnabled != value)
                {
                    this.startEnabled = value;
                    this.OnPropertyChange(value.ToString());
                    this.SetIsValid();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether stop button is enabled.
        /// </summary>
        public bool IsStopEnabled
        {
            get
            {
                return this.stopEnabled;
            }

            set
            {
                if (this.stopEnabled != value)
                {
                    this.stopEnabled = value;
                    this.OnPropertyChange(value.ToString());
                    this.SetIsValid();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether record button is enabled.
        /// </summary>
        public bool IsRecEnabled
        {
            get
            {
                return this.recEnabled;
            }

            set
            {
                if (this.recEnabled != value)
                {
                    this.recEnabled = value;
                    this.OnPropertyChange(value.ToString());
                    this.SetIsValid();
                }
            }
        }

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
                    this.OnPropertyChange("IsValid");
                }
            }
        }

        /// <summary>
        /// Called action when start button is clicked.
        /// </summary>
        /// <param name="browser">browser to open.</param>
        public void ClickStart(string browser)
        {
            this.IsStartEnabled = false;
            this.IsCloseEnabled = true;
            this.IsBrowserSelectEnabled = false;
            this.IsRecEnabled = true;
            this.Driver.OpenBrowser(browser);
        }

        /// <summary>
        /// Called action when close button is clicked.
        /// </summary>
        public void ClickClose()
        {
            this.IsStartEnabled = true;
            this.IsCloseEnabled = false;
            this.IsBrowserSelectEnabled = true;
            this.IsRecEnabled = false;
            this.IsStopEnabled = false;
            this.Driver.CloseBrowser();
        }

        /// <summary>
        /// Called action when rec button is clicked.
        /// </summary>
        /// <param name="browser">browser to open.</param>
        public void ClickRec()
        {
            this.IsRecEnabled = false;
            this.IsStopEnabled = true;
        }

        /// <summary>
        /// Called action when stop button is clicked.
        /// </summary>
        public void ClickStop()
        {
            this.IsRecEnabled = true;
            this.IsStopEnabled = false;
        }

        /// <summary>
        /// Sets the value of isValid.
        /// </summary>
        private void SetIsValid()
        {
            this.IsValid = (!this.IsStartEnabled || !this.IsCloseEnabled) && (!this.IsRecEnabled || !this.IsStopEnabled);
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
