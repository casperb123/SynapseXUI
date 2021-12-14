using System.ComponentModel;

namespace SynapseXUI.ViewModels
{
    public class DragDropWindowViewModel : INotifyPropertyChanged
    {
        private string header;
        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public DragDropWindowViewModel(string header, bool isSelected)
        {
            Header = header;
            IsSelected = isSelected;
        }
    }
}
