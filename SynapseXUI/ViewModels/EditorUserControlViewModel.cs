using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using SynapseXUI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SynapseXUI.ViewModels
{
    public class EditorUserControlViewModel : INotifyPropertyChanged
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point
        {
            public int X;
            public int Y;
        }

        public static EditorUserControlViewModel Instance { get; private set; }

        private bool editorReady;
        private bool tabsLoaded;
        private string editorText;
        private ScriptTabs tabs;
        private ScriptTab selectedTab;
        private int selectedTabIndex;
        private ObservableCollection<Script> scripts;
        private Script selectedScript;
        private bool detectScriptTabChange;
        private ChromiumWebBrowser editor;
        private DragDropWindow dragDropWindow;
        private bool rightClickIsTabItem;

        public readonly EditorUserControl UserControl;
        public delegate Point GetPosition(IInputElement element);
        public bool IsDragging { get; private set; }
        public (int index, ScriptTab tab) Dragging { get; private set; }
        public Point DragStartPoint { get; set; }
        public Point RelativeDragStartPoint { get; set; }

        public bool RightClickIsTabItem
        {
            get => rightClickIsTabItem;
            set
            {
                rightClickIsTabItem = value;
                OnPropertyChanged(nameof(RightClickIsTabItem));
            }
        }

        public ChromiumWebBrowser Editor
        {
            get => editor;
            set
            {
                editor = value;
                OnPropertyChanged(nameof(Editor));
            }
        }

        public Script SelectedScript
        {
            get => selectedScript;
            set
            {
                selectedScript = value;
                OnPropertyChanged(nameof(SelectedScript));
            }
        }

        public ObservableCollection<Script> Scripts
        {
            get => scripts;
            set
            {
                scripts = value;
                OnPropertyChanged(nameof(Scripts));
            }
        }

        public int SelectedTabIndex
        {
            get => selectedTabIndex;
            set
            {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        public ScriptTab SelectedTab
        {
            get => selectedTab;
            set
            {
                selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
            }
        }

        public ScriptTabs Tabs
        {
            get => tabs;
            set
            {
                tabs = value;
                OnPropertyChanged(nameof(Tabs));
            }
        }

        public string EditorText
        {
            get => editorText;
            set
            {
                editorText = value;
                OnPropertyChanged(nameof(EditorText));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public EditorUserControlViewModel(EditorUserControl userControl)
        {
            Instance = this;
            userControl.columnScripts.Width = App.Settings.ScriptsListWidth;
            UserControl = userControl;
            Editor = new ChromiumWebBrowser(App.EditorFilePath);
            Editor.JavascriptObjectRepository.Register("synServiceAsync", this, true);
            Editor.FrameLoadEnd += (s, e) =>
            {
                if (e.Frame.IsMain)
                {
                    editorReady = true;
                    FocusEditor();
                    SetEditorTheme(App.Settings.Theme.ApplicationTheme);
                }
            };

            detectScriptTabChange = true;
            Tabs = new ScriptTabs();
            Scripts = new ObservableCollection<Script>();
            GetScripts();

            PackIconMaterialDesign icon = new PackIconMaterialDesign
            {
                Kind = PackIconMaterialDesignKind.Add,
                Width = 10,
                Height = 10,
                VerticalAlignment = VerticalAlignment.Center,
            };
            icon.SetResourceReference(Control.ForegroundProperty, "MahApps.Brushes.ThemeForeground");

            ScriptTab scriptTab = new ScriptTab(icon, null)
            {
                Header = icon,
                IsAddTabButton = true,
                EnableCloseButton = false
            };
            Tabs.Collection.Add(scriptTab);

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timer.Tick += (s, e) =>
            {
                if (userControl.tabControlEditors.Items.Count == 1)
                {
                    userControl.tabControlEditors.SelectedIndex = 0;

                    if (App.Settings.SaveTabs && File.Exists(App.TabsFilePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(App.TabsFilePath)))
                    {
                        LoadTabs();
                    }
                    else
                    {
                        AddTab(false);
                    }

                    timer.Stop();
                    tabsLoaded = true;
                    SetEditorText(SelectedTab.Text, false);
                }
            };
            timer.Start();

            FileSystemWatcher watcher = new FileSystemWatcher(App.ScriptsFolderPath, "*.*");
            watcher.Created += Watcher_Changed;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.Renamed += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            UserControl.Dispatcher.Invoke(() =>
            {
                GetScripts();
                RefreshTabs();
            });
        }

        public void ChangeTab()
        {
            if (!detectScriptTabChange || SelectedTab is null || !tabsLoaded)
            {
                return;
            }

            if (IsDragging)
            {
                SelectedTab = null;
            }
            else
            {
                int tabsCount = Tabs.Collection.Count;
                if (SelectedTab.IsAddTabButton && tabsCount > 1)
                {
                    SelectedTabIndex = tabsCount - 2;
                }

                SetEditorText(SelectedTab.Text, false);
                FocusEditor();
            }
        }

        public IEnumerable<Script> EnumerateScripts(Script parent)
        {
            if (parent.Children != null)
            {
                foreach (Script script in parent.Children)
                {
                    yield return script;

                    foreach (Script child in EnumerateScripts(script))
                    {
                        yield return child;
                    }
                }
            }
        }

        public void GetScripts()
        {
            Scripts.Clear();

            string[] directories = Directory.GetDirectories(App.ScriptsFolderPath, "*.*", SearchOption.AllDirectories);
            string[] files = Directory.GetFiles(App.ScriptsFolderPath, "*.*", SearchOption.AllDirectories);

            foreach (string directory in directories)
            {
                Script parentScript = null;
                List<Script> folders = Scripts.ToList();
                folders.ToList().ForEach(x => folders.AddRange(EnumerateScripts(x)));

                DirectoryInfo currentDir = new DirectoryInfo(directory).Parent;
                int depth = 1;

                while (currentDir.FullName != App.ScriptsFolderPath && parentScript is null)
                {
                    Script parent = folders.FirstOrDefault(x => x.FullName == currentDir.FullName);
                    if (parent != null)
                    {
                        parentScript = parent;
                        break;
                    }

                    currentDir = currentDir.Parent;
                    depth++;
                }

                if (currentDir.FullName == App.ScriptsFolderPath || parentScript is null)
                {
                    Scripts.Add(new Script(directory, true));
                }
                else
                {
                    parentScript.Children.Add(new Script(directory, true, parentScript));
                }
            }

            foreach (string file in files)
            {
                string directory = Path.GetDirectoryName(file);
                List<Script> folders = Scripts.Where(x => x.IsFolder).ToList();
                folders.ToList().ForEach(x => folders.AddRange(EnumerateScripts(x)));
                Script parent = folders.FirstOrDefault(x => x.FullName == directory);

                if (parent is null)
                {
                    Scripts.Add(new Script(file, false));
                }
                else
                {
                    parent.Children.Add(new Script(file, false, parent));
                }
            }
        }

        private void RefreshTabs()
        {
            bool tabsChanged = false;

            foreach (ScriptTab tab in Tabs.Collection.Where(x => !x.IsAddTabButton))
            {
                if (string.IsNullOrEmpty(tab.FullFilename) || (!string.IsNullOrEmpty(tab.FullFilename) && !File.Exists(tab.FullFilename)))
                {
                    tab.FullFilename = null;
                    tabsChanged = true;
                }
            }

            if (tabsChanged)
            {
                SaveTabs();
            }
        }

        private void LoadTabs()
        {
            using (StreamReader reader = new StreamReader(App.TabsFilePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                ScriptTabs scriptTabs = formatter.Deserialize(reader.BaseStream) as ScriptTabs;
                List<ScriptTab> tabs = scriptTabs.Collection.ToList();

                if (tabs.Count == 0)
                {
                    AddTab(false);
                }
                else
                {
                    foreach (ScriptTab tab in tabs)
                    {
                        if (!string.IsNullOrEmpty(tab.FullFilename) && File.Exists(tab.FullFilename))
                        {
                            AddTab(false, tab.Header.ToString(), tab.FullFilename, scrollToEnd: false);
                        }
                        else
                        {
                            AddTab(false, tab.Header.ToString(), text: tab.Text, scrollToEnd: false);
                        }
                    }

                    SelectedTabIndex = scriptTabs.SelectedIndex;
                    UserControl.tabControlEditors.GetSelectedTabItem().BringIntoView();
                }
            }
        }

        public void SaveTabs()
        {
            if (App.Settings.SaveTabs)
            {
                using (StreamWriter writer = new StreamWriter(App.TabsFilePath))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    ScriptTabs scriptTabs = new ScriptTabs
                    {
                        Collection = new ObservableCollection<ScriptTab>(Tabs.Collection.Where(x => !x.IsAddTabButton)),
                        SelectedIndex = UserControl.tabControlEditors.SelectedIndex
                    };

                    formatter.Serialize(writer.BaseStream, scriptTabs);
                }
            }
        }

        public void ExecuteScript(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                PromptWindow.Show("Execution error", "You can't execute an empty script", PromptType.OK);
            }
            else
            {
                App.Lib.Execute(script);
            }
        }

        public void ExecuteFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Execute File",
                Filter = "Script Files|*.lua;*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                ExecuteScript(File.ReadAllText(dialog.FileName));
            }
        }

        public ScriptTab AddTab(bool saveTabs, string header = "Untitled", string filePath = null, string text = null, bool scrollToEnd = true)
        {
            string theme = App.Settings.Theme.ApplicationTheme;
            ScriptTab scriptTab = new ScriptTab(header, filePath)
            {
                Text = string.IsNullOrEmpty(filePath) ? text : File.ReadAllText(filePath),
                EnableCloseButton = true
            };

            Tabs.Collection.Insert(Tabs.Collection.Count - 1, scriptTab);
            SelectedTab = scriptTab;

            if (scrollToEnd)
            {
                UserControl.tabControlEditors.GetSelectedTabItem().BringIntoView(new Rect(new Size(double.MaxValue, 0)));
            }

            if (saveTabs && string.IsNullOrEmpty(text))
            {
                UserControl.Dispatcher.Invoke(() =>
                {
                    SaveTabs();
                });
            }

            return scriptTab;
        }

        public void CloseTab(ScriptTab scriptTab, bool saveTabs, bool skipConfirmation = false)
        {
            if (!skipConfirmation &&
                App.SxOptions.CloseConfirmation &&
                !PromptWindow.Show("Close Tab", "Are you sure that you want to close this tab? All changes will be lost!", PromptType.YesNo))
            {
                return;
            }

            Tabs.Collection.Remove(scriptTab);

            if (Tabs.Collection.Count == 1)
            {
                AddTab(false);
            }

            if (saveTabs)
            {
                SaveTabs();
            }
        }

        public void FocusEditor(bool skipDetectCheck = false)
        {
            if ((skipDetectCheck || detectScriptTabChange) && SelectedTab != null)
            {
                Task.Run(() =>
                {
                    while (!editorReady) { }
                    UserControl.Dispatcher.Invoke(() =>
                    {
                        Editor.Focus();
                        Editor.ExecuteScriptAsync("editor.focus();");
                    });
                });
            }
        }

        public void SetEditorTheme(string theme)
        {
            Task.Run(() =>
            {
                while (!editorReady) { }
                UserControl.Dispatcher.Invoke(() =>
                {
                    if (theme == "Dark")
                    {
                        Editor.ExecuteScriptAsync("SetTheme", "tomorrow_night_eighties");
                    }
                    else
                    {
                        Editor.ExecuteScriptAsync("SetTheme", "chrome");
                    }
                });
            });
        }

        public void SetEditorText(string text, bool saveTabs)
        {
            Task.Run(() =>
            {
                while (!editorReady) { }
                UserControl.Dispatcher.Invoke(() =>
                {
                    Editor.ExecuteScriptAsync("SetText", text);
                    if (saveTabs)
                    {
                        SaveTabs();
                    }
                });
            });
        }

        public void ClearEditorText()
        {
            if (App.SxOptions.ClearConfirmation && !PromptWindow.Show("Clear Editor", "Are you sure that you want to clear the editor?", PromptType.YesNo))
            {
                return;
            }

            SelectedTab.Text = string.Empty;
            Editor.ExecuteScriptAsync("ClearText()");
            SelectedTab.TextChanged = true;
        }

        public void SaveScriptAs(ScriptTab scriptTab)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save script",
                Filter = "Script Files|*.lua;*.txt"
            };

            if (scriptTab.Header.ToString() != "Untitled")
            {
                dialog.FileName = scriptTab.Header.ToString();
            }

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, scriptTab.Text);
                SelectedTab.FullFilename = dialog.FileName;
                SelectedTab.TextChanged = false;
            }
        }

        public void SaveScript(ScriptTab scriptTab)
        {
            File.WriteAllText(scriptTab.FullFilename, scriptTab.Text);
            SelectedTab.TextChanged = false;
        }

        public void OpenFile(bool newTab, string filePath = null)
        {
            string scriptFilePath = filePath;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Open script",
                    Filter = "Script Files|*.lua;*.txt"
                };

                if (dialog.ShowDialog() == true)
                {
                    scriptFilePath = dialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(scriptFilePath))
            {
                ScriptTab scriptTab;

                if (newTab)
                {
                    scriptTab = AddTab(false, filePath: scriptFilePath);
                }
                else
                {
                    SelectedTab.FullFilename = scriptFilePath;
                    scriptTab = SelectedTab;
                }

                scriptTab.Header = Path.GetFileName(scriptTab.FullFilename);
                string script = File.ReadAllText(scriptFilePath);

                scriptTab.Text = script;
                SetEditorText(script, true);
                FocusEditor();
            }
        }

        public void RenameFile(Script file)
        {
            string path = Path.GetDirectoryName(file.FullName);
            string extension = Path.GetExtension(file.FullName);
            string fileName = Path.GetFileNameWithoutExtension(file.FullName);
            (bool result, object input) = InputWindow.Show("Rename File", $"Enter a new name for the file '{file.Name}'", fileName, InputDataType.Text);

            if (result)
            {
                string newFileName = Path.GetFileNameWithoutExtension(input.ToString());
                string filePath = Path.Combine(path, $"{newFileName}{extension}");

                if (!File.Exists(filePath))
                {
                    File.Move(file.FullName, filePath);
                }
                else
                {
                    PromptWindow.Show("Rename File", $"A file with the name '{newFileName}' already exists. Please write another name", PromptType.OK);
                    RenameFile(file);
                }
            }
        }

        public void DeleteFile()
        {
            if (SelectedScript != null &&
                PromptWindow.Show("Delete File", $"Are you sure that you want to delete the file '{SelectedScript.Name}'?", PromptType.YesNo))
            {
                File.Delete(SelectedScript.FullName);
            }
        }

        public void ReloadTab()
        {
            if (PromptWindow.Show("Reload Tab", $"Are you sure you want to reload the the tab '{SelectedTab.Header}'? All unsaved changes will be lost!", PromptType.YesNo))
            {
                SelectedTab.TextChanged = false;

                if (!string.IsNullOrWhiteSpace(SelectedTab.FullFilename) && File.Exists(SelectedTab.FullFilename))
                {
                    string text = File.ReadAllText(SelectedTab.FullFilename);
                    SelectedTab.Text = text;
                    SetEditorText(text, true);
                }
                else
                {
                    SelectedTab.FullFilename = null;
                }
            }
        }

        public void CloseAllTabs(ScriptTab tabToExclude = null)
        {
            if (App.SxOptions.CloseConfirmation)
            {
                if ((tabToExclude is null &&
                    !PromptWindow.Show("Close All Tabs", "Are you sure that you want to close all tabs?", PromptType.YesNo))
                    ||
                    (tabToExclude != null &&
                    !PromptWindow.Show("Close All Tabs", "Are you sure that you want to close all but selected tabs?", PromptType.YesNo)))
                {
                    return;
                }
            }

            detectScriptTabChange = false;
            List<ScriptTab> tabs = tabToExclude is null
                ? Tabs.Collection.Where(x => !x.IsAddTabButton).ToList()
                : Tabs.Collection.Where(x => x != tabToExclude && !x.IsAddTabButton).ToList();

            tabs.ForEach(x => CloseTab(x, false, true));
            detectScriptTabChange = true;
            SetEditorText(SelectedTab.Text, true);
        }

        public ScriptTab GetTargetScriptTab(object source)
        {
            var current = source as DependencyObject;

            while (current != null)
            {
                if (current is MetroTabItem tabItem)
                {
                    return tabItem.Tag as ScriptTab;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }

        private void CreateDragDropWindow(MetroTabItem tabItem)
        {
            dragDropWindow = new DragDropWindow(tabItem.Header.ToString(), tabItem.IsSelected);
            dragDropWindow.Show();
            MoveDragDropWindow();
        }

        public void MoveDragDropWindow()
        {
            Win32Point point = new Win32Point();
            GetCursorPos(ref point);

            dragDropWindow.Left = point.X - RelativeDragStartPoint.X;
            dragDropWindow.Top = point.Y - RelativeDragStartPoint.Y;
        }

        public void EndDragDrop(bool resetDrag = false)
        {
            if (IsDragging)
            {
                IsDragging = false;

                if (resetDrag && Dragging.tab != null)
                {
                    Tabs.Collection.Insert(Dragging.index, Dragging.tab);
                    SelectedTabIndex = Dragging.index;
                }

                dragDropWindow?.Close();
                dragDropWindow = null;
                Dragging = (-1, null);
            }
        }

        public void TriggerDragDrop(Point position, MetroAnimatedSingleRowTabControl tabControl, MetroTabItem tabItem)
        {
            if (DragStartPoint.X != -1 &&
                DragStartPoint.Y != -1 &&
                (Math.Abs(position.X - DragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - DragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ScriptTab scriptTab = tabItem.Tag as ScriptTab;
                DataObject data = new DataObject();
                data.SetData("ScriptTab", scriptTab);
                CreateDragDropWindow(tabItem);

                IsDragging = true;
                Dragging = (Tabs.Collection.IndexOf(scriptTab), scriptTab);
                Tabs.Collection.Remove(scriptTab);

                DragDrop.DoDragDrop(tabControl, data, DragDropEffects.Move);
            }
        }

        public void DropScriptTab(object source)
        {
            ScriptTab scriptTabTarget = GetTargetScriptTab(source);
            if (scriptTabTarget != null)
            {
                if (!scriptTabTarget.Equals(Dragging.tab))
                {
                    int targetIndex = Tabs.Collection.IndexOf(scriptTabTarget);

                    Tabs.Collection.Insert(targetIndex, Dragging.tab);
                    EndDragDrop();
                    SelectedTabIndex = targetIndex;
                }
            }
            else
            {
                int targetIndex = Tabs.Collection.Count - 1;

                Tabs.Collection.Insert(targetIndex, Dragging.tab);
                EndDragDrop();
                SelectedTabIndex = targetIndex;
            }
        }

        public void RenameTab()
        {
            string header = Path.GetFileNameWithoutExtension(SelectedTab.Header.ToString());
            (bool result, object input) = InputWindow.Show("Rename Tab", $"Enter a new name for the tab '{header}'", header, InputDataType.Text);

            if (result)
            {
                string tabName = Path.GetFileNameWithoutExtension(input.ToString());
                SelectedTab.Header = tabName;
            }
        }

        public TreeViewItem GetTreeViewItem(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        #region CEF Sharp Methods
        public void TextChanged(string text)
        {
            if (SelectedTab.IsInitialized && SelectedTab.Text != text)
            {
                SelectedTab.TextChanged = true;
            }
            SelectedTab.Text = text;
            SelectedTab.IsInitialized = true;
        }
        #endregion
    }
}
