using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
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
            ViewModel.ExecuteScript(ViewModel.SelectedScript.Text);
        }

        private void MenuItemScriptsLoadCurrentTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(false, ViewModel.SelectedScript.FullName);
        }

        private void MenuItemScriptsLoadNewTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenFile(true, ViewModel.SelectedScript.FullName);
        }

        private void MenuItemScriptsRename_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RenameFile(ViewModel.SelectedScript);
        }

        private void MenuItemScriptsDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteFile();
        }

        private void MenuItemRenameTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RenameTab();
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

        private void ListBoxScripts_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right && !(e.OriginalSource is TextBlock))
            {
                e.Handled = true;
            }
        }

        private void TreeViewScripts_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ViewModel.SelectedScript = e.NewValue as Script;
        }

        private void TreeViewItemScripts_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Script script = treeViewItem.Header as Script;

            if (!script.IsFolder)
            {
                treeViewItem.Focus();

                if (e.ChangedButton == MouseButton.Right && (e.OriginalSource is Grid || e.OriginalSource is Path))
                {
                    contextMenuScripts.IsOpen = true;
                }
            }
        }

        private void TreeViewItemScripts_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            Script script = treeViewItem.Header as Script;

            if (script.IsFolder && e.ChangedButton == MouseButton.Right)
            {
                e.Handled = true;
            }
        }

        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (ViewModel.TreeViewItemSuppressBringIntoView)
            {
                return;
            }

            e.Handled = true;
            ViewModel.TreeViewItemSuppressBringIntoView = true;
            TreeViewItem treeViewItem = sender as TreeViewItem;

            if (treeViewItem != null)
            {
                Rect rect = new Rect(-1000, 0, treeViewItem.ActualWidth + 1000, treeViewItem.ActualHeight);
                treeViewItem.BringIntoView(rect);
            }

            ViewModel.TreeViewItemSuppressBringIntoView = false;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            ((TreeViewItem)sender).BringIntoView();
            e.Handled = true;
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

        private void TabControlEditors_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.RightClickIsTabItem = false;
        }

        private void MetroTabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MetroTabItem tabItem = sender as MetroTabItem;
            ScriptTab scriptTab = tabItem.Tag as ScriptTab;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (scriptTab.IsAddTabButton)
                {
                    ViewModel.DragStartPoint = new Point(-1, -1);
                    ViewModel.RelativeDragStartPoint = new Point(-1, -1);
                    ViewModel.AddTab(true);
                }
                else if (e.OriginalSource is Grid)
                {
                    ViewModel.DragStartPoint = new Point(-1, -1);
                    ViewModel.RelativeDragStartPoint = new Point(-1, -1);
                }
                else
                {
                    ViewModel.DragStartPoint = e.GetPosition(null);
                    ViewModel.RelativeDragStartPoint = e.GetPosition(tabItem);
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed &&
                     !scriptTab.IsAddTabButton)
            {
                ViewModel.RightClickIsTabItem = true;
                ViewModel.SelectedTab = tabItem.Tag as ScriptTab;
            }
        }

        private void MetroTabItem_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MetroTabItem tabItem = sender as MetroTabItem;
            ScriptTab scriptTab = tabItem.Tag as ScriptTab;

            if (e.ChangedButton == MouseButton.Right && scriptTab.IsAddTabButton)
            {
                e.Handled = true;
            }
        }

        private void MetroTabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed &&
                !ViewModel.IsDragging &&
                ViewModel.DragStartPoint.X != -1 &&
                ViewModel.DragStartPoint.Y != -1 &&
                ViewModel.RelativeDragStartPoint.X != -1 &&
                ViewModel.RelativeDragStartPoint.Y != -1)
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
