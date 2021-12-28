using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace SynapseXUI.Entities
{
    public class Script : INotifyPropertyChanged
    {
        private string fullName;
        private string name;
        private ObservableCollection<Script> children;

        public string Text { get; set; }
        public Script Parent { get; set; }
        public bool IsFolder { get; set; }

        public ObservableCollection<Script> Children
        {
            get => children;
            set
            {
                children = value;
                OnPropertyChanged(nameof(Children));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string FullName
        {
            get => fullName;
            set
            {
                fullName = value;
                Name = Path.GetFileName(value);
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

        public Script(string path, bool isFolder)
        {
            FullName = path;
            Name = Path.GetFileName(path);
            IsFolder = isFolder;

            if (isFolder)
            {
                Children = new ObservableCollection<Script>();
            }
            else
            {
                Text = File.ReadAllText(path);
            }
        }

        public Script(string path, bool isFolder, Script parent) : this(path, isFolder)
        {
            Parent = parent;
        }
    }
}
