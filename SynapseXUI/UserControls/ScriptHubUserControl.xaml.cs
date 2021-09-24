using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for ScriptHubUserControl.xaml
    /// </summary>
    public partial class ScriptHubUserControl : UserControl
    {
        public readonly ScriptHubUserControlViewModel ViewModel;

        public ScriptHubUserControl()
        {
            InitializeComponent();
            ViewModel = new ScriptHubUserControlViewModel();
            DataContext = ViewModel;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenScript(sender as Tile);
        }
    }
}
