using System;
using System.Collections.Generic;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace nhammerl.TTRecorder.Model.Data
{
    public class XmlDatabaseConnector : IDataConnector
    {
        private XmlDocument _rootDocument;

        public async void InitDataBase()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            _rootDocument = new XmlDocument();

            var databaseFile = await localFolder.CreateFileAsync("TaskTimeRecorder.xml", CreationCollisionOption.OpenIfExists);
            var fileContent = await FileIO.ReadTextAsync(databaseFile);

            if (fileContent == "")
            {
                var rootElement = _rootDocument.CreateElement("TaskTimeRecorder");
                var tasksElement = _rootDocument.CreateElement("Tasks");

                rootElement.AppendChild(tasksElement);
                _rootDocument.AppendChild(rootElement);

                _rootDocument.SaveToFileAsync(databaseFile);
            }
            else
            {
                _rootDocument.LoadXml(fileContent);
            }
        }

        public void SaveTask(ITaskModel task)
        {
        }

        public void DeleteTask(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITaskModel> GetAllTasks()
        {
            throw new NotImplementedException();
        }
    }
}