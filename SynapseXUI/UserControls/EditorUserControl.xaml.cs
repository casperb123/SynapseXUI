using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.Linq;
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

        private void MenuItemScriptsRename_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RenameFile(ViewModel.SelectedScriptFile);
        }

        private void MenuItemScriptsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteFile();
        }

        private void MenuItemRenameTab_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedTab.FullFilename))
            {
                ViewModel.RenameTab();
            }
            else
            {
                ViewModel.RenameFile(ViewModel.ScriptFiles.FirstOrDefault(x => x.FullFilename == ViewModel.SelectedTab.FullFilename));
            }
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

        private void ButtonSaveScript_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ViewModel.SelectedTab.FullFilename))
            {
                ViewModel.SaveScriptAs(ViewModel.SelectedTab);
            }
            else
            {
                ViewModel.SaveScript(ViewModel.SelectedTab);
            }
        }

        private void ButtonSaveScriptAs_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveScriptAs(ViewModel.SelectedTab);
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
            App.Settings.ScriptsListWidth = columnScripts.Width;
        }

        private void MetroTabItem_Drop(object sender, DragEventArgs e)
        {
            if (e.AllowedEffects.HasFlag(DragDropEffects.Move) &&
                e.Data.GetDataPresent("ScriptTab") &&
                ViewModel.Dragging.tab != null)
            {
                ViewModel.DropScriptTab(e.OriginalSource);
                e.Effects = DragDropEffects.Move;
            }
        }

        private void TabControlEditors_Drop(object sender, DragEventArgs e)
        {
            if (e.AllowedEffects.HasFlag(DragDropEffects.Move) &&
                e.Data.GetDataPresent("ScriptTab") &&
                ViewModel.Dragging.tab != null)
            {
                ViewModel.DropScriptTab(e.OriginalSource);
                e.Effects = DragDropEffects.Move;
            }
        }

        private void TabControlEditors_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            ViewModel.MoveDragDropWindow();
        }

        private void MetroTabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.DragStartPoint = e.GetPosition(null);
            ViewModel.RelativeDragStartPoint = e.GetPosition(sender as MetroTabItem);
        }

        private void MetroTabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !ViewModel.IsDragging)
            {
                Point position = e.GetPosition(null);
                MetroTabItem tabItem = sender as MetroTabItem;
                if (!((ScriptTab)tabItem.Tag).IsAddTabButton && ViewModel.Tabs.Collection.Count > 2)
                {
                    ViewModel.TriggerDragDrop(position, tabControlEditors, tabItem);
                }
            }
        }
    }
}
