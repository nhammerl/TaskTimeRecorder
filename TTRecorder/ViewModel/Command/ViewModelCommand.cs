using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace nhammerl.TTRecorder.ViewModel.Command
{
    public class ViewModelCommand : ICommandViewModel, INotifyPropertyChanged
    {
        private Visibility _visibility;
        private ICommand _command;
        private string _imagePath;
        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }

        public ICommand Command
        {
            get { return _command; }
            set
            {
                _command = value;
                OnPropertyChanged("Command");
            }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}