﻿using MahApps.Metro.Controls;
using sxlib.Specialized;
using SynapseXUI.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SynapseXUI.ViewModels
{
    public class SynapseHubUserControlViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SynapseHubScript> scripts;
        private bool isLoading;

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ObservableCollection<SynapseHubScript> Scripts
        {
            get => scripts;
            set
            {
                scripts = value;
                OnPropertyChanged(nameof(Scripts));
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

        public SynapseHubUserControlViewModel()
        {
            Scripts = new ObservableCollection<SynapseHubScript>();
            App.Lib.ScriptHubEvent += Lib_ScriptHubEvent;
        }

        private void Lib_ScriptHubEvent(List<SxLibBase.SynHubEntry> e)
        {
            e.ForEach(x => Scripts.Add(new SynapseHubScript(x)));
            IsLoading = false;
        }

        public void GetScripts()
        {
            IsLoading = true;
            App.Lib.ScriptHub();
        }

        public void OpenScript(Tile tile)
        {
            MainWindow.Instance.ViewModel.SelectedSynapseHubScript = Scripts.FirstOrDefault(x => x.Name == tile.Title);
            MainWindow.Instance.flyoutSynapseScript.IsOpen = true;
        }
    }
}
