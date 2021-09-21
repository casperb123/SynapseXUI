using CefSharp.Wpf;
using System.ComponentModel;
using System.IO;

namespace SynapseXUI.Entities
{
    public class ScriptTab : INotifyPropertyChanged
    {
        private string fullFilename;
        private object header;
        private bool enableCloseButton;
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
                Header = Path.GetFileName(value);
            }
        }

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
