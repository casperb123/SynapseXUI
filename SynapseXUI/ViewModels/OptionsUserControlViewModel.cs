using ControlzEx.Theming;
using SynapseXUI.UserControls;
using System.ComponentModel;

namespace SynapseXUI.ViewModels
{
    public class OptionsUserControlViewModel : INotifyPropertyChanged
    {
        private bool unlockFps;
        private bool autoLaunch;
        private bool autoAttach;
        private bool internalUi;
        private bool topMost;
        private bool clearEditorPrompt;

        public bool UnlockFps
        {
            get => unlockFps;
            set
            {
                unlockFps = value;
                OnPropertyChanged(nameof(UnlockFps));
            }
        }

        public bool AutoLaunch
        {
            get => autoLaunch;
            set
            {
                autoLaunch = value;
                OnPropertyChanged(nameof(AutoLaunch));
            }
        }

        public bool AutoAttach
        {
            get => autoAttach;
            set
            {
                autoAttach = value;
                OnPropertyChanged(nameof(AutoAttach));
            }
        }

        public bool InternalUi
        {
            get => internalUi;
            set
            {
                internalUi = value;
                OnPropertyChanged(nameof(InternalUi));
            }
        }

        public bool TopMost
        {
            get => topMost;
            set
            {
                topMost = value;
                OnPropertyChanged(nameof(TopMost));
                MainWindow.Instance.Topmost = value;
            }
        }

        public bool ClearEditorPrompt
        {
            get => clearEditorPrompt;
            set
            {
                clearEditorPrompt = value;
                OnPropertyChanged(nameof(ClearEditorPrompt));
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

        public OptionsUserControlViewModel(OptionsUserControl userControl)
        {
            App.SxOptions = App.Lib.GetOptions();

            UnlockFps = App.SxOptions.UnlockFPS;
            AutoLaunch = App.SxOptions.AutoLaunch;
            AutoAttach = App.SxOptions.AutoAttach;
            InternalUi = App.SxOptions.InternalUI;
            TopMost = App.SxOptions.TopMost;
            ClearEditorPrompt = App.SxOptions.ClearConfirmation;

            userControl.comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            userControl.comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;
        }

        public void SetSxOptions()
        {
            App.SxOptions.UnlockFPS = UnlockFps;
            App.SxOptions.AutoLaunch = AutoLaunch;
            App.SxOptions.AutoAttach = AutoAttach;
            App.SxOptions.InternalUI = InternalUi;
            App.SxOptions.TopMost = TopMost;
            App.SxOptions.ClearConfirmation = ClearEditorPrompt;

            App.Lib.SetOptions(App.SxOptions);
        }

        public void SaveSettings()
        {
            string theme = App.Settings.Theming.ApplicationTheme;
            string color = App.Settings.Theming.ApplicationColor;

            App.Settings.Save(App.SettingsFilePath);
            App.SetTheme(theme, color);
            MainWindow.Instance.ViewModel.EditorUserControl.ViewModel.SetAllEditorThemes(theme);
        }
    }
}
