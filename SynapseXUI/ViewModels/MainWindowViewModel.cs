using sxlib.Specialized;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SynapseXUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly MainWindow mainWindow;
        private readonly OptionsUserControl optionsUserControl;
        private string synapseStatus;
        private SynapseHubScript selectedSynapseHubScript;
        private RbxHubScript selectedRbxHubcScript;

        public EditorUserControl EditorUserControl { get; private set; }
        public ScriptHubUserControl ScriptHubUserControl { get; private set; }

        public RbxHubScript SelectedRbxHubScript
        {
            get => selectedRbxHubcScript;
            set
            {
                selectedRbxHubcScript = value;
                OnPropertyChanged(nameof(SelectedRbxHubScript));
            }
        }

        public SynapseHubScript SelectedSynapseHubScript
        {
            get => selectedSynapseHubScript;
            set
            {
                selectedSynapseHubScript = value;
                OnPropertyChanged(nameof(SelectedSynapseHubScript));
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
            App.Lib.AttachEvent += Lib_AttachEvent;

            EditorUserControl = new EditorUserControl();
            ScriptHubUserControl = new ScriptHubUserControl();
            optionsUserControl = new OptionsUserControl();
            mainWindow.userControlEditor.Content = EditorUserControl;
            mainWindow.userControlScriptHub.Content = ScriptHubUserControl;
            mainWindow.userControlOptions.Content = optionsUserControl;

            if (App.Settings.WindowSize.SaveWindowSize &&
                App.Settings.WindowSize.WindowWidth >= mainWindow.MinWidth &&
                App.Settings.WindowSize.WindowHeight >= mainWindow.MinHeight)
            {
                mainWindow.Width = App.Settings.WindowSize.WindowWidth;
                mainWindow.Height = App.Settings.WindowSize.WindowHeight;
                mainWindow.WindowState = App.Settings.WindowSize.WindowState;
            }
        }

        private void Lib_AttachEvent(SxLibBase.SynAttachEvents Event, object Param)
        {
            switch (Event)
            {
                case SxLibBase.SynAttachEvents.CHECKING:
                    SetSynapseStatus("Checking...");
                    break;
                case SxLibBase.SynAttachEvents.INJECTING:
                    SetSynapseStatus("Injecting...");
                    break;
                case SxLibBase.SynAttachEvents.CHECKING_WHITELIST:
                    SetSynapseStatus("Checking whitelist...");
                    break;
                case SxLibBase.SynAttachEvents.SCANNING:
                    SetSynapseStatus("Scanning...");
                    break;
                case SxLibBase.SynAttachEvents.READY:
                    SetSynapseStatus("Attached");
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_ATTACH:
                    SetSynapseStatus("Failed to attach");
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_FIND:
                    SetSynapseStatus("Failed to find Roblox");
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.NOT_INJECTED:
                    App.ShowPrompt("Execution error", "Please attach Synapse X before executing a script", PromptType.OK);
                    break;
                case SxLibBase.SynAttachEvents.ALREADY_INJECTED:
                    SetSynapseStatus("Already injected");
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.PROC_DELETION:
                    SetSynapseStatus();
                    mainWindow.buttonAttach.IsEnabled = true;
                    break;
            }
        }

        private void SetSynapseStatus(string status = null)
        {
            SynapseStatus = string.IsNullOrWhiteSpace(status) ? string.Empty : $" - {status}";
        }

        private void ResetSynapseStatus()
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                mainWindow.Dispatcher.Invoke(() =>
                {
                    SetSynapseStatus();
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
            if (App.Settings.WindowSize.SaveWindowSize)
            {
                App.Settings.WindowSize.WindowWidth = mainWindow.Width;
                App.Settings.WindowSize.WindowHeight = mainWindow.Height;
                App.Settings.WindowSize.WindowState = mainWindow.WindowState;
            }

            App.Settings.Save(App.SettingsFilePath);
        }

        public void KillRoblox()
        {
            Process.GetProcessesByName("RobloxPlayerBeta").ToList().ForEach(x => x.Kill());
            mainWindow.buttonAttach.IsEnabled = true;
            SetSynapseStatus();
        }
    }
}
