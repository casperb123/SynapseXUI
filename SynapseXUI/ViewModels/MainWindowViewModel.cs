using SynapseXUI.UserControls;

namespace SynapseXUI.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly MainWindow mainWindow;
        private readonly EditorUserControl editorUserControl;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            editorUserControl = new EditorUserControl();
            mainWindow.userControlEditor.Content = editorUserControl;
        }
    }
}
