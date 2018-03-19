using System.IO;
using System.Windows;

namespace SteamInventoryMonitor
{
    public partial class App : Application
    {
        public static MainWindow MAIN_WINDOW;

        public static string KEY = "DC57CD83BCD9D34E6F183F8013F26D90";
        public static string LANGUAGE = "english";
        public static string IMG_URL = "https://steamcommunity-a.akamaihd.net/economy/image/";
        public static string INVENTORIES = $"{Directory.GetCurrentDirectory()}/Data/Inventories.json";
        public static string TASK = $"{Directory.GetCurrentDirectory()}/Data/task.json";

        public static string ID64 = "";
    }
}
