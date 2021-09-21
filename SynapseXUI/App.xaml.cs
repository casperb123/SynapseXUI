using System.IO;
using System.Reflection;
using System.Windows;

namespace SynapseXUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string StartUpPath { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            StartUpPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            base.OnStartup(e);
        }
    }
}
