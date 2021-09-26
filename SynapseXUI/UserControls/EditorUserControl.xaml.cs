using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;

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

        private void MenuItemEditorsCloseAllTabs_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseAllTabs();
        }

        private void MenuItemEditorsCloseAllButSelected_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CloseAllTabs(ViewModel.SelectedTab);
        }

        private void CloseTabCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.CloseTab(e.Parameter as MetroTabItem);
        }
    }
}
