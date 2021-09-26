using ControlzEx.Theming;
using SharpConfig;
using SynapseXUI.UserControls;
using System;
using System.ComponentModel;
using System.IO;
using static sxlib.Static.Data;

namespace SynapseXUI.ViewModels
{
    public class OptionsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly OptionsUserControl userControl;
        private bool unlockFps;
        private bool autoLaunch;
        private bool autoAttach;
        private bool internalUi;
        private bool topMost;
        private bool clearEditorPrompt;
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
            this.userControl = userControl;
            App.SxOptions = App.Lib.GetOptions();

            UnlockFps = App.SxOptions.UnlockFPS;
            AutoLaunch = App.SxOptions.AutoLaunch;
            AutoAttach = App.SxOptions.AutoAttach;
            InternalUi = App.SxOptions.InternalUI;
            TopMost = App.SxOptions.TopMost;
            ClearEditorPrompt = App.SxOptions.ClearConfirmation;

            userControl.comboBoxTheme.ItemsSource = ThemeManager.Current.BaseColors;
            userControl.comboBoxColor.ItemsSource = ThemeManager.Current.ColorSchemes;
            ApplicationTheme = App.Options["Theming"]["Theme"].StringValue;
            ApplicationColor = App.Options["Theming"]["Color"].StringValue;
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

        public void SetOptions()
        {
            string optionsPath = Path.Combine(App.StartupPath, "SynapseXUI.cfg");

            App.Options["Theming"]["Theme"].StringValue = ApplicationTheme;
            App.Options["Theming"]["Color"].StringValue = ApplicationColor;

            App.Options.SaveToFile(optionsPath);
            App.SetTheme(ApplicationTheme, ApplicationColor);
            MainWindow.Instance.ViewModel.EditorUserControl.ViewModel.SetAllEditorThemes(ApplicationTheme);
        }
    }
}
