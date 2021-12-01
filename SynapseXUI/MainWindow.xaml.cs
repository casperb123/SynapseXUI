﻿using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using SynapseXUI.Windows;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public readonly MainWindowViewModel ViewModel;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            ViewModel.EditorUserControl.ViewModel.SaveTabs();
            App.Settings.Save(App.SettingsFilePath);
            Environment.Exit(0);
        }

        private void ButtonAttach_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Attach();
        }

        private void ButtonExecuteSynapseHubScript_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedSynapseHubScript.Script.Execute();
        }

        private void ButtonKillRoblox_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.KillRoblox();
        }

        private void ButtonReinstallRoblox_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReinstallRoblox();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            App.NotifyUpdate();
        }

        private void IconGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            PromptWindow.Show("Synapse X", "This UI is developed by Casper:\n" +
                                        "Discord: CasperTheGhost#3549", PromptType.OK);
        }

        private void MetroWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.SetEditorScriptsMaxWidth();
        }
    }
}
