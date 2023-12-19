using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("ButtonClicked");
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
    }
}