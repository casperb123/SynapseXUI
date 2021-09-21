using CefSharp;
using CefSharp.Wpf;
using MahApps.Metro.IconPacks;
using Microsoft.Win32;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SynapseXUI.ViewModels
{
    public class EditorUserControlViewModel : INotifyPropertyChanged
    {
        private readonly EditorUserControl userControl;
        private string editorText;
        private ObservableCollection<ScriptTab> tabs;
        private ScriptTab selectedTab;

        public static EditorUserControlViewModel Instance { get; private set; }

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
            AddTab();
        }

        private void EditorUserControlViewModel_MouseDown(object sender, MouseButtonEventArgs e)
        {
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

        public void AddTab(string filePath = null)
        {
            ChromiumWebBrowser browser = new ChromiumWebBrowser(Path.Combine(App.StartUpPath, @"bin\Editor.html"));
            browser.JavascriptObjectRepository.Register("synServiceAsync", this, true);
            ScriptTab scriptTab = new ScriptTab(browser, filePath);

            Tabs.Insert(Tabs.Count - 1, scriptTab);
            SelectedTab = scriptTab;
        }

        public void CloseTab(ScriptTab tab)
        {
            Tabs.Remove(tab);
        }

        private void SetEditorText(string text)
        {
            SelectedTab.Editor.ExecuteScriptAsync("SetText", text);
            EditorText = text;
        }

        private void ClearEditorText()
        {
            SelectedTab.Editor.ExecuteScriptAsync("ClearText");
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
            }
        }

        public void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Open script",
                Filter = "Script Files | *.txt;*.lua"
            };

            if (dialog.ShowDialog() == true)
            {
                SetEditorText(File.ReadAllText(dialog.FileName));
            }
        }

        #region CEF Sharp Methods
        private void TextChanged(string text)
        {
            EditorText = text;
        }

        private void SaveRequest(string text)
        {
            SaveFile(text);
        }

        private void OpenRequest()
        {
            OpenFile();
        }
        #endregion
    }
}
