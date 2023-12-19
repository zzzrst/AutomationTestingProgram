using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // this method creates a run for TAP
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ButtonClicked");

            string caption = "Alert";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result;

            string messageBoxText = "Placeholder text";

            // file we're running
            string fileName = this.FileNameTextbox.Text;

            if (fileName == null || fileName.Length == 0)
            {
                messageBoxText = "Missing mandatory file name";
            }
            else
            {
                messageBoxText += "Running " + fileName;
            }

            // mandatory fields
            string env = this.EnvironmentValue.ToString();
            string browser = this.BrowserPicker.Text;
            string projectName = this.ProjectNameValues.Text;
            string planName = this.PlanNameValues.Text;
            string notifyList = this.NotifyListValues.Text;

            // additional parameters
            string reportToDevOps = this.ReportToDevOpsCheckbox.IsChecked.ToString() ?? "False";
            string generateHTML = this.GenerateHTMLCheckbox.IsChecked.ToString() ?? "False";
            string runAODA = this.RunAODACheckbox.IsChecked.ToString() ?? "False";
            string debugMode = this.DebugModeCheckbox.IsChecked.ToString() ?? "False";
            string runOnDevOps = this.DevOpsCheckbox.IsChecked.ToString() ?? "False";

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
        }

        private void Environment_TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("Textbox Text changed");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Combobox selection changed");
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("Textbox text changed 1");
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            Console.WriteLine("textbox text changed 2");
        }

        private void FileNameTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PlanNameValues_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ProjectNameValues_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BrowseFilesButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".xlsx"; // Default file extension
            dialog.Filter = "Excel documents (.xlsx)|*.xlsx"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                this.FileNameTextbox.Text = filename;
            }
        }
    }
}