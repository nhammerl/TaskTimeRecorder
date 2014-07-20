using nhammerl.TTRecorder.Annotations;
using nhammerl.TTRecorder.Model;
using nhammerl.TTRecorder.ViewModel.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nhammerl.TTRecorder.ViewModel
{
    /// <summary>
    /// Main Logic from View.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Properties

        public ViewModelCommand PunchIn { get; set; }

        public ViewModelCommand CloseInputDialog { get; set; }

        public ViewModelCommand CloseDialog { get; set; }

        public ObservableCollection<ITaskViewModel> Tasks { get; set; }

        private bool _showInputDialog;

        public bool ShowInputDialog
        {
            get { return _showInputDialog; }
            set
            {
                _showInputDialog = value;
                OnPropertyChanged();

                if (_showInputDialog == false)
                {
                    DialogInputValue = "";
                }
            }
        }

        private string _dialogInputValue;

        public string DialogInputValue
        {
            get { return _dialogInputValue; }
            set
            {
                _dialogInputValue = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public MainViewModel()
        {
            // Init Commands
            PunchIn = new ViewModelCommand
            {
                Command = new RelayCommand(r => ShowInputDialog = true),
                Text = "Punch-in!"
            };

            CloseInputDialog = new ViewModelCommand
            {
                Command = new RelayCommand(r => CreateTask()),
                Text = "Create task"
            };

            CloseDialog = new ViewModelCommand
            {
                Command = new RelayCommand(
                    r =>
                    {
                        ShowInputDialog = false;
                    }),
                Text = "X"
            };

            // Init Taskcollection.
            Tasks = new ObservableCollection<ITaskViewModel>();
        }

        /// <summary>
        /// Creat new task.
        /// </summary>
        public void CreateTask()
        {
            Tasks.Add(new DefaultTaskViewModel(new DefaultTaskModel(DialogInputValue), Tasks));
            ShowInputDialog = false;
        }

        /// <summary>
        /// Show a messagedialog
        /// </summary>
        /// <param name="message">message to display</param>
        public void ShowMessage(string message)
        {
            ShowInputDialog = true;
        }

        #region PropertyChanged helper

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) { handler(this, new PropertyChangedEventArgs(propertyName)); }
        }

        #endregion PropertyChanged helper
    }
}