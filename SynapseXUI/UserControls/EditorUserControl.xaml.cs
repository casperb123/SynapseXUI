using SynapseXUI.ViewModels;
using System.Windows.Controls;

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
    }
}
