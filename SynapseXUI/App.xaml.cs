using CefSharp;
using CefSharp.Wpf;
using ControlzEx.Theming;
using sxlib.Specialized;
using SynapseXUI.Entities;
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
        public static string StartupFolderPath { get; private set; }
        public static string SettingsFilePath { get; private set; }
        public static string EditorFilePath { get; private set; }
        public static string ScriptsFolderPath { get; private set; }
        public static SxLibWPF Lib { get; set; }
        public static Options SxOptions { get; set; }
        public static Settings Settings { get; private set; }

        public App()
        {
            CefSettings cefSettings = new CefSettings
            {
                LogSeverity = LogSeverity.Disable
            };

            if (!Cef.IsInitialized)
            {
                Cef.Initialize(cefSettings);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Instance = this;
            StartupFolderPath = Directory.GetCurrentDirectory();
            string authFolderPath = Path.Combine(StartupFolderPath, "auth");
            string binFolderPath = Path.Combine(StartupFolderPath, "bin");

            SettingsFilePath = Path.Combine(StartupFolderPath, "SynapseXUI.ini");
            EditorFilePath = Path.Combine(binFolderPath, "Editor.html");
            ScriptsFolderPath = Path.Combine(StartupFolderPath, "scripts");

            if (!Directory.Exists(authFolderPath) ||
                !Directory.Exists(binFolderPath) ||
                !Directory.Exists(ScriptsFolderPath))
            {
                MessageBox.Show("Please open the official Synapse X application before using our application", "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            }

            Settings = Settings.GetSettings(SettingsFilePath);
            string theme = Settings.Theming.ApplicationTheme;
            string color = Settings.Theming.ApplicationColor;

            SetTheme(theme, color);
            base.OnStartup(e);
        }

        public static void SetTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Current, $"{theme}.{color}");
        }
    }
}
