using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System;
using System.ComponentModel;
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
            Environment.Exit(0);
        }

        private void ButtonAttach_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.Attach();
        }

        private void TabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabItemScriptHub.IsSelected)
            {
                ViewModel.LoadScriptHub();
            }
        }
    }
}
