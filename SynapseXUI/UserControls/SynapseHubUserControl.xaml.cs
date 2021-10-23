using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for SynapseScriptHubUserControl.xaml
    /// </summary>
    public partial class SynapseHubUserControl : UserControl
    {
        public readonly SynapseHubUserControlViewModel ViewModel;

        public SynapseHubUserControl()
        {
            InitializeComponent();
            ViewModel = new SynapseHubUserControlViewModel();
            DataContext = ViewModel;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenScript(sender as Tile);
        }
    }
}
