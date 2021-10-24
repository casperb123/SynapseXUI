﻿using MahApps.Metro.Controls;
using Newtonsoft.Json;
using SynapseXUI.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;

namespace SynapseXUI.ViewModels
{
    public class RbxScriptsHubUserControlViewModel : INotifyPropertyChanged
    {
        private readonly Uri rbxScriptsLink;
        private readonly WebClient webClient;
        private ObservableCollection<RbxHubScript> scripts;
        private bool loaded;

        public bool Loaded
        {
            get => loaded;
            set
            {
                loaded = value;
                OnPropertyChanged(nameof(Loaded));
            }
        }

        public ObservableCollection<RbxHubScript> Scripts
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

        public RbxScriptsHubUserControlViewModel()
        {
            rbxScriptsLink = new Uri("https://rbxscripts.xyz/wp-json/wl/v1/posts/");
            Scripts = new ObservableCollection<RbxHubScript>();
            webClient = new WebClient();
            webClient.DownloadStringCompleted += Client_DownloadStringCompleted;
            webClient.DownloadStringAsync(rbxScriptsLink);
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (App.ShowPrompt("Getting scripts failed", "Getting the scripts from rbxscriptx.xyz failed, would you like to try again?\n\n" +
                                                             "Error:\n" +
                                                             $"{e.Error.Message}", PromptType.YesNo))
                {
                    webClient.DownloadStringAsync(rbxScriptsLink);
                }
            }
            else
            {
                List<RbxHubScript> scripts = JsonConvert.DeserializeObject<RbxHubScript[]>(e.Result).ToList();
                scripts.ForEach(x => Scripts.Add(x));
                Loaded = true;
            }
        }

        public void ExecuteScript(Tile tile)
        {
            RbxHubScript script = Scripts.FirstOrDefault(x => x.Id == (int)tile.Tag);
            App.Lib.Execute(script.Excerpt);
        }
    }
}