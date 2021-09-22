﻿using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        private string editorText;
        private ObservableCollection<ScriptTab> tabs;
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

        public ObservableCollection<ScriptTab> Tabs
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

        public EditorUserControlViewModel(EditorUserControl userControl)
        {
            Instance = this;
            this.userControl = userControl;
            Tabs = new ObservableCollection<ScriptTab>();
            Tabs.CollectionChanged += Tabs_CollectionChanged;
            ScriptFiles = new ObservableCollection<ScriptFile>();

            Directory.GetFiles(Path.Combine(App.StartupPath, "scripts")).ToList().ForEach(x => ScriptFiles.Add(new ScriptFile(x)));

            PackIconMaterialDesign icon = new PackIconMaterialDesign
            {
                Kind = PackIconMaterialDesignKind.Add,
                Width = 10,
                Height = 10,
                VerticalAlignment = VerticalAlignment.Center
            };
            ScriptTab scriptTab = new ScriptTab
            {
                Header = icon,
                IsAddTabButton = true
            };
            Tabs.Add(scriptTab);

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

                    AddTab();
                    timer.Stop();
                }
            };
            timer.Start();
        }

        private void TabControlEditors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedTab != null && SelectedTab.IsAddTabButton)
            {
                userControl.tabControlEditors.SelectedIndex = Tabs.Count - 2;
            }
        }

        private void EditorsAddTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            AddTab();
        }

        private void Tabs_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Tabs.ToList().ForEach(x => x.EnableCloseButton = Tabs.Count > 2 && !x.IsAddTabButton);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
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

        public void AddTab(string filePath = null)
        {
            ChromiumWebBrowser browser = new ChromiumWebBrowser(Path.Combine(App.StartupPath, @"bin\Editor.html"));
            browser.JavascriptObjectRepository.Register("synServiceAsync", this, true);

            ScriptTab scriptTab = new ScriptTab(browser, filePath);
            Tabs.Insert(Tabs.Count - 1, scriptTab);
            SelectedTab = scriptTab;
        }

        public void CloseTab(ScriptTab tab)
        {
            Tabs.Remove(tab);
        }

        public void SetEditorText(string text)
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            timer.Tick += async (s, e) =>
            {
                JavascriptResponse response = await SelectedTab.Editor.GetMainFrame().EvaluateScriptAsync("SetText('')");
                if (response.Success)
                {
                    timer.Stop();
                    SelectedTab.Editor.ExecuteScriptAsync("SetText", text);
                    SelectedTab.Text = text;
                }
            };
            timer.Start();
        }

        public void ClearEditorText()
        {
            SelectedTab.Editor.ExecuteScriptAsync("ClearText()");
        }

        public void SaveFile(string text)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Save script",
                Filter = "Script Files | *.txt;*.lua"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, text);
                SelectedTab.FullFilename = dialog.FileName;
            }
        }

        public void OpenFile(bool newTab, string filePath = null)
        {
            string scriptFilePath = filePath;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Open script",
                    Filter = "Script Files | *.txt;*.lua"
                };

                if (dialog.ShowDialog() == true)
                {
                    scriptFilePath = dialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(scriptFilePath))
            {
                if (newTab)
                {
                    AddTab(scriptFilePath);
                }
                else
                {
                    SelectedTab.FullFilename = scriptFilePath;
                }

                SetEditorText(File.ReadAllText(scriptFilePath));
            }
        }

        public void CloseAllTabs(ScriptTab tabToExclude = null)
        {
            if (tabToExclude is null)
            {
                Tabs.Where(x => !x.IsAddTabButton).ToList().ForEach(x => Tabs.Remove(x));
                AddTab();
            }
            else
            {
                Tabs.Where(x => x != tabToExclude && !x.IsAddTabButton).ToList().ForEach(x => Tabs.Remove(x));
            }
        }

        #region CEF Sharp Methods
        public void TextChanged(string text)
        {
            SelectedTab.Text = text;
        }
        #endregion
    }
}
