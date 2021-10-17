﻿using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private readonly EditorUserControl userControl;
        private bool editorReady;
        private bool tabsLoaded;
        private string editorText;
        private ScriptTabs tabs;
        private ScriptTab selectedTab;
        private int selectedTabIndex;
        private ObservableCollection<ScriptFile> scriptFiles;
        private ScriptFile selectedScriptFile;
        private bool detectScriptTabChange;
        private ChromiumWebBrowser editor;

        public ChromiumWebBrowser Editor
        {
            get => editor;
            set
            {
                editor = value;
                OnPropertyChanged(nameof(Editor));
            }
        }

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
            this.userControl = userControl;
            Editor = new ChromiumWebBrowser(App.EditorFilePath);
            Editor.JavascriptObjectRepository.Register("synServiceAsync", this, true);
            Editor.FrameLoadEnd += (s, e) =>
            {
                if (e.Frame.IsMain)
                {
                    editorReady = true;
                    FocusEditor();
                }
            };

            detectScriptTabChange = true;
            Tabs = new ScriptTabs();
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
                    userControl.tabControlEditors.GetSelectedTabItem().PreviewMouseDown += EditorsAddTab_PreviewMouseDown;

                    if (App.Settings.SaveTabs && File.Exists(App.TabsFilePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(App.TabsFilePath)))
                    {
                        GetTabs();
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
            userControl.Dispatcher.Invoke(() =>
            {
                GetScripts();
                RefreshTabs();
            });
        }

        private void EditorsAddTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            AddTab(true);
        }

        public void ChangeTab()
        {
            if (!detectScriptTabChange || SelectedTab is null || !tabsLoaded)
            {
                return;
            }

            if (SelectedTab.IsAddTabButton)
            {
                SelectedTabIndex = Tabs.Collection.Count - 2;
            }

            SetEditorText(SelectedTab.Text, false);
            FocusEditor();
        }

        public void GetScripts()
        {
            ScriptFiles.Clear();
            Directory.GetFiles(App.ScriptsFolderPath).ToList().ForEach(x => ScriptFiles.Add(new ScriptFile(x)));
        }

        private void RefreshTabs()
        {
            bool tabsChanged = false;

            foreach (ScriptTab tab in Tabs.Collection.Where(x => !x.IsAddTabButton))
            {
                if (!string.IsNullOrEmpty(tab.FullFilename) && !File.Exists(tab.FullFilename))
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

        private void GetTabs()
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
                            AddTab(false, tab.FullFilename, tab.Text);
                        }
                        else
                        {
                            AddTab(false, text: tab.Text);
                        }
                    }

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
            string theme = App.Settings.Theme.ApplicationTheme;
            ScriptTab scriptTab = new ScriptTab(filePath)
            {
                Text = text,
                EnableCloseButton = true
            };
            Tabs.Collection.Insert(Tabs.Collection.Count - 1, scriptTab);
            SelectedTab = scriptTab;

            if (saveTabs && string.IsNullOrEmpty(text))
            {
                userControl.Dispatcher.Invoke(() =>
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
                !App.ShowPrompt("Close Tab", "Are you sure that you want to close this tab? All changes will be lost!", PromptType.YesNo))
            {
                return;
            }

            Tabs.Collection.Remove(scriptTab);

            if (Tabs.Collection.Count == 1)
            {
                AddTab(false);
                //FocusEditor(true);
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
                    userControl.Dispatcher.Invoke(() =>
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
                userControl.Dispatcher.Invoke(() =>
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
                userControl.Dispatcher.Invoke(() =>
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
            if (App.SxOptions.ClearConfirmation && !App.ShowPrompt("Clear Editor", "Are you sure that you want to clear the editor?", PromptType.YesNo))
            {
                return;
            }

            SelectedTab.Text = string.Empty;
            Editor.ExecuteScriptAsync("ClearText()");
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
                SetEditorText(script, true);
                FocusEditor();
            }
        }

        public void DeleteFile()
        {
            if (App.Settings.DeleteFileConfirmation && !App.ShowPrompt("Delete File", "Are you sure that you want to delete this file? This can't be undone!", PromptType.YesNo))
            {
                return;
            }

            if (selectedScriptFile != null)
            {
                File.Delete(SelectedScriptFile.FullFilename);
            }
        }

        public void CloseAllTabs(ScriptTab tabToExclude = null)
        {
            if (App.SxOptions.CloseConfirmation)
            {
                if ((tabToExclude is null &&
                    !App.ShowPrompt("Close All Tabs", "Are you sure that you want to close all tabs?", PromptType.YesNo))
                    ||
                    (tabToExclude != null &&
                    !App.ShowPrompt("Close All Tabs", "Are you sure that you want to close all but selected tabs?", PromptType.YesNo)))
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

        #region CEF Sharp Methods
        public void TextChanged(string text)
        {
            SelectedTab.Text = text;
        }
        #endregion
    }
}
