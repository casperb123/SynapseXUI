using CefSharp;
using CefSharp.Wpf;
using ControlzEx.Theming;
using sxlib.Specialized;
using SynapseXUI.Entities;
using SynapseXUI.UserControls;
using SynapseXUI.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public static GitHub GitHub { get; private set; }

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
            GitHub = new GitHub("casperb123", "SynapseXUI");
            StartupFolderPath = Directory.GetCurrentDirectory();
            ScriptsFolderPath = Path.Combine(StartupFolderPath, "scripts");
            string libsFolderPath = Path.Combine(StartupFolderPath, "libs");
            string aceFolderPath = Path.Combine(libsFolderPath, "ace");
            DataFolderPath = Path.Combine(libsFolderPath, "data");
            EditorFilePath = Path.Combine(aceFolderPath, "Editor.html");
            SettingsFilePath = Path.Combine(DataFolderPath, "Options.ini");
            TabsFilePath = Path.Combine(DataFolderPath, "Tabs.bin");

            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }

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

        public static void ResetSettings()
        {
            Settings.SetDefault();
            EditorUserControl.Instance.columnScripts.Width = Settings.ScriptsListWidth;
            Current.MainWindow.WindowState = Settings.WindowSize.WindowState;
            Current.MainWindow.Width = Settings.WindowSize.WindowWidth;
            Current.MainWindow.Height = Settings.WindowSize.WindowHeight;

            Settings.Save(SettingsFilePath);
        }

        public static async Task CheckForUpdate(bool notifyIfLatest)
        {
            try
            {
                bool updateAvailable = await GitHub.CheckForUpdateAsync();
                if (updateAvailable)
                {
                    if (PromptWindow.Show("Update Available", $"An update is available, would you like to download it?\n" +
                                                              $"Current Version: {GitHub.CurrentVersion}\n" +
                                                              $"Latest Version: {GitHub.LatestVersion}\n\n" +
                                                              $"{GitHub.Changelog}", PromptType.YesNo))
                    {
                        GitHub.DownloadLatestRelease();
                    }
                }
                else if (notifyIfLatest)
                {
                    PromptWindow.Show("No Update Available", "No update is available, you're already on the latest version", PromptType.OK);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
