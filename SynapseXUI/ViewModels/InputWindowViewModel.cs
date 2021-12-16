using SynapseXUI.Entities;
using SynapseXUI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseXUI.ViewModels
{
    public class InputWindowViewModel : INotifyPropertyChanged
    {
        private string title;
        private string message;
        private InputDataType type;
        private object input;

        public bool Focused { get; set; }

        public object Input
        {
            get => input;
            set
            {
                input = value;
                OnPropertyChanged(nameof(Input));
            }
        }

        public InputDataType Type
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

        public InputWindowViewModel(InputWindow window, string title, string message, object input, InputDataType type)
        {
            Title = title;
            Message = message;
            Input = input;
            Type = type;

            window.Topmost = App.SxOptions is null || App.SxOptions.TopMost;

            if (type == InputDataType.Text)
            {
                window.textBoxInput.Focus();
                TextBoxSelectAll(window);
            }
        }

        private async void TextBoxSelectAll(InputWindow window)
        {
            await Task.Delay(50);
            window.textBoxInput.SelectAll();
        }
    }
}
