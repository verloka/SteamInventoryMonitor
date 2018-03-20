using System;
using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskObject
    {
        public List<TaskItem> ExistItems { get; set; }
        
        public List<Tuple<string, string, string, int, int>> NotExistsItems { get; set; }

        public TaskObject()
        {
            ExistItems = new List<TaskItem>();
            NotExistsItems = new List<Tuple<string, string, string, int, int>>();
        }
    }
}
