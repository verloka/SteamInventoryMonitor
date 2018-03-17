namespace SteamInventoryMonitor.Models
{
    public class UserID64
    {
        public UserIdResponse response { get; set; }

        public bool Success { get => response?.success == 1; }
        public string ID64 { get => response?.steamid; }
    }
}
