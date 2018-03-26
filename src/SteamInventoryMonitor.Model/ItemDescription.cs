namespace SteamInventoryMonitor.Models
{
    public class ItemDescription
    {
        public int appid { get; set; }
        public string classid { get; set; }
        public string icon_url { get; set; }
        public string icon_url_large { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int tradable { get; set; }
        public int marketable { get; set; }

    }
}
