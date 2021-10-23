using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for RbxScriptsHubUserControl.xaml
    /// </summary>
    public partial class RbxScriptsHubUserControl : UserControl
    {
        private readonly RbxScriptsHubUserControlViewModel viewModel;

        public RbxScriptsHubUserControl()
        {
            InitializeComponent();
            viewModel = new RbxScriptsHubUserControlViewModel();
            DataContext = viewModel;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ExecuteScript(sender as Tile);
        }
    }
}
