using System;
using System.Collections.Generic;

namespace nhammerl.TTRecorder.Model.Data
{
    public interface IDataConnector
    {
        void InitDataBase();

        void SaveTask(ITaskModel task);

        void DeleteTask(Guid taskId);

        IEnumerable<ITaskModel> GetAllTasks();
    }
}