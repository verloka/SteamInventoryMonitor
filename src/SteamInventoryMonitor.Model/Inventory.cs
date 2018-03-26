using Newtonsoft.Json;
using System.Collections.Generic;

namespace SteamInventoryMonitor.Models
{
    public class Inventory
    {
        [JsonProperty(PropertyName = "assets")]
        public List<Item> Assets { get; set; }

        [JsonProperty(PropertyName = "descriptions")]
        public List<ItemDescription> Descriptions { get; set; }

        [JsonProperty(PropertyName = "more_items")]
        public int MoreItems { get; set; }

        [JsonProperty(PropertyName = "last_assetid")]
        public string LastAssteId { get; set; }

        [JsonProperty(PropertyName = "total_inventory_count")]
        public int InventoryCount { get; set; }

        [JsonProperty(PropertyName = "success")]
        public int Success { get; set; }

        public bool IsSuccess { get => Success == 1; }
        public bool IsNext { get => MoreItems == 1; }
    }
}
