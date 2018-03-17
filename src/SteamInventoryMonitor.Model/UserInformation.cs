namespace SteamInventoryMonitor.Models
{
    public class UserInformation
    {
        public bool Success { get => response?.players.Count == 1; }
        public Player Player { get => response?.players[0]; }
        public PlayerResponse response { get; set; }
    }
}
