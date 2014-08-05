using nhammerl.TTRecorder.Annotations;
using nhammerl.TTRecorder.Model;
using nhammerl.TTRecorder.Model.Data;
using nhammerl.TTRecorder.ViewModel.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace nhammerl.TTRecorder.ViewModel
{
    public class DefaultTaskViewModel : ITaskViewModel, INotifyPropertyChanged
    {
        private DateTime _breakStart;
        private DateTime _breakEnd;
        private bool _taskOnBreak;
        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<ITaskViewModel> _targetList;
        private readonly IDataConnector _dataConnector;
        private readonly bool _initLoad = true;

        private ITaskModel _taskModel;

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

        private string _elapsedTime;

        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                _elapsedTime = value;
                OnPropertyChanged();
            }
        }

        private ICommandViewModel _break;

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

        private ICommandViewModel _punchOut;

        public ICommandViewModel PunchOut
        {
            get { return _punchOut; }
            set
            {
                _punchOut = value;
                OnPropertyChanged();
            }
        }

        private TaskState _state;

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

        private Brush _borderBrush;

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

        private bool _isEnabled { get; set; }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        private ICommandViewModel _deleteFromList;

        public ICommandViewModel DeleteFromList
        {
            get { return _deleteFromList; }
            set
            {
                _deleteFromList = value;
                OnPropertyChanged();
            }
        }

        private double _itemVisualWidth;

        public double ItemVisualWidth
        {
            get { return _itemVisualWidth; }
            set
            {
                _itemVisualWidth = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public DefaultTaskViewModel(ITaskModel taskModel, ObservableCollection<ITaskViewModel> targetList, IDataConnector dataConnector)
        {
            _taskOnBreak = false;
            TaskModel = taskModel;
            this._targetList = targetList;
            _dataConnector = dataConnector;

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

            _timer = new DispatcherTimer();
            _timer.Tick += timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();

            State = TaskState.Running;
            _initLoad = false;
        }

        public DefaultTaskViewModel(ITaskModel taskModel, ObservableCollection<ITaskViewModel> targetList, IDataConnector dataConnector, TaskState state)
        {
            _taskOnBreak = false;
            TaskModel = taskModel;
            this._targetList = targetList;
            _dataConnector = dataConnector;

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

            _timer = new DispatcherTimer();
            _timer.Tick += timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Start();

            State = state;
            _initLoad = false;
        }

        /// <summary>
        /// Save Task to List of target and to connector.
        /// </summary>
        public void SaveTaskToList()
        {
            _targetList.Add(this);
            _dataConnector.SaveTask(TaskModel, State);
        }

        private void timer_Tick(object sender, object e)
        {
            if (!_taskOnBreak)
            {
                var pausedMinutes = TaskModel.Breaks.Sum(b => b.TotalSeconds);

                ElapsedTime = TimeSpan.FromSeconds((((DateTime.Now - TaskModel.Start).TotalSeconds) - pausedMinutes)).ToString(@"hh\:mm\:ss");
            }
        }

        /// <summary>
        /// Break current task.
        /// </summary>
        private void BreakTask()
        {
            if (_taskOnBreak)
            {
                _breakEnd = DateTime.Now;
                _taskOnBreak = false;

                var breakSpan = _breakEnd - _breakStart;

                TaskModel.Breaks.Add(breakSpan);

                State = TaskState.Running;
                Break.Text = "Break";
            }
            else
            {
                _breakStart = DateTime.Now;
                _taskOnBreak = true;
                State = TaskState.OnBreak;
                Break.Text = "Go on";
            }
        }

        /// <summary>
        /// Finish the task.
        /// </summary>
        private void FinishTask()
        {
            if (_taskOnBreak)
            {
                BreakTask();
            }

            TaskModel.End = DateTime.Now;
            State = TaskState.Completed;
        }

        private void ReopenTask()
        {
            State = TaskState.Running;
            PunchOut.Command = new RelayCommand(r => FinishTask());
            TaskModel.End = new DateTime();
            PunchOut.Text = "Finished";
            PunchOut.ImagePath = @"Images/finish.png";
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