using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using SynapseXUI.Windows;
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
                PromptWindow.Show("Auto Launch", "You have selected to enable the Auto Launch option. Currently this works just as the Auto Attach option, so you need to have this UI open before opening roblox.\n\n" +
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

        private void HyperlinkResetOptions_Click(object sender, RoutedEventArgs e)
        {
            if (PromptWindow.Show("Reset settings", "Are you sure that you want to reset the settings to default?", PromptType.YesNo))
            {
                App.ResetSettings();
            }
        }

        private void HyperlinkResetWindowSize_Click(object sender, RoutedEventArgs e)
        {
            if (PromptWindow.Show("Reset Window Size", "Are you sure that you want to reset the window size and state to default?", PromptType.YesNo))
            {
                App.Settings.WindowSize.SetDefault();
            }
        }

        private void HyperlinkResetScriptsWidth_Click(object sender, RoutedEventArgs e)
        {
            if (PromptWindow.Show("Reset Scripts Width", "Are you sure that you want to reset the scripts listbox width to default?", PromptType.YesNo))
            {
                App.Settings.ScriptsListWidth = new GridLength(200);
                EditorUserControl.Instance.columnScripts.Width = App.Settings.ScriptsListWidth;
            }
        }
    }
}
