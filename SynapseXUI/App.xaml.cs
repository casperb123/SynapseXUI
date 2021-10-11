using CefSharp;
using CefSharp.Wpf;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
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
        public static string SettingsFolderPath { get; private set; }
        public static string SettingsFilePath { get; private set; }
        public static string TabsFilePath { get; private set; }
        public static string EditorFilePath { get; private set; }
        public static string ScriptsFolderPath { get; private set; }
        public static SxLibWPF Lib { get; set; }
        public static Options SxOptions { get; set; }
        public static Settings Settings { get; private set; }
        public static MetroDialogSettings DialogSettings { get; private set; }

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
            string libsFolderPath = Path.Combine(StartupFolderPath, "libs");
            string aceFolderPath = Path.Combine(libsFolderPath, "ace");
            SettingsFolderPath = Path.Combine(StartupFolderPath, "data");
            EditorFilePath = Path.Combine(aceFolderPath, "Editor.html");
            ScriptsFolderPath = Path.Combine(StartupFolderPath, "scripts");
            DialogSettings = new MetroDialogSettings
            {
                DefaultButtonFocus = MessageDialogResult.Affirmative,
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No"
            };

            if (!Directory.Exists(authFolderPath) ||
                !Directory.Exists(binFolderPath) ||
                !Directory.Exists(aceFolderPath) ||
                !Directory.Exists(ScriptsFolderPath))
            {
                MessageBox.Show("Please open the official Synapse X UI before using our UI", "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            }

            if (!Directory.Exists(SettingsFolderPath))
            {
                Directory.CreateDirectory(SettingsFolderPath);
            }

            SettingsFilePath = Path.Combine(SettingsFolderPath, "Options.ini");
            TabsFilePath = Path.Combine(SettingsFolderPath, "Tabs.bin");

            Settings = Settings.GetSettings(SettingsFilePath);
            string theme = Settings.Theme.ApplicationTheme;
            string color = Settings.Theme.ApplicationColor;

            SetTheme(theme, color);
            base.OnStartup(e);
        }

        public static void SetTheme(string theme, string color)
        {
            ThemeManager.Current.ChangeTheme(Current, $"{theme}.{color}");
        }
    }
}
