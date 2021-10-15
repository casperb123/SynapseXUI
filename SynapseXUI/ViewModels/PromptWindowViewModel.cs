using SynapseXUI.Entities;
using SynapseXUI.Windows;
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

        public PromptWindowViewModel(PromptWindow window, string title, string message, PromptType type)
        {
            Title = title;
            Message = message;
            Type = type;

            window.Topmost = App.SxOptions is null ? true : App.SxOptions.TopMost;

            if (type == PromptType.OK)
            {
                window.buttonOk.Focus();
            }
            else if (type == PromptType.YesNo)
            {
                window.buttonYes.Focus();
            }
        }
    }
}
