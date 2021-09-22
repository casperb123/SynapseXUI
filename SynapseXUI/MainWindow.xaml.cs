using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System;
using System.ComponentModel;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel viewModel;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            viewModel = new MainWindowViewModel(this);
            DataContext = viewModel;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ButtonAttach_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel.Attach();
        }
    }
}
