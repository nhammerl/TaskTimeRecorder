using nhammerl.TTRecorder.Model;
using nhammerl.TTRecorder.ViewModel.Command;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace nhammerl.TTRecorder.ViewModel
{
    /// <summary>
    /// TaskViewModel to handle some actions in View for a specified Task.
    /// </summary>
    public interface ITaskViewModel
    {
        // Core Task.
        ITaskModel TaskModel { get; set; }

        // Helper to display actual time.
        string ElapsedTime { get; set; }

        // Action, fired on break button.
        ICommandViewModel Break { get; set; }

        // Action, fired on PushOut button.
        ICommandViewModel PunshOut { get; set; }

        // Current state from Task.
        TaskState State { get; set; }

        // Color of border around the task
        Brush BorderBrush { get; set; }

        // Task enabled for actions?
        bool IsEnabled { get; set; }

        // Remove item from specified list if available
        ICommandViewModel DeleteFromList { get; set; }
    }
}