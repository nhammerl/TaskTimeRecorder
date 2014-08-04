using nhammerl.TTRecorder.ViewModel;
using System;
using System.Collections.Generic;

namespace nhammerl.TTRecorder.Model.Data
{
    public interface IDataConnector
    {
        void InitDataBase();

        void SaveTask(ITaskModel task, TaskState state);

        void DeleteTask(Guid taskId);

        void UpdateTask(ITaskModel task, TaskState state);

        void LoadAllTasks();
    }
}