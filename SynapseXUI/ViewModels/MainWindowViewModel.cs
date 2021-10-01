using MahApps.Metro.Controls.Dialogs;
using sxlib;
using sxlib.Specialized;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SynapseXUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly MainWindow mainWindow;
        private ScriptHubUserControl scriptHubUserControl;
        private OptionsUserControl optionsUserControl;
        private ProgressDialogController progressDialog;
        private string synapseStatus;
        private ScriptHubScript selectedHubScript;
        private bool synapseLoaded;

        public EditorUserControl EditorUserControl { get; private set; }

        public bool SynapseLoaded
        {
            get => synapseLoaded;
            set
            {
                synapseLoaded = value;
                OnPropertyChanged(nameof(SynapseLoaded));
            }
        }

        public ScriptHubScript SelectedHubScript
        {
            get => selectedHubScript;
            set
            {
                selectedHubScript = value;
                OnPropertyChanged(nameof(SelectedHubScript));
            }
        }

        public string SynapseStatus
        {
            get => synapseStatus;
            set
            {
                synapseStatus = value;
                OnPropertyChanged(nameof(SynapseStatus));
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

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            SynapseStatus = "Synapse X UI";
            InitializeSxLib();

            if (App.Settings.Size.SaveApplicationSize &&
                App.Settings.Size.ApplicationWidth >= mainWindow.MinWidth &&
                App.Settings.Size.ApplicationHeight >= mainWindow.MinHeight)
            {
                mainWindow.Width = App.Settings.Size.ApplicationWidth;
                mainWindow.Height = App.Settings.Size.ApplicationHeight;
            }
        }

        private async void InitializeSxLib()
        {
            progressDialog = await mainWindow.ShowProgressAsync("Loading Synapse X", "Please wait...");
            progressDialog.Minimum = 0;
            progressDialog.Maximum = 100;

            App.Lib = SxLib.InitializeWPF(mainWindow, App.StartupFolderPath);
            App.Lib.Load();
            App.Lib.LoadEvent += Lib_LoadEvent;
            App.Lib.AttachEvent += Lib_AttachEvent;
            App.Lib.ScriptHubEvent += Lib_ScriptHubEvent;
        }

        private void Lib_ScriptHubEvent(List<SxLibBase.SynHubEntry> e)
        {
            e.ForEach(x => scriptHubUserControl.ViewModel.Scripts.Add(new ScriptHubScript(x)));
            scriptHubUserControl.ViewModel.Loaded = true;
        }

        private async void Lib_AttachEvent(SxLibBase.SynAttachEvents e, object Param)
        {
            switch (e)
            {
                case SxLibBase.SynAttachEvents.CHECKING:
                    SynapseStatus = "Synapse X UI - Checking...";
                    break;
                case SxLibBase.SynAttachEvents.INJECTING:
                    SynapseStatus = "Synapse X UI - Injecting...";
                    break;
                case SxLibBase.SynAttachEvents.CHECKING_WHITELIST:
                    SynapseStatus = "Synapse X UI - Checking whitelist...";
                    break;
                case SxLibBase.SynAttachEvents.SCANNING:
                    SynapseStatus = "Synapse X UI - Scanning...";
                    break;
                case SxLibBase.SynAttachEvents.READY:
                    SynapseStatus = "Synapse X UI - Attached";
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_ATTACH:
                    SynapseStatus = "Synapse X UI - Failed to attach";
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_FIND:
                    SynapseStatus = "Synapse X UI - Failed to find Roblox";
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.NOT_INJECTED:
                    await mainWindow.ShowMessageAsync("Execution error", "Please attach Synapse X before executing a script");
                    break;
                case SxLibBase.SynAttachEvents.ALREADY_INJECTED:
                    SynapseStatus = "Synapse X UI - Already injected!";
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.PROC_DELETION:
                    SynapseStatus = "Synapse X UI";
                    mainWindow.buttonAttach.IsEnabled = true;
                    break;
            }
        }

        private async void Lib_LoadEvent(SxLibBase.SynLoadEvents e, object Param)
        {
            switch (e)
            {
                case SxLibBase.SynLoadEvents.UNKNOWN:
                    await progressDialog.CloseAsync();
                    await mainWindow.ShowMessageAsync("Loading Synapse X", "An unknown error occured");
                    mainWindow.Close();
                    break;
                case SxLibBase.SynLoadEvents.NOT_LOGGED_IN:
                    await progressDialog.CloseAsync();
                    await mainWindow.ShowMessageAsync("Loading Synapse X", "You are not logged in to Synapse X, please open the original application");
                    mainWindow.Close();
                    break;
                case SxLibBase.SynLoadEvents.NOT_UPDATED:
                    await progressDialog.CloseAsync();
                    await mainWindow.ShowMessageAsync("Loading Synapse X", "Synapse X hasn't been updated yet");
                    mainWindow.Close();
                    break;
                case SxLibBase.SynLoadEvents.FAILED_TO_VERIFY:
                    await progressDialog.CloseAsync();
                    await mainWindow.ShowMessageAsync("Loading Synapse X", "Failed to very data");
                    mainWindow.Close();
                    break;
                case SxLibBase.SynLoadEvents.FAILED_TO_DOWNLOAD:
                    await progressDialog.CloseAsync();
                    await mainWindow.ShowMessageAsync("Loading Synapse X", "Failed to download data");
                    mainWindow.Close();
                    break;
                case SxLibBase.SynLoadEvents.UNAUTHORIZED_HWID:
                    await progressDialog.CloseAsync();
                    await mainWindow.ShowMessageAsync("Loading Synapse X", "Unauthorized Hwid detected");
                    mainWindow.Close();
                    break;
                case SxLibBase.SynLoadEvents.CHECKING_WL:
                    progressDialog.SetMessage("Checking whitelist...");
                    progressDialog.SetProgress(20);
                    break;
                case SxLibBase.SynLoadEvents.DOWNLOADING_DATA:
                    progressDialog.SetMessage("Downloading data...");
                    progressDialog.SetProgress(40);
                    break;
                case SxLibBase.SynLoadEvents.CHECKING_DATA:
                    progressDialog.SetMessage("Checking data...");
                    progressDialog.SetProgress(60);
                    break;
                case SxLibBase.SynLoadEvents.DOWNLOADING_DLLS:
                    progressDialog.SetMessage("Downloading DLLs...");
                    progressDialog.SetProgress(80);
                    break;
                case SxLibBase.SynLoadEvents.READY:
                    progressDialog.SetMessage("Ready!");
                    progressDialog.SetProgress(100);

                    EditorUserControl = new EditorUserControl();
                    scriptHubUserControl = new ScriptHubUserControl();
                    optionsUserControl = new OptionsUserControl();
                    mainWindow.userControlEditor.Content = EditorUserControl;
                    mainWindow.userControlScriptHub.Content = scriptHubUserControl;
                    mainWindow.userControlOptions.Content = optionsUserControl;
                    SynapseLoaded = true;

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        await progressDialog.CloseAsync();
                    });
                    break;
            }
        }

        private void ResetSynapseStatus()
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                mainWindow.Dispatcher.Invoke(() =>
                {
                    SynapseStatus = "Synapse X UI";
                    mainWindow.buttonAttach.IsEnabled = true;
                });
            });
        }

        public void Attach()
        {
            mainWindow.buttonAttach.IsEnabled = false;
            App.Lib.Attach();
        }

        public void SaveSettings()
        {
            if (App.Settings.Size.SaveApplicationSize)
            {
                App.Settings.Size.ApplicationWidth = mainWindow.Width;
                App.Settings.Size.ApplicationHeight = mainWindow.Height;
            }

            App.Settings.Save(App.SettingsFilePath);
        }

        public void KillRoblox()
        {
            Process.GetProcessesByName("RobloxPlayerBeta").ToList().ForEach(x => x.Kill());
            mainWindow.buttonAttach.IsEnabled = true;
            SynapseStatus = "Synapse X UI";
        }
    }
}
