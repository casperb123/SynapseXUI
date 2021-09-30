using CefSharp.Wpf;
using System;
using System.ComponentModel;
using System.IO;

namespace SynapseXUI.Entities
{
    [Serializable]
    public class ScriptTab : INotifyPropertyChanged
    {
        private string fullFilename;
        private object header;
        private bool enableCloseButton;
        [NonSerialized]
        private ChromiumWebBrowser editor;
        private string text;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }

        public bool EditorReady { get; set; }

        public ChromiumWebBrowser Editor
        {
            get => editor;
            set
            {
                editor = value;
                OnPropertyChanged(nameof(Editor));
            }
        }

        public bool IsAddTabButton { get; set; }

        public bool EnableCloseButton
        {
            get => enableCloseButton;
            set
            {
                enableCloseButton = value;
                OnPropertyChanged(nameof(EnableCloseButton));
            }
        }

        public object Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public string FullFilename
        {
            get => fullFilename;
            set
            {
                fullFilename = value;
                OnPropertyChanged(nameof(FullFilename));
                Header = string.IsNullOrEmpty(value) ? "Untitled" : Path.GetFileName(value);
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public ScriptTab(ChromiumWebBrowser browser = null, string filePath = null)
        {
            Editor = browser;

            if (filePath is null)
            {
                Header = "Untitled";
            }
            else
            {
                FullFilename = filePath;
            }
        }
    }
}
