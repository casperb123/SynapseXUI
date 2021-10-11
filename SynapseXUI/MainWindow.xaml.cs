using CefSharp;
using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
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
            App.Lib.ScriptHubMarkAsClosed();
        }

        private void ButtonAttach_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Attach();
        }

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabItemScriptHub.IsSelected)
            {
                App.Lib.ScriptHub();
            }
        }

        private void ButtonExecuteHubScript_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedHubScript.Script.Execute();
        }

        private void ButtonUtilities_Click(object sender, RoutedEventArgs e)
        {
            flyoutUtilities.IsOpen = true;
        }

        private void ButtonKillRoblox_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.KillRoblox();
        }
    }
}
