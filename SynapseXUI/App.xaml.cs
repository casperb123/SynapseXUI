using CefSharp;
using CefSharp.Wpf;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using sxlib.Specialized;
using SynapseXUI.Entities;
using SynapseXUI.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public static string DataFolderPath { get; private set; }
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
            if (Process.GetProcessesByName("SynapseXUI").ToList().Count > 1)
            {
                PromptWindow.Show("Synapse X UI", "An instance of this application is already running", PromptType.OK);
                Environment.Exit(32);
            }

            Instance = this;
            StartupFolderPath = Directory.GetCurrentDirectory();
            ScriptsFolderPath = Path.Combine(StartupFolderPath, "scripts");
            string authFolderPath = Path.Combine(StartupFolderPath, "auth");
            string binFolderPath = Path.Combine(StartupFolderPath, "bin");
            string libsFolderPath = Path.Combine(StartupFolderPath, "libs");
            string aceFolderPath = Path.Combine(libsFolderPath, "ace");
            string slInjectorLibsFilePath = Path.Combine(libsFolderPath, "SLInjector.dll");
            string slInjectorBinFilePath = Path.Combine(binFolderPath, "SLInjector.dll");
            DataFolderPath = Path.Combine(libsFolderPath, "data");
            EditorFilePath = Path.Combine(aceFolderPath, "Editor.html");
            DialogSettings = new MetroDialogSettings
            {
                DefaultButtonFocus = MessageDialogResult.Affirmative,
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No"
            };

            if (!Directory.Exists(authFolderPath) ||
                !Directory.Exists(binFolderPath) ||
                !Directory.Exists(ScriptsFolderPath))
            {
                PromptWindow.Show("Synapse X UI", "Please open the official Synapse X UI before using our UI", PromptType.OK);
                Environment.Exit(2);
            }

            if (!File.Exists(slInjectorBinFilePath))
            {
                File.Copy(slInjectorLibsFilePath, slInjectorBinFilePath);
            }

            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }

            SettingsFilePath = Path.Combine(DataFolderPath, "Options.ini");
            TabsFilePath = Path.Combine(DataFolderPath, "Tabs.bin");

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
