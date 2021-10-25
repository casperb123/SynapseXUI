using System.Windows.Controls;

namespace SynapseXUI.UserControls
{
    /// <summary>
    /// Interaction logic for ScriptHubUserControl.xaml
    /// </summary>
    public partial class ScriptHubUserControl : UserControl
    {
        private readonly SynapseHubUserControl synapseUserControl;
        private readonly RbxScriptsHubUserControl rbxUserControl;
        private bool synapseLoaded;
        private bool rbxLoaded;

        public ScriptHubUserControl()
        {
            InitializeComponent();
            synapseUserControl = new SynapseHubUserControl();
            rbxUserControl = new RbxScriptsHubUserControl();
            tabItemSynapse.Content = synapseUserControl;
            tabItemRbx.Content = rbxUserControl;
        }

        private void TabControlHubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabItemSynapse.IsSelected && !synapseLoaded)
            {
                synapseUserControl.ViewModel.GetScripts();
                synapseLoaded = true;
            }
            else if (tabItemRbx.IsSelected && !rbxLoaded)
            {
                rbxUserControl.ViewModel.GetScripts();
                rbxLoaded = true;
            }
        }
    }
}
