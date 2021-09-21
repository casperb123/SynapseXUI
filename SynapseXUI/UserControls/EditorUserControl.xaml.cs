using SynapseXUI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for EditorUserControl.xaml
    /// </summary>
    public partial class EditorUserControl : UserControl
    {
        public readonly EditorUserControlViewModel ViewModel;

        public static EditorUserControl Instance { get; set; }

        public EditorUserControl()
        {
            InitializeComponent();
            ViewModel = new EditorUserControlViewModel(this);
            DataContext = ViewModel;
        }

        private void MenuItemEditorsAddTab_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddTab();
        }

        private void TabItemEditorsAddTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ViewModel.AddTab();
        }
    }
}
