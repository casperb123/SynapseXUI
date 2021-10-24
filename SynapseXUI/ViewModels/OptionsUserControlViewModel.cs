using ControlzEx.Theming;
using SynapseXUI.Entities;
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
        private bool closeTabConfirmation;
        private bool clearEditorConfirmation;

        public bool CloseTabConfirmation
        {
            get => closeTabConfirmation;
            set
            {
                closeTabConfirmation = value;
                OnPropertyChanged(nameof(CloseTabConfirmation));
            }
        }

        public bool ClearEditorConfirmation
        {
            get => clearEditorConfirmation;
            set
            {
                clearEditorConfirmation = value;
                OnPropertyChanged(nameof(ClearEditorConfirmation));
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

        public bool InternalUi
        {
            get => internalUi;
            set
            {
                internalUi = value;
                OnPropertyChanged(nameof(InternalUi));
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

        public bool AutoLaunch
        {
            get => autoLaunch;
            set
            {
                autoLaunch = value;
                OnPropertyChanged(nameof(AutoLaunch));
            }
        }

        public bool UnlockFps
        {
            get => unlockFps;
            set
            {
                unlockFps = value;
                OnPropertyChanged(nameof(UnlockFps));
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
            CloseTabConfirmation = App.SxOptions.CloseConfirmation;
            ClearEditorConfirmation = App.SxOptions.ClearConfirmation;

            userControl.comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            userControl.comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;
        }

        public void SaveSxOptions()
        {
            App.SxOptions.UnlockFPS = UnlockFps;
            App.SxOptions.AutoLaunch = AutoLaunch;
            App.SxOptions.AutoAttach = AutoAttach;
            App.SxOptions.InternalUI = InternalUi;
            App.SxOptions.TopMost = TopMost;
            App.SxOptions.CloseConfirmation = CloseTabConfirmation;
            App.SxOptions.ClearConfirmation = ClearEditorConfirmation;

            App.Lib.SetOptions(App.SxOptions);
        }

        public void SaveSettings()
        {
            string theme = App.Settings.Theme.ApplicationTheme;
            string color = App.Settings.Theme.ApplicationColor;

            App.Settings.Save(App.SettingsFilePath);
            App.SetTheme(theme, color);
            MainWindow.Instance.ViewModel.EditorUserControl.ViewModel.SetEditorTheme(theme);
        }
    }
}
