using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for OptionsUserControl.xaml
    /// </summary>
    public partial class OptionsUserControl : UserControl
    {
        private readonly OptionsUserControlViewModel viewModel;

        public OptionsUserControl()
        {
            InitializeComponent();
            viewModel = new OptionsUserControlViewModel(this);
            DataContext = viewModel;
        }

        private void ToggleSwitchSxOptions_Toggled(object sender, RoutedEventArgs e)
        {
            viewModel.SaveSxOptions();
        }

        private void ComboBoxSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                viewModel.SaveSettings();
            }
        }

        private void ToggleSwitchSettings_Toggled(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
            {
                viewModel.SaveSettings();
            }
        }
    }
}
