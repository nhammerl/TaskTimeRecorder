using nhammerl.TTRecorder.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace nhammerl.TTRecorder.Model.Data
{
    public class XmlDatabaseConnector : IDataConnector
    {
        private readonly ObservableCollection<ITaskViewModel> _workingTasksList;
        private XmlDocument _rootDocument;
        private IStorageFile _databaseXmlFile;

        public XmlDatabaseConnector(ObservableCollection<ITaskViewModel> workingTasksList)
        {
            _workingTasksList = workingTasksList;
            InitDataBase();
        }

        /// <summary>
        /// Init the Connector. Create the File if not exist, or open and load it.
        /// </summary>
        public async void InitDataBase()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            _rootDocument = new XmlDocument();

            _databaseXmlFile = await localFolder.CreateFileAsync("TaskTimeRecorder.xml", CreationCollisionOption.OpenIfExists);
            var fileContent = await FileIO.ReadTextAsync(_databaseXmlFile);

            if (fileContent == "")
            {
                var rootElement = _rootDocument.CreateElement("TaskTimeRecorder");
                var tasksElement = _rootDocument.CreateElement("Tasks");

                rootElement.AppendChild(tasksElement);
                _rootDocument.AppendChild(rootElement);

                _rootDocument.SaveToFileAsync(_databaseXmlFile);
            }
            else
            {
                _rootDocument.LoadXml(fileContent);
            }
        }

        /// <summary>
        /// Save Task to Xml File.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskState"></param>
        public void SaveTask(ITaskModel task, TaskState taskState)
        {
            // build the root nodes
            var rootTask = _rootDocument.CreateElement("Task");

            // build the model nodes
            var startDate = _rootDocument.CreateElement("StartDate");
            startDate.InnerText = task.Start.Ticks.ToString();

            var endDate = _rootDocument.CreateElement("EndDate");
            endDate.InnerText = task.End.Ticks.ToString();

            var title = _rootDocument.CreateElement("Title");
            title.InnerText = task.Title;

            var id = _rootDocument.CreateElement("Id");
            id.InnerText = task.Id.ToString();

            var state = _rootDocument.CreateElement("State");
            state.InnerText = ((int)taskState).ToString();

            // Add new nodes to Task node and Tasknode to rootNode
            rootTask.AppendChild(startDate);
            rootTask.AppendChild(endDate);
            rootTask.AppendChild(title);
            rootTask.AppendChild(id);
            rootTask.AppendChild(state);

            _rootDocument.GetElementsByTagName("Tasks")[0].AppendChild(rootTask);

            _rootDocument.SaveToFileAsync(_databaseXmlFile);
        }

        /// <summary>
        /// Delete Xmlnode for specified task.
        /// </summary>
        /// <param name="taskId"></param>
        public void DeleteTask(Guid taskId)
        {
            var tasks = _rootDocument.GetElementsByTagName("Task");
            var taskToDelete = tasks.FirstOrDefault(ic => ic.ChildNodes.Any(c => c.InnerText == taskId.ToString()));

            _rootDocument.GetElementsByTagName("Tasks")[0].RemoveChild(taskToDelete);
            _rootDocument.SaveToFileAsync(_databaseXmlFile);
        }

        public void UpdateTask(ITaskModel task, TaskState state)
        {
            DeleteTask(task.Id);
            SaveTask(task, state);
        }

        public void LoadAllTasks()
        {
            var tasks = _rootDocument.GetElementsByTagName("Task");

            foreach (var xmlTask in tasks)
            {
                var title = xmlTask.ChildNodes.FirstOrDefault(c => c.NodeName == "Title").InnerText;
                var startDate = xmlTask.ChildNodes.FirstOrDefault(c => c.NodeName == "StartDate").InnerText;
                var endDate = xmlTask.ChildNodes.FirstOrDefault(c => c.NodeName == "EndDate").InnerText;
                var id = xmlTask.ChildNodes.FirstOrDefault(c => c.NodeName == "Id").InnerText;
                var state = xmlTask.ChildNodes.FirstOrDefault(c => c.NodeName == "State").InnerText;

                var taskModel = new DefaultTaskModel(title, new Guid(id))
                {
                    Start = new DateTime(Convert.ToInt64(startDate)),
                    End = new DateTime(Convert.ToInt64(endDate))
                };

                _workingTasksList.Add(new DefaultTaskViewModel(taskModel, _workingTasksList, this, (TaskState)Convert.ToInt32(state)));
            }
        }
    }
}