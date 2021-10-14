﻿using MahApps.Metro.Controls.Dialogs;
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
        private readonly ScriptHubUserControl scriptHubUserControl;
        private readonly OptionsUserControl optionsUserControl;
        private string synapseStatus;
        private ScriptHubScript selectedHubScript;

        public EditorUserControl EditorUserControl { get; private set; }

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
            SynapseStatus = "Synapse X";

            App.Lib.ScriptHubEvent += Lib_ScriptHubEvent;
            App.Lib.AttachEvent += Lib_AttachEvent;

            EditorUserControl = new EditorUserControl();
            scriptHubUserControl = new ScriptHubUserControl();
            optionsUserControl = new OptionsUserControl();
            mainWindow.userControlEditor.Content = EditorUserControl;
            mainWindow.userControlScriptHub.Content = scriptHubUserControl;
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

        private void Lib_ScriptHubEvent(List<SxLibBase.SynHubEntry> e)
        {
            e.ForEach(x => scriptHubUserControl.ViewModel.Scripts.Add(new ScriptHubScript(x)));
            scriptHubUserControl.ViewModel.Loaded = true;
        }

        private void Lib_AttachEvent(SxLibBase.SynAttachEvents e, object Param)
        {
            switch (e)
            {
                case SxLibBase.SynAttachEvents.CHECKING:
                    SynapseStatus = "Synapse X - Checking...";
                    break;
                case SxLibBase.SynAttachEvents.INJECTING:
                    SynapseStatus = "Synapse X - Injecting...";
                    break;
                case SxLibBase.SynAttachEvents.CHECKING_WHITELIST:
                    SynapseStatus = "Synapse X - Checking whitelist...";
                    break;
                case SxLibBase.SynAttachEvents.SCANNING:
                    SynapseStatus = "Synapse X - Scanning...";
                    break;
                case SxLibBase.SynAttachEvents.READY:
                    SynapseStatus = "Synapse X - Attached";
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_ATTACH:
                    SynapseStatus = "Synapse X - Failed to attach";
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.FAILED_TO_FIND:
                    SynapseStatus = "Synapse X - Failed to find Roblox";
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.NOT_INJECTED:
                    App.ShowPrompt("Execution error", "Please attach Synapse X before executing a script", PromptType.OK);
                    break;
                case SxLibBase.SynAttachEvents.ALREADY_INJECTED:
                    SynapseStatus = "Synapse X - Already injected!";
                    ResetSynapseStatus();
                    break;
                case SxLibBase.SynAttachEvents.PROC_DELETION:
                    SynapseStatus = "Synapse X";
                    mainWindow.buttonAttach.IsEnabled = true;
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
                    SynapseStatus = "Synapse X";
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
            SynapseStatus = "Synapse X";
        }
    }
}
