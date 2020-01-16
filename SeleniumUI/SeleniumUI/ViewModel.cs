// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// The ViewModel Class.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// Constructor.
        /// </summary>
        public ViewModel()
        {
            this.Button = new ButtonObject
            {
                IsStartEnabled = false,
                IsCloseEnabled = false,
                IsBrowserSelectEnabled = true,
                IsStopEnabled = false,
                IsRecEnabled = false,
                Driver = new SeleniumDriver(),
            };
            this.ApplyChangesCommand = new RelayCommand(
                o => this.ExecuteApplyChangesCommand(),
                o => this.Button.IsValid);
            this.ClickClose = new RelayCommand(
                o => this.Button.ClickClose(),
                o => this.Button.IsValid);
            this.ClickRec = new RelayCommand(
                o => this.Button.ClickRec(),
                o => this.Button.IsValid);
            this.ClickStop = new RelayCommand(
                o => this.Button.ClickStop(),
                o => this.Button.IsValid);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the buttons to edit.
        /// </summary>
        public ButtonObject Button { get; set; }

        /// <summary>
        /// Gets the "apply changes" command.
        /// </summary>
        public ICommand ApplyChangesCommand { get; private set; }

        /// <summary>
        /// Start button command.
        /// </summary>
        /// <param name="browser">the browser to open.</param>
        /// <returns>returns the command.</returns>
        public ICommand ClickStart(string browser)
        {
            return new RelayCommand(
                o => this.Button.ClickStart(browser),
                o => this.Button.IsValid);
        }

        /// <summary>
        /// Gets closes the browser.
        /// </summary>
        public ICommand ClickClose { get; private set; }

        /// <summary>
        /// Gets closes the browser.
        /// </summary>
        public ICommand ClickRec { get; private set; }

        /// <summary>
        /// Gets closes the browser.
        /// </summary>
        public ICommand ClickStop { get; private set; }

        /// <summary>
        /// Executes the 'apply changes' command.
        /// </summary>
        private void ExecuteApplyChangesCommand()
        {
            // stuff
        }
    }
}