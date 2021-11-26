﻿using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using static MahApps.Metro.Controls.BaseMetroTabControl;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for EditorUserControl.xaml
    /// </summary>
    public partial class EditorUserControl : UserControl
    {
        public static EditorUserControl Instance { get; private set; }

        public readonly EditorUserControlViewModel ViewModel;

        public EditorUserControl()
        {
            InitializeComponent();
            Instance = this;
            ViewModel = new EditorUserControlViewModel(this);
            DataContext = ViewModel;
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExecuteScript(ViewModel.SelectedTab.Text);
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(true);
        }

        private void ButtonExecuteFile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExecuteFile();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearEditorText();
        }

        private void MenuItemScriptsExecute_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExecuteScript(ViewModel.SelectedScriptFile.Script);
        }

        private void MenuItemScriptsLoadCurrentTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(false, ViewModel.SelectedScriptFile.FullFilename);
        }

        private void MenuItemScriptsLoadNewTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(true, ViewModel.SelectedScriptFile.FullFilename);
        }

        private void MenuItemScriptsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteFile();
        }

        private void MenuItemReloadTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReloadTab();
        }

        private void MenuItemEditorsCloseAllTabs_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseAllTabs();
        }

        private void MenuItemEditorsCloseAllButSelected_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseAllTabs(ViewModel.SelectedTab);
        }

        private void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedTab.FullFilename))
            {
                ViewModel.SaveFileAs(ViewModel.SelectedTab.Text);
            }
            else
            {
                ViewModel.SaveFile(ViewModel.SelectedTab);
            }
        }

        private void ButtonSaveFileAs_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveFileAs(ViewModel.SelectedTab.Text);
        }

        private void TabControlEditors_TabItemClosingEvent(object sender, TabItemClosingEventArgs e)
        {
            e.Cancel = true;

            ScriptTab scriptTab = e.ClosingTabItem.Tag as ScriptTab;
            ViewModel.CloseTab(scriptTab, true);
        }

        private void TabControlEditors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ChangeTab();
        }

        private void ListBoxScripts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            App.Settings.ScriptsListWidth = new GridLength(columnScripts.ActualWidth);
        }
    }
}
