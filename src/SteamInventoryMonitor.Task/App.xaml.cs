using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SteamInventoryMonitor.Task
{
    public partial class App : Application
    {
        public static MainWindow MAIN_WINDOW;

        public static string LANGUAGE = "english";
        public static string TASK = $"{Directory.GetCurrentDirectory()}/Data/task.json";
        public static string IMG_URL = "https://steamcommunity-a.akamaihd.net/economy/image/";
    }
}
