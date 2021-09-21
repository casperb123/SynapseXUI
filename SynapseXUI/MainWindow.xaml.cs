using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainWindowViewModel viewModel;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            viewModel = new MainWindowViewModel(this);
            DataContext = viewModel;
        }
    }
}
