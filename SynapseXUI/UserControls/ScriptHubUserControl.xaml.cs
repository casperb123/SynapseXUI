using MahApps.Metro.Controls;
using sxlib.Specialized;
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
    /// Interaction logic for ScriptHubUserControl.xaml
    /// </summary>
    public partial class ScriptHubUserControl : UserControl
    {
        public readonly ScriptHubUserControlViewModel ViewModel;

        public ScriptHubUserControl()
        {
            InitializeComponent();
            ViewModel = new ScriptHubUserControlViewModel(this);
            DataContext = ViewModel;
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenScript(sender as Tile);
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ExecuteScript();
        }
    }
}
