using CefSharp;
using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private bool hubsInitiated;

        public readonly MainWindowViewModel ViewModel;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.EditorUserControl.ViewModel.SaveTabs();
            ViewModel.SaveSettings();
            Cef.ShutdownWithoutChecks();
        }

        private void ButtonAttach_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Attach();
        }

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabItemScriptHub.IsSelected && !hubsInitiated)
            {
                ViewModel.ScriptHubUserControl.InitiateScriptHubs();
                hubsInitiated = true;
            }
        }

        private void ButtonExecuteSynapseHubScript_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSynapseHubScript.Script.Execute();
        }

        private void ButtonExecuteRbxHubScript_Click(object sender, RoutedEventArgs e)
        {
            App.Lib.Execute(ViewModel.SelectedRbxHubScript.Excerpt);
        }

        private void ButtonKillRoblox_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.KillRoblox();
        }

        private void ButtonReinstallRoblox_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReinstallRoblox();
        }

        private void IconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            App.ShowPrompt("Synapse X", "This UI is developed by Casper:\n" +
                                        "Discord: CasperTheGhost#3549", PromptType.OK);
        }
    }
}
