using ControlzEx.Theming;
using SharpConfig;
using sxlib.Specialized;
using System;
using System.IO;
using System.Windows;
using static sxlib.Static.Data;

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
        public static Options SxOptions { get; set; }
        public static Configuration Options { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Instance = this;
            StartupPath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(StartupPath, "auth")) ||
                !Directory.Exists(Path.Combine(StartupPath, "bin")) ||
                !Directory.Exists(Path.Combine(StartupPath, "scripts")))
            {
                MessageBox.Show("Please open the official Synapse X application before using our application", "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            }

            string settingsPath = Path.Combine(StartupPath, "SynapseXUI.cfg");
            if (File.Exists(settingsPath))
            {
                Options = Configuration.LoadFromFile(settingsPath);
                string theme = Options["Theming"]["Theme"].StringValue;
                string color = Options["Theming"]["Color"].StringValue;

                SetTheme(theme, color);
            }
            else
            {
                Options = new Configuration();
                Options["Theming"]["Theme"].StringValue = "Dark";
                Options["Theming"]["Color"].StringValue = "Blue";
                Options.SaveToFile(settingsPath);
            }

            base.OnStartup(e);
        }

        public static void SetTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Current, $"{theme}.{color}");
        }
    }
}
