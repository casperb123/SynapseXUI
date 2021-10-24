using System.ComponentModel;
using System.IO;

namespace SynapseXUI.Entities
{
    public class ScriptFile : INotifyPropertyChanged
    {
        private string fullFilename;
        private string filename;

        public string Script { get; set; }

        public string Filename
        {
            get => filename;
            set
            {
                filename = value;
                OnPropertyChanged(nameof(Filename));
            }
        }

        public string FullFilename
        {
            get => fullFilename;
            set
            {
                fullFilename = value;
                Filename = Path.GetFileName(value);
            }
        }

        public ScriptFile(string filePath)
        {
            FullFilename = filePath;
            Script = File.ReadAllText(filePath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
