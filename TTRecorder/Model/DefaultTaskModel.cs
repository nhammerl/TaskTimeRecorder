using nhammerl.TTRecorder.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace nhammerl.TTRecorder.Model
{
    /// <summary>
    /// The default Taskmodel for timerecorder.
    /// </summary>
    public class DefaultTaskModel : ITaskModel, INotifyPropertyChanged
    {
        #region Properties

        // Id of task.
        private readonly Guid _id;

        public Guid Id
        {
            get { return _id; }
        }

        // Displayed title of task.
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        // Task startdate.
        private DateTime _start;

        public DateTime Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalMinutes");
            }
        }

        // Task endddate.
        private DateTime _end;

        public DateTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalMinutes");
            }
        }

        // Collection of breaks.
        private ObservableCollection<TimeSpan> _breaks;

        public ObservableCollection<TimeSpan> Breaks
        {
            get { return _breaks; }
            set
            {
                _breaks = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalMinutes");
            }
        }

        // Helper for easy display or calculation of TotalMinutes.
        public int TotalMinutes
        {
            get
            {
                var pausedMinutes = Breaks.Sum(b => b.TotalMinutes);

                return Convert.ToInt32((End - Start).TotalMinutes - pausedMinutes);
            }
        }

        #endregion Properties

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public DefaultTaskModel(string title)
        {
            _id = Guid.NewGuid();
            Start = DateTime.Now;
            Title = title;
            Breaks = new ObservableCollection<TimeSpan>();
        }

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public DefaultTaskModel(string title, Guid id)
        {
            _id = id;
            Start = DateTime.Now;
            Title = title;
            Breaks = new ObservableCollection<TimeSpan>();
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