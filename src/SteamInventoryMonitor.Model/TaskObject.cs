using System;
using System.Collections.Generic;

namespace SteamInventoryMonitor.Model
{
    public class TaskObject
    {
        public event Action Updated;

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

        public void Remove(string uid, bool nf = false)
        {
            if(!nf)
                foreach (var item in Items)
                {
                    if (item.UID == uid)
                    {
                        Items.Remove(item);
                        Updated?.Invoke();
                        return;
                    }
                }
            else
                foreach (var item in ItemsNF)
                {
                    if (item.UID == uid)
                    {
                        ItemsNF.Remove(item);
                        Updated?.Invoke();
                        return;
                    }
                }
        }
        public void Update(string uid, int compareMethod, int compareArgument, bool nf = false)
        {
            if (!nf)
                foreach (var item in Items)
                {
                    if (item.UID == uid)
                    {
                        item.CompareMethod = compareMethod;
                        item.CompareArgument = compareArgument;

                        Updated?.Invoke();
                        return;
                    }
                }
            else
                foreach (var item in ItemsNF)
                {
                    if (item.UID == uid)
                    {
                        item.CompareMethod = compareMethod;
                        item.CompareArgument = compareArgument;

                        Updated?.Invoke();
                        return;
                    }
                }
        }
        public void Clear()
        {
            Items.Clear();
            ItemsNF.Clear();

            Updated?.Invoke();
        }
    }
}
