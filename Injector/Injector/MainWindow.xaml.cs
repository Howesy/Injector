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
using System.Diagnostics;
using System.IO;

namespace WPF_Hexus_Injector
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

        public bool ActiveCheck(string processName) {
            foreach (Process process in Process.GetProcesses()) {
                if (process.ProcessName.Contains(processName)) return true;
            } return false;
        }

        private void Btn_Inject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Inject Injection = new Inject("ENTER PROCESS NAME");          
                Injection.InjectDLL(Injection.CurrentDirectory, "DLLNAME.dll");
                MessageBox.Show("Injection successful!");
            } catch(Exception error) {
                MessageBox.Show(error.Message.ToString(), "Error Occured!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        private void Btn_Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("Redacted help link.");
        }
    }
}
