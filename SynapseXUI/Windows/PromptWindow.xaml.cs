using MahApps.Metro.Controls;
using SynapseXUI.Entities;
using SynapseXUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            viewModel = new PromptWindowViewModel(title, message, type);
            DataContext = viewModel;
            Topmost = App.SxOptions.TopMost;

            if (type == PromptType.OK)
            {
                buttonOk.Focus();
            }
            else if (type == PromptType.YesNo)
            {
                buttonYes.Focus();
            }
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
