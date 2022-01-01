using sxlib;
using sxlib.Specialized;
using SynapseXUI.Entities;
using SynapseXUI.Windows;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SynapseXUI.ViewModels
{
    public class LoadingWindowViewModel : INotifyPropertyChanged
    {
        private readonly LoadingWindow loadingWindow;

        private double loadingProgress;
        private string loadingStatus;

        public string LoadingStatus
        {
            get => loadingStatus;
            set
            {
                loadingStatus = value;
                OnPropertyChanged(nameof(LoadingStatus));
            }
        }

        public double LoadingProgress
        {
            get => loadingProgress;
            set
            {
                loadingProgress = value;
                OnPropertyChanged(nameof(LoadingProgress));
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

        public LoadingWindowViewModel(LoadingWindow loadingWindow)
        {
            this.loadingWindow = loadingWindow;
            LoadingStatus = "Initializing...";
            App.Lib = SxLib.InitializeWPF(loadingWindow, App.StartupFolderPath);
            App.Lib.LoadEvent += Lib_LoadEvent;
            Load();
        }

        private async void Load()
        {
            await Task.Delay(1000);
            await App.CheckForUpdate(false);

            string authFolderPath = Path.Combine(App.StartupFolderPath, "auth");
            string binFolderPath = Path.Combine(App.StartupFolderPath, "bin");
            string libsFolderPath = Path.Combine(App.StartupFolderPath, "libs");
            string slInjectorLibsFilePath = Path.Combine(libsFolderPath, "SLInjector.dll");
            string slInjectorBinFilePath = Path.Combine(binFolderPath, "SLInjector.dll");

            if (!Directory.Exists(authFolderPath) ||
                !Directory.Exists(binFolderPath) ||
                !Directory.Exists(App.ScriptsFolderPath))
            {
                PromptWindow.Show("Synapse X UI", "Please open the official Synapse X UI before using our UI", PromptType.OK);
                Environment.Exit(2);
            }

            if (!File.Exists(slInjectorBinFilePath))
            {
                File.Copy(slInjectorLibsFilePath, slInjectorBinFilePath);
            }

            App.Lib.Load();
        }

        private async void Lib_LoadEvent(SxLibBase.SynLoadEvents Event, object Param)
        {
            switch (Event)
            {
                case SxLibBase.SynLoadEvents.UNKNOWN:
                    PromptWindow.Show("Loading Synapse X", "An unknown error occured. Please try opening the application again", PromptType.OK);
                    Environment.Exit(0);
                    break;
                case SxLibBase.SynLoadEvents.NOT_LOGGED_IN:
                    PromptWindow.Show("Loading Synapse X", "You are not logged in to Synapse X. Please open the official UI", PromptType.OK);
                    Environment.Exit(0);
                    break;
                case SxLibBase.SynLoadEvents.NOT_UPDATED:
                    PromptWindow.Show("Loading Synapse X", "Synapse X is not currently updated. Please wait for an update to release", PromptType.OK);
                    Environment.Exit(0);
                    break;
                case SxLibBase.SynLoadEvents.FAILED_TO_VERIFY:
                    PromptWindow.Show("Loading Synapse X", "Failed to verify data. Please open the official UI", PromptType.OK);
                    Environment.Exit(0);
                    break;
                case SxLibBase.SynLoadEvents.FAILED_TO_DOWNLOAD:
                    PromptWindow.Show("Loading Synapse X", "Failed to download data. Please check your anti-virus software", PromptType.OK);
                    Environment.Exit(0);
                    break;
                case SxLibBase.SynLoadEvents.UNAUTHORIZED_HWID:
                    PromptWindow.Show("Loading Synapse X", "Unauthorized Hwid detected. Please open the official UI", PromptType.OK);
                    Environment.Exit(0);
                    break;
                case SxLibBase.SynLoadEvents.CHECKING_WL:
                    LoadingStatus = "Checking whitelist...";
                    LoadingProgress = 20;
                    break;
                case SxLibBase.SynLoadEvents.DOWNLOADING_DATA:
                    LoadingStatus = "Downloading data...";
                    LoadingProgress = 40;
                    break;
                case SxLibBase.SynLoadEvents.CHECKING_DATA:
                    LoadingStatus = "Checking data...";
                    LoadingProgress = 60;
                    break;
                case SxLibBase.SynLoadEvents.DOWNLOADING_DLLS:
                    LoadingStatus = "Downloading DLLs...";
                    LoadingProgress = 80;
                    break;
                case SxLibBase.SynLoadEvents.READY:
                    LoadingStatus = "Ready!";
                    LoadingProgress = 100;

                    await Task.Delay(1000);
                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;

                    mainWindow.Show();
                    App.Lib.SetWindow(mainWindow);
                    loadingWindow.Close();
                    break;
            }
        }
    }
}
