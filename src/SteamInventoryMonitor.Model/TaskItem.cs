using System;
using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskItem
    {
        public string UID { get; set; }
        public string Name { get; set; }
        public string ClassId { get; set; }
        public string AppId { get; set; }
        public int AppContext { get; set; }
        public string IconUrl { get; set; }

        public string OwnerID64 { get; set; }
        public string OwnerName { get; set; }
        public string OwnerAvatar { get; set; }
        public int CompareMethod { get; set; }
        public int CompareArgument { get; set; }

        Dictionary<string, object> vars;

        public TaskItem()
        {
            UID = Guid.NewGuid().ToString();
            vars = new Dictionary<string, object>();
        }

        public void SetVar(string name, object obj) => vars.Add(name, obj);
        public T GetVar<T>(string name)
        {
            try
            {
                return (T)vars[name];
            }
            catch
            {
                return default(T);
            }
        }
    }
}
