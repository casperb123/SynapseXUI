using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace SynapseXUI.Entities
{
    public class Settings : INotifyPropertyChanged
    {
        private Theming theming;

        public Theming Theming
        {
            get { return theming; }
            set
            {
                theming = value;
                OnPropertyChanged(nameof(Theming));
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

        public Settings()
        {
            Theming = new Theming();
        }

        public void Save(string settingsFilePath)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }

        public static Settings GetSettings(string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath))
            {
                Settings settings = new Settings();
                settings.Save(settingsFilePath);
                return settings;
            }

            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsFilePath));
        }
    }

    public class Theming : INotifyPropertyChanged
    {
        private string applicationTheme;
        private string applicationColor;

        public string ApplicationColor
        {
            get => applicationColor;
            set
            {
                applicationColor = value;
                OnPropertyChanged(nameof(ApplicationColor));
            }
        }

        public string ApplicationTheme
        {
            get => applicationTheme;
            set
            {
                applicationTheme = value;
                OnPropertyChanged(nameof(ApplicationTheme));
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

        public Theming()
        {
            ApplicationTheme = "Dark";
            ApplicationColor = "Blue";
        }
    }
}
