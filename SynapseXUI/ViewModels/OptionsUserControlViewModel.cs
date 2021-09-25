using SynapseXUI.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseXUI.ViewModels
{
    public class OptionsUserControlViewModel : INotifyPropertyChanged
    {
        private readonly OptionsUserControl userControl;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public OptionsUserControlViewModel(OptionsUserControl userControl)
        {
            this.userControl = userControl;
        }
    }
}
