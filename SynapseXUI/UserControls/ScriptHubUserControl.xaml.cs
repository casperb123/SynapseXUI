using System.Windows.Controls;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for ScriptHubUserControl.xaml
    /// </summary>
    public partial class ScriptHubUserControl : UserControl
    {
        public ScriptHubUserControl()
        {
            InitializeComponent();
        }

        public void InitiateScriptHubs()
        {
            userControlSynapseHub.Content = new SynapseHubUserControl();
            userControlRbxScripts.Content = new RbxScriptsHubUserControl();
        }
    }
}
