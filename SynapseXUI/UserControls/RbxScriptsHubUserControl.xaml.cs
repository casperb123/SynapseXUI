﻿using SynapseXUI.Entities;
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

        public RbxScriptsHubUserControlViewModel ViewModel { get; private set; }

        public RbxScriptsHubUserControl()
        {
            InitializeComponent();
            ViewModel = new RbxScriptsHubUserControlViewModel();
            DataContext = ViewModel;
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
                ViewModel.FilterScripts();
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.FilterScripts();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            textBoxSearch.Focus();
            ViewModel.SearchQuery = null;
            ViewModel.FilterScripts();
        }

        private void ButtonReload_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetScripts();
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            string script = ((Button)sender).Tag as string;
            App.Lib.Execute(script);
        }

        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            RbxHubScript script = ((Button)sender).Tag as RbxHubScript;
            ViewModel.DownloadScript(script);
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            string slug = ((Button)sender).Tag as string;
            Process.Start($"https://rbxscripts.xyz/{slug}");
        }
    }
}
