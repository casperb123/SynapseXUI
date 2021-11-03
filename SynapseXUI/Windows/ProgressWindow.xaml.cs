using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SynapseXUI.Windows
{
    /// <summary>
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : MetroWindow
    {
        private readonly ProgressWindowViewModel viewModel;

        public ProgressWindow(string title, string message, bool buttonVisible)
        {
            InitializeComponent();
            viewModel = new ProgressWindowViewModel(title, message, buttonVisible);
            DataContext = viewModel;

            if (Application.Current.MainWindow != this)
            {
                Owner = Application.Current.MainWindow;
                Topmost = App.SxOptions.TopMost;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void IconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void SetProgress(double progress)
        {
            viewModel.Progress = progress;
        }

        public void SetMessage(string message)
        {
            viewModel.Message = message;
        }

        public void SetButtonEnabled(bool enabled)
        {
            viewModel.ButtonEnabled = enabled;
        }

        public static ProgressWindow Show(string title, string message, bool buttonVisible)
        {
            ProgressWindow window = new ProgressWindow(title, message, buttonVisible);
            window.Show();
            return window;
        }
    }
}
