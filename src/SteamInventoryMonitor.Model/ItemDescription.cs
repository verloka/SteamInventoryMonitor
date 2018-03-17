using System.Collections.Generic;

namespace SteamInventoryMonitor.Models
{
    public class ItemDescription
    {
        public int appid { get; set; }
        public string classid { get; set; }
        public string instanceid { get; set; }
        public int currency { get; set; }
        public string icon_url { get; set; }
        public string icon_url_large { get; set; }
        public string icon_drag_url { get; set; }
        public string name { get; set; }
        public string market_hash_name { get; set; }
        public string market_name { get; set; }
        public object name_color { get; set; }
        public object background_color { get; set; }
        public string type { get; set; }
        public int tradable { get; set; }
        public int marketable { get; set; }
        public int commodity { get; set; }
        public int market_fee_app { get; set; }
        public int market_tradable_restriction { get; set; }
        public int market_marketable_restriction { get; set; }

        public List<ItemTag> tags { get; set; }
        public List<Description> descriptions { get; set; }

    }
}
