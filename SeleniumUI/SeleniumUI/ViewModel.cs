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
    public class ViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// Constructor.
        /// </summary>
        public ViewModel()
        {
            this.Model = new ModelControler
            {
                StartButton = new ButtonObject() { IsObjectEnabled = false, },
                IsStartEnabled = false,
                IsCloseEnabled = false,
                IsBrowserSelectEnabled = true,
                IsStopEnabled = false,
                IsRecEnabled = false,
                Driver = new SeleniumDriver(),
            };
            this.ApplyChangesCommand = new RelayCommand(
                o => this.ExecuteApplyChangesCommand(),
                o => this.Model.IsValid);
            this.ClickClose = new RelayCommand(
                o => this.Model.ClickClose(),
                o => this.Model.IsValid);
            this.ClickRec = new RelayCommand(
                o => this.Model.ClickRec(),
                o => this.Model.IsValid);
            this.ClickStop = new RelayCommand(
                o => this.Model.ClickStop(),
                o => this.Model.IsValid);
        }

        /// <summary>
        /// Gets or sets the buttons to edit.
        /// </summary>
        public ModelControler Model { get; set; }

        /// <summary>
        /// Gets the "apply changes" command.
        /// </summary>
        public ICommand ApplyChangesCommand { get; private set; }

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
        /// Start button command.
        /// </summary>
        /// <param name="browser">the browser to open.</param>
        /// <returns>returns the command.</returns>
        public ICommand ClickStart(string browser)
        {
            return new RelayCommand(
                o => this.Model.ClickStart(browser),
                o => this.Model.IsValid);
        }

        /// <summary>
        /// Executes the 'apply changes' command.
        /// </summary>
        private void ExecuteApplyChangesCommand()
        {
            // stuff
        }
    }
}