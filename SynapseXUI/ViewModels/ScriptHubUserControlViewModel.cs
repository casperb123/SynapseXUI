using MahApps.Metro.Controls;
using sxlib.Specialized;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace SynapseXUI.ViewModels
{
    public class ScriptHubUserControlViewModel : INotifyPropertyChanged
    {
        private ScriptHubUserControl userControl;
        private ObservableCollection<ScriptHubScript> scripts;
        private ScriptHubScript selectedScript;
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

        public ScriptHubScript SelectedScript
        {
            get => selectedScript;
            set
            {
                selectedScript = value;
                OnPropertyChanged(nameof(SelectedScript));
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

        public ScriptHubUserControlViewModel(ScriptHubUserControl userControl)
        {
            this.userControl = userControl;
            Scripts = new ObservableCollection<ScriptHubScript>();
        }

        public void OpenScript(Tile tile)
        {
            SelectedScript = Scripts.FirstOrDefault(x => x.Name == tile.Title);
            userControl.flyoutScript.IsOpen = true;
        }

        public void ExecuteScript()
        {
            SelectedScript.Script.Execute();
        }
    }
}
