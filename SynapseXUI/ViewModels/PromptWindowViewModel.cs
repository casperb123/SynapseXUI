using SynapseXUI.Entities;
using System.ComponentModel;

namespace SynapseXUI.ViewModels
{
    public class PromptWindowViewModel : INotifyPropertyChanged
    {
        private string title;
        private string message;
        private PromptType type;

        public PromptType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (!string.IsNullOrWhiteSpace(prop))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }

        public PromptWindowViewModel(string title, string message, PromptType type)
        {
            Title = title;
            Message = message;
            Type = type;
        }
    }
}
