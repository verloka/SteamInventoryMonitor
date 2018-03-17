namespace SteamInventoryMonitor.Models
{
    public class Item
    {
        public int id { get; set; }
        public int contextid { get; set; }
        public string assetid { get; set; }
        public string classid { get; set; }
        public string instanceid { get; set; }
        public int amount { get; set; }
    }
}
