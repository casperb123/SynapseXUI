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

        public ChromiumWebBrowser Editor { get; set; }

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

        public string Text { get; set; }

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

        private void OnPropertyChanged(string prop)
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
