using System;

namespace SynapseXUI.Entities
{
    public class VersionEventArgs : EventArgs
    {
        public Version CurrentVersion { get; set; }
        public Version LatestVersion { get; set; }

        public VersionEventArgs(Version currentVersion, Version latestVersion)
        {
            CurrentVersion = currentVersion;
            LatestVersion = latestVersion;
        }
    }
}
