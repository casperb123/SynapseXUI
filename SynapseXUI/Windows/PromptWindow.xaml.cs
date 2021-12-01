using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SynapseXUI.Windows
{
    /// <summary>
    /// Interaction logic for PromptWindow.xaml
    /// </summary>
    public partial class PromptWindow : MetroWindow
    {
        private readonly PromptWindowViewModel viewModel;

        public PromptWindow(string title, string message, PromptType type)
        {
            InitializeComponent();
            viewModel = new PromptWindowViewModel(this, title, message, type);
            DataContext = viewModel;

            if (Application.Current.MainWindow != this)
            {
                Owner = Application.Current.MainWindow;
                if (App.SxOptions != null)
                {
                    Topmost = App.SxOptions.TopMost;
                }
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void IconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public static bool Show(string title, string message, PromptType type)
        {
            PromptWindow prompt = new PromptWindow(title, message, type);
            return prompt.ShowDialog().Value;
        }
    }
}
