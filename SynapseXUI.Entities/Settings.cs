using System.ComponentModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace SynapseXUI.Entities
{
    public class Settings : INotifyPropertyChanged
    {
        private Theme theme;
        private Size size;
        private bool deleteFileConfirmation;
        private bool saveTabs;

        public bool SaveTabs
        {
            get => saveTabs;
            set
            {
                saveTabs = value;
                OnPropertyChanged(nameof(SaveTabs));
            }
        }

        public bool DeleteFileConfirmation
        {
            get => deleteFileConfirmation;
            set
            {
                deleteFileConfirmation = value;
                OnPropertyChanged(nameof(DeleteFileConfirmation));
            }
        }

        public Size Size
        {
            get => size;
            set
            {
                size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        public Theme Theme
        {
            get { return theme; }
            set
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
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
            SaveTabs = true;
            DeleteFileConfirmation = true;
            Theme = new Theme();
            Size = new Size();
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

    public class Theme : INotifyPropertyChanged
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

        public Theme()
        {
            ApplicationTheme = "Dark";
            ApplicationColor = "Blue";
        }
    }

    public class Size : INotifyPropertyChanged
    {
        private double applicationWidth;
        private double applicationHeight;
        private WindowState applicationState;
        private bool saveApplicationSize;

        public bool SaveApplicationSize
        {
            get => saveApplicationSize;
            set
            {
                saveApplicationSize = value;
                OnPropertyChanged(nameof(SaveApplicationSize));
            }
        }

        public double ApplicationWidth
        {
            get => applicationWidth;
            set
            {
                applicationWidth = value;
                OnPropertyChanged(nameof(ApplicationWidth));
            }
        }

        public double ApplicationHeight
        {
            get => applicationHeight;
            set
            {
                applicationHeight = value;
                OnPropertyChanged(nameof(ApplicationHeight));
            }
        }

        public WindowState ApplicationState
        {
            get => applicationState;
            set
            {
                applicationState = value;
                OnPropertyChanged(nameof(ApplicationState));
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
    }
}
