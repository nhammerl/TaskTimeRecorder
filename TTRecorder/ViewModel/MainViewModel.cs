using nhammerl.TTRecorder.Annotations;
using nhammerl.TTRecorder.Model;
using nhammerl.TTRecorder.ViewModel.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Popups;

namespace nhammerl.TTRecorder.ViewModel
{
    /// <summary>
    /// Main Logic from View.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Properties

        private readonly MainPage _mainPage;

        public ViewModelCommand PunchIn { get; set; }

        public ViewModelCommand CloseInputDialog { get; set; }

        public ViewModelCommand CloseDialog { get; set; }

        public ViewModelCommand CreateOutputFile { get; set; }

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
        public MainViewModel(MainPage mainPage)
        {
            _mainPage = mainPage;

            // Init Commands
            PunchIn = new ViewModelCommand
            {
                Command = new RelayCommand(r => ShowInputDialog = true),
                Text = "Punch-in!",
                ImagePath = @"Images/play.png"
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

            CreateOutputFile = new ViewModelCommand
            {
                Command = new RelayCommand(r => GenerateOutputFile()),
                Text = "Save to file",
                ImagePath = @"Images/save.png"
            };

            // Init Taskcollection.
            Tasks = new ObservableCollection<ITaskViewModel>();
        }

        /// <summary>
        /// Creat new task.
        /// </summary>
        public void CreateTask()
        {
            Tasks.Add(new DefaultTaskViewModel(new DefaultTaskModel(DialogInputValue), Tasks) { ItemVisualWidth = _mainPage.ActualWidth });
            ShowInputDialog = false;
        }

        public async void GenerateOutputFile()
        {
            var savePicker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = string.Format("TaskTimeRecords_{0}", DateTime.Now.ToString("yyyyMMdd"));

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);

                // write to file
                await FileIO.WriteTextAsync(file, BuildTaskInfo());

                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                var status = await CachedFileManager.CompleteUpdatesAsync(file);

                if (status == FileUpdateStatus.Complete)
                {
                    ShowMessage("File " + file.Name + " saved.");
                }
                else
                {
                    ShowMessage("File " + file.Name + " couldn't be saved.");
                }
            }
        }

        private string BuildTaskInfo()
        {
            var taskInfoBuilder = new StringBuilder();

            foreach (var task in Tasks)
            {
                taskInfoBuilder.AppendLine(
                    string.Format("######################### | {0} | #########################", task.TaskModel.Title));
                if (task.State == TaskState.Completed)
                {
                    taskInfoBuilder.AppendLine(string.Format("Total minutes: {0}", task.TaskModel.TotalMinutes));
                }
                else
                {
                    taskInfoBuilder.AppendLine(string.Format("Total minutes: -"));
                }
                taskInfoBuilder.AppendLine(string.Format("Start-End: {0} - {1}", task.TaskModel.Start, task.TaskModel.End));
                taskInfoBuilder.AppendLine(string.Format("State: {0}", task.State));

                if (task.TaskModel.Breaks.Any())
                {
                    int i = 0;
                    foreach (var taskBreak in task.TaskModel.Breaks)
                    {
                        i++;
                        taskInfoBuilder.AppendLine(string.Format("Break {0}: {1} minutes", i, string.Format("{0:0.##}", taskBreak.TotalMinutes)));
                    }
                }
            }

            return taskInfoBuilder.ToString();
        }

        private void ShowMessage(string message)
        {
            var dialogWindow = new MessageDialog(message);
            dialogWindow.ShowAsync();
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