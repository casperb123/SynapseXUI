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
        private string text;
        [NonSerialized]
        private bool textChanged;

        [field: NonSerialized]
        public bool IsInitialized { get; set; }
        [field: NonSerialized]
        public bool Syncronize { get; set; }

        public bool TextChanged
        {
            get => textChanged;
            set
            {
                textChanged = value;
                OnPropertyChanged(nameof(TextChanged));
            }
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged(nameof(Text));
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
                if (Syncronize)
                {
                    Header = string.IsNullOrEmpty(value) ? "Untitled" : Path.GetFileName(value);
                }
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

        public ScriptTab(object header, string filePath, bool syncronize)
        {
            Header = header;
            Syncronize = syncronize;

            if (!string.IsNullOrEmpty(filePath))
            {
                FullFilename = filePath;
            }
        }
    }
}
