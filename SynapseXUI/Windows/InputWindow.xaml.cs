using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SynapseXUI.Windows
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : MetroWindow
    {
        public readonly InputWindowViewModel ViewModel;

        public InputWindow(string title, string message, object input, InputDataType type)
        {
            InitializeComponent();
            ViewModel = new InputWindowViewModel(this, title, message, input, type);
            DataContext = ViewModel;

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

        private void IconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            ResultTrue();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ResultTrue()
        {
            if (ViewModel.Type == InputDataType.Text)
            {
                if (!string.IsNullOrWhiteSpace(ViewModel.Input.ToString()))
                {
                    DialogResult = true;
                }
            }
        }

        public static (bool result, object input) Show(string title, string message, object input, InputDataType type)
        {
            InputWindow window = new InputWindow(title, message, input, type);

            if (window.ShowDialog().Value)
            {
                return (true, window.ViewModel.Input);
            }

            return (false, null);
        }

        private void TextBoxInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, @"[<>:""/\\|?*]+");
        }

        private void TextBoxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ResultTrue();
            }
        }
    }
}
