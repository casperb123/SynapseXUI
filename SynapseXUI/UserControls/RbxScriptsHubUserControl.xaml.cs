﻿using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for RbxScriptsHubUserControl.xaml
    /// </summary>
    public partial class RbxScriptsHubUserControl : UserControl
    {
        public static RoutedCommand ClearTextCommand = new RoutedCommand();

        private readonly RbxScriptsHubUserControlViewModel viewModel;

        public RbxScriptsHubUserControl()
        {
            InitializeComponent();
            viewModel = new RbxScriptsHubUserControlViewModel();
            DataContext = viewModel;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ExecuteScript(sender as Tile);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void TextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                viewModel.FilterScripts();
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            viewModel.FilterScripts();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            viewModel.SearchQuery = null;
            viewModel.FilterScripts();
        }
    }
}
