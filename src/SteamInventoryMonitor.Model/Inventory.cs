using System.Collections.Generic;

namespace SteamInventoryMonitor.Models
{
    public class Inventory
    {
        public List<Item> assets { get; set; }
        public List<ItemDescription> descriptions { get; set; }
        public int more_items { get; set; }
        public string last_assetid { get; set; }
        public int total_inventory_count { get; set; }
        public int success { get; set; }
        public int rwgrsn { get; set; }

        public bool Success { get => success == 1; }
        public bool Next { get => more_items == 1; }
    }
}
