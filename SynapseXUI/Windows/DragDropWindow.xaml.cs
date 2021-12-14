using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;

namespace SynapseXUI.Windows
{
    /// <summary>
    /// Interaction logic for DragDropWindow.xaml
    /// </summary>
    public partial class DragDropWindow : MetroWindow
    {
        private readonly DragDropWindowViewModel viewModel;

        public DragDropWindow(string header, bool isSelected)
        {
            InitializeComponent();
            viewModel = new DragDropWindowViewModel(header, isSelected);
            DataContext = viewModel;
        }
    }
}
