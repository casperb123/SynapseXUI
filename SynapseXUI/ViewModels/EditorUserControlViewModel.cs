using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SynapseXUI.ViewModels
{
    public class EditorUserControlViewModel : INotifyPropertyChanged
    {
        public static EditorUserControlViewModel Instance { get; private set; }
        public static RoutedCommand CloseTabCommand = new RoutedCommand();

        private readonly EditorUserControl userControl;
        private string editorText;
        private ScriptTabs tabs;
        private ScriptTab selectedTab;
        private ObservableCollection<ScriptFile> scriptFiles;
        private ScriptFile selectedScriptFile;

        public ScriptFile SelectedScriptFile
        {
            get => selectedScriptFile;
            set
            {
                selectedScriptFile = value;
                OnPropertyChanged(nameof(SelectedScriptFile));
            }
        }

        public ObservableCollection<ScriptFile> ScriptFiles
        {
            get => scriptFiles;
            set
            {
                scriptFiles = value;
                OnPropertyChanged(nameof(ScriptFiles));
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
            this.userControl = userControl;
            Tabs = new ScriptTabs();
            Tabs.Collection.CollectionChanged += Tabs_CollectionChanged;
            ScriptFiles = new ObservableCollection<ScriptFile>();
            GetScripts();

            PackIconMaterialDesign icon = new PackIconMaterialDesign
            {
                Kind = PackIconMaterialDesignKind.Add,
                Width = 10,
                Height = 10,
                VerticalAlignment = VerticalAlignment.Center,
            };
            icon.SetResourceReference(Control.ForegroundProperty, "MahApps.Brushes.ThemeForeground");

            ScriptTab scriptTab = new ScriptTab
            {
                Header = icon,
                IsAddTabButton = true
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
                    userControl.tabControlEditors.GetSelectedTabItem().PreviewMouseDown += EditorsAddTab_PreviewMouseDown;
                    userControl.tabControlEditors.SelectionChanged += TabControlEditors_SelectionChanged;

                    if (App.Settings.SaveTabs && File.Exists(App.TabsFilePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(App.TabsFilePath)))
                    {
                        GetTabs();
                    }
                    else
                    {
                        AddTab(false);
                    }

                    timer.Stop();
                }
            };
            timer.Start();

            FileSystemWatcher watcher = new FileSystemWatcher(App.ScriptsFolderPath, "*.*");
            watcher.Created += Watcher_Changed;
            watcher.Changed += Watcher_Changed;
            watcher.Deleted += Watcher_Changed;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            userControl.Dispatcher.Invoke(() =>
            {
                GetScripts();
            });
        }

        private void TabControlEditors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedTab != null && SelectedTab.IsAddTabButton)
            {
                userControl.tabControlEditors.SelectedIndex = Tabs.Collection.Count - 2;
            }
        }

        private void EditorsAddTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            AddTab(true);
        }

        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Tabs.Collection.ToList().ForEach(x => x.EnableCloseButton = Tabs.Collection.Count > 2 && !x.IsAddTabButton);
        }

        public void GetScripts()
        {
            ScriptFiles.Clear();
            Directory.GetFiles(App.ScriptsFolderPath).ToList().ForEach(x => ScriptFiles.Add(new ScriptFile(x)));
        }

        private void GetTabs()
        {
            using (StreamReader reader = new StreamReader(App.TabsFilePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                ScriptTabs scriptTabs = formatter.Deserialize(reader.BaseStream) as ScriptTabs;
                List<ScriptTab> tabs = scriptTabs.Collection.Where(x => string.IsNullOrEmpty(x.FullFilename) || File.Exists(x.FullFilename)).ToList();

                if (tabs.Count == 0)
                {
                    AddTab(false);
                }
                else
                {
                    tabs.ForEach(x => AddTab(false, x.FullFilename, x.Text));
                    userControl.tabControlEditors.SelectedIndex = scriptTabs.SelectedIndex;
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
                        SelectedIndex = userControl.tabControlEditors.SelectedIndex
                    };

                    formatter.Serialize(writer.BaseStream, scriptTabs);
                }
            }
        }

        public void ExecuteScript(string script)
        {
            if (!string.IsNullOrWhiteSpace(script))
            {
                App.Lib.Execute(script);
            }
        }

        public void ExecuteFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Execute File",
                Filter = "Script Files | *.lua;*.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                ExecuteScript(File.ReadAllText(dialog.FileName));
            }
        }

        public ScriptTab AddTab(bool saveTabs, string filePath = null, string text = null)
        {
            ChromiumWebBrowser browser = new ChromiumWebBrowser(App.EditorFilePath);
            browser.JavascriptObjectRepository.Register("synServiceAsync", this, true);

            string theme = App.Settings.Theming.ApplicationTheme;
            ScriptTab scriptTab = new ScriptTab(browser, filePath);
            Tabs.Collection.Insert(Tabs.Collection.Count - 1, scriptTab);
            SelectedTab = scriptTab;

            browser.FrameLoadEnd += (s, e) =>
            {
                if (e.Frame.IsMain)
                {
                    scriptTab.EditorReady = true;
                }
            };

            SetEditorTheme(scriptTab, theme);

            if (string.IsNullOrEmpty(text))
            {
                if (saveTabs)
                {
                    userControl.Dispatcher.Invoke(() =>
                    {
                        SaveTabs();
                    });
                }
            }
            else
            {
                scriptTab.Text = text;
                SetEditorText(scriptTab, text, saveTabs);
            }

            return scriptTab;
        }

        public void CloseTab(MetroTabItem tab, bool saveTabs)
        {
            ScriptTab scriptTab = Tabs.Collection.FirstOrDefault(x => x.Editor == tab.Content);
            IBrowserHost browserHost = scriptTab.Editor.GetBrowserHost();

            browserHost.CloseBrowser(true);
            browserHost.CloseDevTools();
            Tabs.Collection.Remove(scriptTab);

            if (saveTabs)
            {
                SaveTabs();
            }
        }

        public void CloseTab(ScriptTab scriptTab, bool saveTabs)
        {
            IBrowserHost browserHost = scriptTab.Editor.GetBrowserHost();

            browserHost.CloseBrowser(true);
            browserHost.CloseDevTools();
            Tabs.Collection.Remove(scriptTab);

            if (saveTabs)
            {
                SaveTabs();
            }
        }

        public void SetEditorTheme(ScriptTab scriptTab, string theme)
        {
            Task.Run(() =>
            {
                while (!scriptTab.EditorReady) { }
                userControl.Dispatcher.Invoke(() =>
                {
                    if (theme == "Dark")
                    {
                        scriptTab.Editor.ExecuteScriptAsync("SetTheme", "tomorrow_night_eighties");
                    }
                    else
                    {
                        scriptTab.Editor.ExecuteScriptAsync("SetTheme", "chrome");
                    }
                });
            });
        }

        public void SetAllEditorThemes(string theme)
        {
            Tabs.Collection.Where(x => !x.IsAddTabButton).ToList().ForEach(x => SetEditorTheme(x, theme));
        }

        public void SetEditorText(ScriptTab scriptTab, string text, bool saveTabs)
        {
            Task.Run(() =>
            {
                while (!scriptTab.EditorReady) { }
                userControl.Dispatcher.Invoke(() =>
                {
                    scriptTab.Editor.ExecuteScriptAsync("SetText", text);
                    if (saveTabs)
                    {
                        SaveTabs();
                    }
                });
            });
        }

        public async void ClearEditorText()
        {
            if (App.SxOptions.ClearConfirmation)
            {
                MessageDialogResult result = await MainWindow.Instance.ShowMessageAsync("Clear Editor", "Are you sure that you want to clear the editor?", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }

            SelectedTab.Text = string.Empty;
            SelectedTab.Editor.ExecuteScriptAsync("ClearText()");
        }

        public void SaveFileAs(string text)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save script",
                Filter = "Script Files|*.lua;*.txt",
                InitialDirectory = App.ScriptsFolderPath
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, text);
                SelectedTab.FullFilename = dialog.FileName;
            }
        }

        public void SaveFile(ScriptTab scriptTab)
        {
            File.WriteAllText(scriptTab.FullFilename, scriptTab.Text);
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
                    scriptTab = AddTab(false, scriptFilePath);
                }
                else
                {
                    SelectedTab.FullFilename = scriptFilePath;
                    scriptTab = SelectedTab;
                }

                string script = File.ReadAllText(scriptFilePath);

                scriptTab.Text = script;
                SetEditorText(scriptTab, script, true);
            }
        }

        public void CloseAllTabs(ScriptTab tabToExclude = null)
        {
            if (tabToExclude is null)
            {
                Tabs.Collection.Where(x => !x.IsAddTabButton).ToList().ForEach(x => CloseTab(x, false));
                AddTab(false);
            }
            else
            {
                Tabs.Collection.Where(x => x != tabToExclude && !x.IsAddTabButton).ToList().ForEach(x => CloseTab(x, false));
            }

            SaveTabs();
        }

        #region CEF Sharp Methods
        public void TextChanged(string text)
        {
            SelectedTab.Text = text;
        }
        #endregion
    }
}
