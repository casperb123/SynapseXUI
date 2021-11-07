﻿using Microsoft.Win32;
using Newtonsoft.Json;
using SynapseXUI.Entities;
using SynapseXUI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SynapseXUI.ViewModels
{
    public class RbxScriptsHubUserControlViewModel : INotifyPropertyChanged
    {
        private readonly Uri rbxScriptsLink;
        private readonly WebClient webClient;
        private List<RbxHubScript> loadedScripts;
        private ObservableCollection<RbxHubScript> scripts;
        private bool isLoading;
        private string searchQuery;

        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
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
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (PromptWindow.Show("Getting scripts failed", "Getting the scripts from rbxscriptx.xyz failed, would you like to try again?\n\n" +
                                                             "Error:\n" +
                                                             $"{e.Error.Message}", PromptType.YesNo))
                {
                    webClient.DownloadStringAsync(rbxScriptsLink);
                }
            }
            else
            {
                loadedScripts = JsonConvert.DeserializeObject<RbxHubScript[]>(e.Result).ToList();
                FilterScripts();
            }
        }

        public void GetScripts()
        {
            IsLoading = true;
            Scripts.Clear();
            webClient.DownloadStringAsync(rbxScriptsLink);
        }

        public void FilterScripts()
        {
            Scripts.Clear();
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                loadedScripts.ForEach(x => Scripts.Add(x));
            }
            else
            {
                loadedScripts.Where(x => x.Title.ToLower().Contains(SearchQuery.ToLower())).ToList().ForEach(x => Scripts.Add(x));
            }
            IsLoading = false;
        }

        public void DownloadScript(RbxHubScript script)
        {
            string name = Regex.Replace(script.Title, "[^a-zA-Z0-9-_.]+", "", RegexOptions.Compiled);

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Download script",
                Filter = "Script Files|*.lua;*.txt",
                InitialDirectory = App.ScriptsFolderPath,
                FileName = name
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, script.Excerpt);
            }
        }
    }
}
