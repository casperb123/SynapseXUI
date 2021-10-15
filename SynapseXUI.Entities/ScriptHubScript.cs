using sxlib.Specialized;
using System.ComponentModel;

namespace SynapseXUI.Entities
{
    public class ScriptHubScript : INotifyPropertyChanged
    {
        private string name;
        private string picture;
        private string description;

        public SxLibBase.SynHubEntry Script { get; private set; }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Picture
        {
            get => picture;
            set
            {
                picture = value;
                OnPropertyChanged(nameof(Picture));
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public ScriptHubScript(SxLibBase.SynHubEntry script)
        {
            Name = script.Name;
            Picture = script.Picture;
            Description = script.Description;
            Script = script;
        }
    }
}
