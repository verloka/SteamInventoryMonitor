using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskObject
    {
        public List<TaskItem> ExistItems { get; set; }
        public List<KeyValuePair<string, string>> NotExistsItems { get; set; }

        public TaskObject()
        {
            ExistItems = new List<TaskItem>();
            NotExistsItems = new List<KeyValuePair<string, string>>();
        }
    }
}
