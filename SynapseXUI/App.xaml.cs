using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string StartUpPath { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            StartUpPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!Directory.Exists(Path.Combine(StartUpPath, "bin")))
            {
                MessageBox.Show("Please place the application in the Synapse X root folder (where the original Synapse X application is)", "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            base.OnStartup(e);
        }
    }
}
