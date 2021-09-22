﻿using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for EditorUserControl.xaml
    /// </summary>
    public partial class EditorUserControl : UserControl
    {
        public readonly EditorUserControlViewModel ViewModel;

        public static EditorUserControl Instance { get; set; }

        public EditorUserControl()
        {
            InitializeComponent();
            ViewModel = new EditorUserControlViewModel(this);
            DataContext = ViewModel;
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(true);
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearEditorText();
        }

        private void MenuItemScriptsLoadCurrentTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(false, ViewModel.SelectedScriptFile.FullFilename);
        }

        private void MenuItemScriptsLoadNewTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(true, ViewModel.SelectedScriptFile.FullFilename);
        }

        private void MenuItemEditorsCloseAllTabs_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseAllTabs();
        }

        private void MenuItemEditorsCloseAllButSelected_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseAllTabs(ViewModel.SelectedTab);
        }
    }
}
