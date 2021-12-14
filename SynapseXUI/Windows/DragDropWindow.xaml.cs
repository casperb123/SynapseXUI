using MahApps.Metro.Controls;
using SynapseXUI.ViewModels;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace SynapseXUI.Windows
{
    /// <summary>
    /// Interaction logic for DragDropWindow.xaml
    /// </summary>
    public partial class DragDropWindow : MetroWindow
    {
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        private readonly DragDropWindowViewModel viewModel;

        public DragDropWindow(string header, bool isSelected)
        {
            InitializeComponent();
            viewModel = new DragDropWindowViewModel(header, isSelected);
            DataContext = viewModel;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);

            base.OnSourceInitialized(e);
        }
    }
}
