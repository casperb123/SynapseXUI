using sxlib;
using sxlib.Specialized;
using System.ComponentModel;
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
            LoadLib();
        }

        private async void LoadLib()
        {
            await Task.Delay(1000);
            App.Lib.Load();
        }

        private async void Lib_LoadEvent(SxLibBase.SynLoadEvents Event, object Param)
        {
            switch (Event)
            {
                case SxLibBase.SynLoadEvents.UNKNOWN:
                    MessageBox.Show("Loading Synapse X", "An unknown error occured, please open the official UI");
                    break;
                case SxLibBase.SynLoadEvents.NOT_LOGGED_IN:
                    MessageBox.Show("Loading Synapse X", "You are not logged in to Synapse X, please open the official UI");
                    break;
                case SxLibBase.SynLoadEvents.NOT_UPDATED:
                    MessageBox.Show("Loading Synapse X", "Synapse X hasn't been updated yet");
                    break;
                case SxLibBase.SynLoadEvents.FAILED_TO_VERIFY:
                    MessageBox.Show("Loading Synapse X", "Failed to verify data, please open the official UI");
                    break;
                case SxLibBase.SynLoadEvents.FAILED_TO_DOWNLOAD:
                    MessageBox.Show("Loading Synapse X", "Failed to download data, please check your anti-virus software");
                    break;
                case SxLibBase.SynLoadEvents.UNAUTHORIZED_HWID:
                    MessageBox.Show("Loading Synapse X", "Unauthorized Hwid detected, please open the official UI");
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
                    mainWindow.Show();
                    App.Lib.SetWindow(mainWindow);
                    loadingWindow.Close();
                    break;
            }
        }
    }
}
