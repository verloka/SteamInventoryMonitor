using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskObject
    {
        public bool IsEmpty
        {
            get => Items.Count == 0 && ItemsNF.Count == 0;
        }

        public List<TaskItem> Items { get; set; }
        public List<TaskItem> ItemsNF { get; set; }

        public TaskObject()
        {
            Items = new List<TaskItem>();
            ItemsNF = new List<TaskItem>();
        }
    }
}
