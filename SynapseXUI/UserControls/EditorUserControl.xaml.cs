using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static MahApps.Metro.Controls.BaseMetroTabControl;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for EditorUserControl.xaml
    /// </summary>
    public partial class EditorUserControl : UserControl
    {
        public readonly EditorUserControlViewModel ViewModel;

        public EditorUserControl()
        {
            InitializeComponent();
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

        private void ListBoxScripts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ViewModel.DeleteFile();
            }
        }

        private async void TabControlEditors_TabItemClosingEvent(object sender, TabItemClosingEventArgs e)
        {
            e.Cancel = true;

            if (App.SxOptions.CloseConfirmation)
            {
                MessageDialogResult result = await MainWindow.Instance.ShowMessageAsync("Close Tab", "Are you sure that you want to close this tab? All changes will be lost!", MessageDialogStyle.AffirmativeAndNegative, App.DialogSettings);
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }

            ViewModel.CloseTab(e.ClosingTabItem, true);
        }
    }
}
