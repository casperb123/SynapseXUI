using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SynapseXUI.ViewModels
{
    public class ScriptHubUserControlViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ScriptHubScript> scripts;
        private bool loaded;

        public bool Loaded
        {
            get => loaded;
            set
            {
                loaded = value;
                OnPropertyChanged(nameof(Loaded));
            }
        }

        public ObservableCollection<ScriptHubScript> Scripts
        {
            get => scripts;
            set
            {
                scripts = value;
                OnPropertyChanged(nameof(Scripts));
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

        public ScriptHubUserControlViewModel()
        {
            Scripts = new ObservableCollection<ScriptHubScript>();
        }

        public void OpenScript(Tile tile)
        {
            MainWindow.Instance.ViewModel.SelectedHubScript = Scripts.FirstOrDefault(x => x.Name == tile.Title);
            MainWindow.Instance.flyoutScript.IsOpen = true;
        }
    }
}
