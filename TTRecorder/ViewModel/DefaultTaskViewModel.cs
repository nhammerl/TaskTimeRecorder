using nhammerl.TTRecorder.Annotations;
using nhammerl.TTRecorder.Model;
using nhammerl.TTRecorder.Model.Data;
using nhammerl.TTRecorder.ViewModel.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace nhammerl.TTRecorder.ViewModel
{
    public class DefaultTaskViewModel : ITaskViewModel, INotifyPropertyChanged
    {
        #region private properties

        private readonly IDataConnector _dataConnector;
        private readonly bool _initLoad = true;
        private readonly ObservableCollection<ITaskViewModel> _targetList;
        private readonly DispatcherTimer _timer;
        private Brush _borderBrush;
        private ICommandViewModel _break;
        private DateTime _breakEnd;
        private DateTime _breakStart;
        private ICommandViewModel _deleteFromList;
        private double _itemVisualWidth;
        private ICommandViewModel _punchOut;
        private TaskState _state;
        private ITaskModel _taskModel;

        #endregion private properties

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="taskModel"></param>
        /// <param name="targetList"></param>
        /// <param name="dataConnector"></param>
        /// <param name="mainPage"></param>
        /// <param name="state"></param>
        public DefaultTaskViewModel(ITaskModel taskModel, ObservableCollection<ITaskViewModel> targetList, IDataConnector dataConnector, MainPage mainPage, TaskState state)
        {
            TaskModel = taskModel;
            _targetList = targetList;
            _dataConnector = dataConnector;
            ItemVisualWidth = mainPage.ActualWidth;

            // Commands
            Break = new ViewModelCommand()
            {
                Command = new RelayCommand(r => BreakTask()),
                Text = "Break"
            };

            PunchOut = new ViewModelCommand
            {
                Command = new RelayCommand(r => FinishTask()),
                Text = "Finished",
                ImagePath = @"Images/finish.png"
            };

            DeleteFromList = new ViewModelCommand
            {
                Command = new RelayCommand(
                    r =>
                    {
                        if (targetList != null && targetList.Contains(this))
                        {
                            targetList.Remove(this);
                            _dataConnector.DeleteTask(taskModel.Id);
                        }
                    }),
                Text = "Delete",
                ImagePath = "Images/delete.png"
            };

            // Timer Init
            _timer = new DispatcherTimer();
            _timer.Tick += timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();

            // Setstate
            State = state;
            _initLoad = false;
        }

        public Brush BorderBrush
        {
            get
            {
                return _borderBrush;
            }
            set
            {
                _borderBrush = value;
                OnPropertyChanged();
            }
        }

        public ICommandViewModel Break
        {
            get
            {
                return _break;
            }
            set
            {
                _break = value;
                OnPropertyChanged();
            }
        }

        public ICommandViewModel DeleteFromList
        {
            get { return _deleteFromList; }
            set
            {
                _deleteFromList = value;
                OnPropertyChanged();
            }
        }

        public string ElapsedTime
        {
            get
            {
                var totalSeconds = TaskModel.Breaks.Sum(b => b.TotalSeconds);
                var result = "";

                switch (State)
                {
                    case TaskState.Completed:
                        result = TimeSpan.FromSeconds((((TaskModel.End - TaskModel.Start).TotalSeconds) - totalSeconds)).ToString(@"hh\:mm\:ss");
                        break;

                    case TaskState.Running:
                        result = TimeSpan.FromSeconds((((DateTime.Now - TaskModel.Start).TotalSeconds) - totalSeconds)).ToString(@"hh\:mm\:ss");
                        break;

                    case TaskState.OnBreak:
                        result = TimeSpan.FromSeconds((((TaskModel.LastBreak - TaskModel.Start).TotalSeconds) - totalSeconds)).ToString(@"hh\:mm\:ss");
                        break;
                }

                return result;
            }
            set
            {
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public double ItemVisualWidth
        {
            get { return _itemVisualWidth; }
            set
            {
                _itemVisualWidth = value;
                OnPropertyChanged();
            }
        }

        public ICommandViewModel PunchOut
        {
            get { return _punchOut; }
            set
            {
                _punchOut = value;
                OnPropertyChanged();
            }
        }

        public TaskState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                OnPropertyChanged();

                switch (_state)
                {
                    case TaskState.Completed:
                        BorderBrush = new SolidColorBrush(Colors.Green) { Opacity = 0.5 };
                        IsEnabled = false;

                        Break.ImagePath = @"Images/finish.png";
                        Break.Text = "Break";

                        PunchOut.Text = "Reopen Task";
                        PunchOut.ImagePath = @"Images/reopen.png";
                        PunchOut.Command = new RelayCommand(r => ReopenTask());
                        _timer.Stop();

                        break;

                    case TaskState.OnBreak:
                        BorderBrush = new SolidColorBrush(Colors.Red) { Opacity = 0.5 };
                        IsEnabled = true;
                        Break.ImagePath = @"Images/play.png";
                        Break.Text = "Go on";
                        _timer.Stop();

                        if (_initLoad)
                        {
                            var pausedMinutes = TaskModel.Breaks.Sum(b => b.TotalSeconds);

                            ElapsedTime = TimeSpan.FromSeconds((((TaskModel.LastBreak - TaskModel.Start).TotalSeconds) - pausedMinutes)).ToString(@"hh\:mm\:ss");

                            _breakStart = TaskModel.LastBreak;
                        }
                        else
                        {
                            TaskModel.LastBreak = DateTime.Now;
                        }
                        break;

                    case TaskState.Running:
                        BorderBrush = new SolidColorBrush(Colors.Yellow) { Opacity = 0.5 };
                        IsEnabled = true;
                        Break.ImagePath = @"Images/break.png";
                        _timer.Start();
                        break;
                }
                if (!_initLoad)
                {
                    _dataConnector.UpdateTask(TaskModel, value);
                }
                else if (_state == TaskState.Completed || _state == TaskState.OnBreak)
                {
                    timer_Tick(null, null);
                }
            }
        }

        public ITaskModel TaskModel
        {
            get
            {
                return _taskModel;
            }
            set
            {
                _taskModel = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled { get; set; }

        /// <summary>
        /// Save Task to List of target and to connector.
        /// </summary>
        public void SaveTaskToList()
        {
            _targetList.Add(this);
            _dataConnector.SaveTask(TaskModel, State);
        }

        public void UpdateTaskInfosToXml()
        {
            _dataConnector.UpdateTask(TaskModel, State);
        }

        /// <summary>
        /// Break current task.
        /// </summary>
        private void BreakTask()
        {
            if (State == TaskState.OnBreak)
            {
                _breakEnd = DateTime.Now;

                var breakSpan = _breakEnd - _breakStart;

                TaskModel.Breaks.Add(breakSpan);

                State = TaskState.Running;
                Break.Text = "Break";
            }
            else
            {
                _breakStart = DateTime.Now;
                State = TaskState.OnBreak;
                Break.Text = "Go on";
            }
        }

        /// <summary>
        /// Finish the task.
        /// </summary>
        private void FinishTask()
        {
            if (State == TaskState.OnBreak)
            {
                BreakTask();
            }

            TaskModel.End = DateTime.Now;
            TaskModel.LastBreak = DateTime.Now;
            State = TaskState.Completed;
        }

        /// <summary>
        /// Reopen Task
        /// </summary>
        private void ReopenTask()
        {
            TaskModel.Breaks.Add(DateTime.Now - TaskModel.End);
            State = TaskState.Running;
            PunchOut.Command = new RelayCommand(r => FinishTask());
            TaskModel.End = new DateTime();
            PunchOut.Text = "Finished";
            PunchOut.ImagePath = @"Images/finish.png";
        }

        /// <summary>
        /// timer action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, object e)
        {
            if (State != TaskState.OnBreak)
            {
                OnPropertyChanged("ElapsedTime");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) { handler(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}