using sxlib.Specialized;
using System;
using System.IO;
using System.Windows;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get; private set; }
        public static string StartupPath { get; private set; }
        public static SxLibWPF Lib { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Instance = this;
            StartupPath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(StartupPath, "auth")) || !Directory.Exists(Path.Combine(StartupPath, "bin")))
            {
                MessageBox.Show("Please open the original application before using our application", "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            }

            base.OnStartup(e);
        }
    }
}
