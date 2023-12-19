using System.Diagnostics;
using System.Reflection;
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
using System.IO;
using log4net;
using log4net.Repository.Hierarchy;

namespace TestRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                messageBoxText = "Running " + fileName;
            }

            // mandatory fields
            string azurePAT = "****";

            string env = this.EnvironmentValue.Text;
            string browser = this.BrowserPicker.Text;
            string projectName = this.ProjectNameValues.Text;
            string planName = this.PlanNameValues.Text;
            string notifyList = this.NotifyListValues.Text;
            string planStructure = this.PlanStructure.Text;
            string buildNumber = this.BuildNumberValue.Text;

            // additional parameters
            string reportToDevOps = this.ReportToDevOpsCheckbox.IsChecked.ToString() ?? "False";
            string generateHTML = this.GenerateHTMLCheckbox.IsChecked.ToString() ?? "False";
            string runAODA = this.RunAODACheckbox.IsChecked.ToString() ?? "False";
            string debugMode = this.DebugModeCheckbox.IsChecked.ToString() ?? "False";

            string runOnDevOps = this.DevOpsCheckbox.IsChecked.ToString() ?? "False";

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);

            // if runOnDevOps is checked
            if (runOnDevOps == "True")
            {
                string pathToRunner = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\runOnDevOps.ps1";
                string releaseID = this.PathToExecutableValueAndReleaseIDValue.Text;

                string command = $"powershell -Executionpolicy Bypass \"&'{pathToRunner}' " +
                      $"-browser '{browser}' " +
                      $"-azurePAT '{azurePAT}' " +
                      $"-notifyList '{notifyList}' " +
                      $"-fileNames '{fileName}' " +
                      $"-planName '{planName}' " +
                      $"-project '{projectName}' " +
                      $"-buildNumber '{buildNumber}' " +
                      $"-environment '{env}' " +
                      $"-releaseID '{releaseID}' " +
                      $"-planStructure '{planStructure}' \"";

                RunTestSetDevOps(command);
            }
            else
            {
                string pathToExe = this.PathToExecutableValueAndReleaseIDValue.Text;

                // run on local
                RunTestSetLocally(
                    pathToExecutable: pathToExe,
                    projectName: projectName,
                    planName: planName,
                    setArgs: fileName,
                    browser: browser,
                    buildNumber: buildNumber,
                    environment: env,
                    respectRunAODAFlag: runAODA,
                    notifyList: notifyList);
            }
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
        private void BrowseForExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".xlsx"; // Default file extension
            dialog.Filter = "Exe documents (.exe)|*.exe"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
                this.PathToExecutableValueAndReleaseIDValue.Text = filename;
            }
        }
        private void DevOpsCheckbox_Checked_1(object sender, RoutedEventArgs e)
        {

            if (this.DevOpsCheckbox.IsChecked != true)
            {
                this.PathToExecutableLabelAndReleaseIdLabel.Text = "Path to Executable";
                this.BrowseForExecutableButton.Visibility = Visibility.Visible;
            }
            else if (this.DevOpsCheckbox.IsChecked == true)
            {
                this.PathToExecutableLabelAndReleaseIdLabel.Text = "DevOps Release ID";
                this.BrowseForExecutableButton.Visibility = Visibility.Hidden;
            }
        }

        private static int RunTestSetDevOps(string command)
        {
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "cmd.exe",
                Arguments = $"/C exit | {command}",
            };
            p.StartInfo = startInfo;
            p.Start();
            string line;

            while ((line = p.StandardOutput.ReadLine()) != null)
            {
                log.Info(line);
            }

            p.WaitForExit();

            if (p.ExitCode == 0)
            {
                MessageBox.Show("Successfully completed execution to DevOps", "Successful execution", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Failed, see executionLogger.log for more details.", "Failed Execution", MessageBoxButton.OK);
            }

            return p.ExitCode;
        }

        public void RunTestSetLocally(string pathToExecutable, string projectName, string planName, string setArgs, string browser, string buildNumber, string environment,
             string respectRunAODAFlag, string notifyList)
        {
            string cmdToRun = $"{pathToExecutable} --setArgs \"{setArgs}\"";

            if (browser != string.Empty)
            {
                cmdToRun += $" --browser \"{browser}\"";
            }
            if (environment != string.Empty)
            {
                cmdToRun += $" --environment \"{environment}\"";
            }
            if (buildNumber != string.Empty)
            {
                cmdToRun += $" --buildNumber \"{buildNumber}\"";
            }
            if (respectRunAODAFlag != string.Empty)
            {
                cmdToRun += $" --respectRunAODAFlag \"{respectRunAODAFlag}\"";
            }
            if (notifyList != string.Empty)
            {
                cmdToRun += $" --notifyList \"{notifyList}\"";
            }
            if (planName != string.Empty)
            {
                cmdToRun += $" --planName \"{planName}\"";
            }
            if (projectName != string.Empty)
            {
                cmdToRun += $" --projectName \"{projectName}\"";
            }

            log.Info("Logging " + cmdToRun);

            try
            {
                Process p = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    // we use shell to execute (not headless)
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe",
                    Arguments = $"/C exit | {cmdToRun}",
                };
                p.StartInfo = startInfo;

                p.Start();

                string line;
                while ((line = p.StandardOutput.ReadLine()) != null)
                {
                    log.Info(line);
                }

                p.WaitForExit();

                if (p.ExitCode == 0)
                {
                    MessageBox.Show("Successfully completed execution locally", "Successful execution", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Failed, see executionLogger.log for details.", "Failed Execution", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}