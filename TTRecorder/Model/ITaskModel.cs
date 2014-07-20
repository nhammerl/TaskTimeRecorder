using System;
using System.Collections.ObjectModel;

namespace nhammerl.TTRecorder.Model
{
    /// <summary>
    /// Interface for an basic task for the timerecroder.
    /// </summary>
    public interface ITaskModel
    {
        // Id of the task.
        Guid Id { get; }

        // Display title of task.
        string Title { get; set; }

        // Startdate of task.
        DateTime Start { get; set; }

        // Enddate of task.
        DateTime End { get; set; }

        // Collection of breaks.
        ObservableCollection<TimeSpan> Breaks { get; set; }

        // Helper to display the Count of total minutes.
        int TotalMinutes { get; }
    }
}