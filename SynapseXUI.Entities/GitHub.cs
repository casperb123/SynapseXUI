using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SynapseXUI.Entities
{
    public class GitHub : INotifyPropertyChanged
    {
        private readonly GitHubClient client;
        private readonly string gitHubUsername;
        private readonly string gitHubRepositoryName;
        private string releaseLink;

        private bool checkingForUpdate;
        private bool isUpdateAvailable;
        private Version currentVersion;
        private Version latestVersion;
        private string changelog;

        public string Changelog
        {
            get => changelog;
            set
            {
                changelog = value;
                OnPropertyChanged(nameof(Changelog));
            }
        }

        public Version LatestVersion
        {
            get => latestVersion;
            set
            {
                latestVersion = value;
                OnPropertyChanged(nameof(LatestVersion));
            }
        }

        public Version CurrentVersion
        {
            get => currentVersion;
            set
            {
                currentVersion = value;
                OnPropertyChanged(nameof(CurrentVersion));
            }
        }

        public bool IsUpdateAvailable
        {
            get => isUpdateAvailable;
            set
            {
                isUpdateAvailable = value;
                OnPropertyChanged(nameof(IsUpdateAvailable));
            }
        }

        public bool CheckingForUpdate
        {
            get => checkingForUpdate;
            private set
            {
                checkingForUpdate = value;
                OnPropertyChanged(nameof(CheckingForUpdate));
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

        public GitHub(string gitHubUsername, string gitHubRepositoryName)
        {
            client = new GitHubClient(new ProductHeaderValue("SynapseXMetroUI"));
            this.gitHubUsername = gitHubUsername;
            this.gitHubRepositoryName = gitHubRepositoryName;
            CurrentVersion = Version.ConvertToVersion(Assembly.GetEntryAssembly().GetName().Version.ToString(), true);
        }

        public async Task<bool> CheckForUpdateAsync()
        {
            try
            {
                CheckingForUpdate = true;
                IReadOnlyList<Release> releases = await client.Repository.Release.GetAll(gitHubUsername, gitHubRepositoryName);
                Release release = releases.FirstOrDefault(x => Version.ConvertToVersion(x.TagName.Replace("v", "")) > currentVersion);

                if (release is null)
                {
                    CheckingForUpdate = false;
                    return false;
                }
                else
                {
                    LatestVersion = Version.ConvertToVersion(release.TagName.Replace("v", ""));
                    IsUpdateAvailable = true;
                    Changelog = release.Body;
                    releaseLink = release.Assets[0].BrowserDownloadUrl;
                    CheckingForUpdate = false;
                    return true;
                }
            }
            catch (Exception)
            {
                CheckingForUpdate = false;
                throw;
            }
        }

        public void DownloadLatestRelease()
        {
            Process.Start(releaseLink);
        }
    }
}
