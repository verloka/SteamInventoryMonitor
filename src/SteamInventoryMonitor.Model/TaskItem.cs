using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskItem
    {
        public string Name { get; set; }
        public string AssetId { get; set; }
        public string AppId { get; set; }
        public int AppContext { get; set; }
        public string IconUrl { get; set; }

        public string OwnerID64 { get; set; }

        Dictionary<string, object> vars;

        public TaskItem()
        {
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
