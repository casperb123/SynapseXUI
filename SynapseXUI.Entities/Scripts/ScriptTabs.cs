using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SynapseXUI.Entities.Scripts
{
    [Serializable]
    public class ScriptTabs : INotifyPropertyChanged
    {
        private ObservableCollection<ScriptTab> collection;
        private int selectedIndex;

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        public ObservableCollection<ScriptTab> Collection
        {
            get => collection;
            set
            {
                collection = value;
                OnPropertyChanged(nameof(Collection));
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public ScriptTabs()
        {
            Collection = new ObservableCollection<ScriptTab>();
        }
    }
}
