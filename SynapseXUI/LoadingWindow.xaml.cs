﻿using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : MetroWindow
    {
        private readonly LoadingWindowViewModel viewModel;

        public LoadingWindow()
        {
            InitializeComponent();
            viewModel = new LoadingWindowViewModel(this);
            DataContext = viewModel;
        }
    }
}