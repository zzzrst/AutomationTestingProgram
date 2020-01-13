// <copyright file="MainWindow.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SeleniumUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Creates a new Selenium driver.
        /// </summary>
        private SeleniumDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.buttonClose.IsEnabled = false;
            this.buttonStart.IsEnabled = false;
            this.driver = new SeleniumDriver();
        }

        /// <summary>
        /// Happens when the start button is clicked.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">.</param>
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            this.buttonStart.IsEnabled = false;
            this.buttonClose.IsEnabled = true;
            this.dropDownBrowsers.IsEnabled = false;
            this.driver.OpenBrowser(this.dropDownBrowsers.Text);
        }

        /// <summary>
        /// Happens when the close button is clicked.
        /// </summary>
        /// <param name="sender">sender.</param>
        /// <param name="e">.</param>
        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.buttonStart.IsEnabled = true;
            this.buttonClose.IsEnabled = false;
            this.dropDownBrowsers.IsEnabled = true;
            this.driver.CloseBrowser();
        }

        private void DropDownBrowsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dropDownBrowsers.SelectedIndex > 0)
            {
                this.buttonStart.IsEnabled = true;
            }// for some reason, the button start does not exist yet. Probably because it loads later
            else if (this.buttonStart != null)
            {
                this.buttonStart.IsEnabled = false;
            }
        }
    }
}
