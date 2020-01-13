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

namespace SeleniumUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            buttonClose.IsEnabled = false;
            buttonStart.IsEnabled = false;

        }

        /// <summary>
        /// Happens when the start button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = false;
            buttonClose.IsEnabled = true;
            dropDownBrowsers.IsEnabled = false;
        }

        /// <summary>
        /// Happens when the close button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            buttonStart.IsEnabled = true;
            buttonClose.IsEnabled = false;
            dropDownBrowsers.IsEnabled = true;
        }

        private void dropDownBrowsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dropDownBrowsers.SelectedIndex > 0)
            {
                buttonStart.IsEnabled = true;
            }// for some reason, the button start does not exist yet. Probably because it loads later
            else if (buttonStart != null)
            {
                buttonStart.IsEnabled = false;
            }
        }
    }
}
