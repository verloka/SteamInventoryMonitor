using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskObject
    {
        public List<TaskItem> Items { get; set; }
        
        public List<TaskItem> ItemsNF { get; set; }

        public TaskObject()
        {
            Items = new List<TaskItem>();
            ItemsNF = new List<TaskItem>();
        }
    }
}
