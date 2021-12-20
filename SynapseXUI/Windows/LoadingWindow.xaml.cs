using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : MetroWindow
    {
        private readonly LoadingWindowViewModel viewModel;

        public LoadingWindow()
        {
            InitializeComponent();
            viewModel = new LoadingWindowViewModel(this);
            DataContext = viewModel;
        }

        private void IconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            App.Settings.Save(App.SettingsFilePath);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            App.Settings.Save(App.SettingsFilePath);
        }
    }
}
