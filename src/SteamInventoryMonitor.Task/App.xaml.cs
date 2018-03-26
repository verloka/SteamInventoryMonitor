using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SteamInventoryMonitor.Task
{
    public partial class App : Application
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        public static string LANGUAGE = "english";
        public static int NOTIFICATION_DELAY_S = 5;
        public static string TASK = $"{Directory.GetCurrentDirectory()}/Data/task.json";
        public static string IMG_URL = "https://steamcommunity-a.akamaihd.net/economy/image/";
        
        private const string MutexName = "SteamInventoryMonitor.Task";
        private readonly Mutex mutex;
        bool CreatedNew;

        public App()
        {
            mutex = new Mutex(true, MutexName, out CreatedNew);
            if (!CreatedNew)
                Current.Shutdown(0);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!CreatedNew)
                return;

            MainWindow mw = new MainWindow();
            mw.Show();
        }
    }
}
