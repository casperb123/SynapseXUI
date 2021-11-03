using System.ComponentModel;

namespace SynapseXUI.ViewModels
{
    public class ProgressWindowViewModel : INotifyPropertyChanged
    {
        private double progress;
        private string title;
        private string message;
        private bool buttonVisible;
        private bool buttonEnabled;

        public bool ButtonEnabled
        {
            get => buttonEnabled;
            set
            {
                buttonEnabled = value;
                OnPropertyChanged(nameof(ButtonEnabled));
            }
        }

        public bool ButtonVisible
        {
            get => buttonVisible;
            set
            {
                buttonVisible = value;
                OnPropertyChanged(nameof(ButtonVisible));
            }
        }

        public string Message
        {
            get => message;
            set
            {
                message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged(nameof(Progress));
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

        public ProgressWindowViewModel(string title, string message, bool buttonVisible)
        {
            Title = title;
            Message = message;
            ButtonVisible = buttonVisible;
        }
    }
}
