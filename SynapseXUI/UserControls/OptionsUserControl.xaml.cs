using SynapseXUI.Entities;
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
            if (IsLoaded)
            {
                viewModel.SaveSxOptions();
            }
        }

        private void ToggleSwitchAutoLaunch_Toggled(object sender, RoutedEventArgs e)
        {
            if (IsLoaded &&
                viewModel.AutoLaunch &&
                App.ShowPrompt("Auto Launch", "You have selected to enable the Auto Launch option.\n\n" +
                                              "Please note that this option replaces your launcher with a custom one made by Synapse X. Are you sure you want to continue?", PromptType.YesNo))
            {
                viewModel.SaveSxOptions();
            }
            else
            {
                viewModel.AutoLaunch = false;
            }
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
