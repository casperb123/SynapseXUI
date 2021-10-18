﻿using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System.Windows;

namespace SynapseXUI.Windows
{
    /// <summary>
    /// Interaction logic for PromptWindow.xaml
    /// </summary>
    public partial class PromptWindow : MetroWindow
    {
        private readonly PromptWindowViewModel viewModel;

        public PromptWindow(string title, string message, PromptType type)
        {
            InitializeComponent();
            viewModel = new PromptWindowViewModel(this, title, message, type);
            DataContext = viewModel;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}