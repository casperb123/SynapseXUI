using System.ComponentModel;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace SynapseXUI.Entities
{
    public class Settings : INotifyPropertyChanged
    {
        private Theme theme;
        private WindowSize windowSize;
        private GridLength scriptsListWidth;
        private bool autoRefreshScripts;
        private bool saveTabs;
        private bool loaderTopMost;

        public bool LoaderTopMost
        {
            get => loaderTopMost;
            set
            {
                loaderTopMost = value;
                OnPropertyChanged(nameof(LoaderTopMost));
            }
        }

        public bool SaveTabs
        {
            get => saveTabs;
            set
            {
                saveTabs = value;
                OnPropertyChanged(nameof(SaveTabs));
            }
        }

        public bool AutoRefreshScripts
        {
            get => autoRefreshScripts;
            set
            {
                autoRefreshScripts = value;
                OnPropertyChanged(nameof(AutoRefreshScripts));
            }
        }

        public GridLength ScriptsListWidth
        {
            get => scriptsListWidth;
            set
            {
                scriptsListWidth = value;
                OnPropertyChanged(nameof(ScriptsListWidth));
            }
        }

        public WindowSize WindowSize
        {
            get => windowSize;
            set
            {
                windowSize = value;
                OnPropertyChanged(nameof(WindowSize));
            }
        }

        public Theme Theme
        {
            get => theme;
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
            LoaderTopMost = true;
            SaveTabs = true;
            Theme = new Theme();
            WindowSize = new WindowSize();
            ScriptsListWidth = new GridLength(200);
        }

        public void Save(string settingsFilePath)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }

        public void SetDefault()
        {
            LoaderTopMost = true;
            SaveTabs = true;
            Theme.SetDefault();
            WindowSize.SetDefault();
            ScriptsListWidth = new GridLength(200);
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
        private string applicationColor;
        private string applicationTheme;

        public string ApplicationTheme
        {
            get => applicationTheme;
            set
            {
                applicationTheme = value;
                OnPropertyChanged(nameof(ApplicationTheme));
            }
        }

        public string ApplicationColor
        {
            get => applicationColor;
            set
            {
                applicationColor = value;
                OnPropertyChanged(nameof(ApplicationColor));
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
            SetDefault();
        }

        public void SetDefault()
        {
            ApplicationTheme = "Dark";
            ApplicationColor = "Blue";
        }
    }

    public class WindowSize : INotifyPropertyChanged
    {
        private double windowWidth;
        private double windowHeight;
        private WindowState windowState;

        public double WindowWidth
        {
            get => windowWidth;
            set
            {
                windowWidth = value;
                OnPropertyChanged(nameof(WindowWidth));
            }
        }

        public double WindowHeight
        {
            get => windowHeight;
            set
            {
                windowHeight = value;
                OnPropertyChanged(nameof(WindowHeight));
            }
        }

        public WindowState WindowState
        {
            get => windowState;
            set
            {
                windowState = value;
                OnPropertyChanged(nameof(WindowState));
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

        public WindowSize()
        {
            SetDefault();
        }

        public void SetDefault()
        {
            WindowWidth = 800;
            WindowHeight = 500;
            WindowState = WindowState.Normal;
        }
    }
}
