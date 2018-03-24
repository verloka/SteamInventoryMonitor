using Newtonsoft.Json;
using SteamInventoryMonitor.Model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using SteamInventoryMonitor.Models;
using System.Windows.Threading;

namespace SteamInventoryMonitor.Task
{
    public partial class MainWindow : Window
    {
        public TaskObject TO { get; private set; }

        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 60);
            //timer.Start();
        }

        public Task<bool> Parsing()
        {
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    TO = File.Exists(App.TASK) ? JsonConvert.DeserializeObject<TaskObject>(File.ReadAllText(App.TASK)) : new TaskObject();

                    if (TO.IsEmpty)
                        return false;

                    var items = TO.Items.Concat(TO.ItemsNF);

                    var owners = from own in items
                                 group own by own.OwnerID64 into ownGroup
                                 select new
                                 {
                                     ID64 = ownGroup.Key,
                                     OWNERS = from i in ownGroup
                                              select i
                                 };

                    foreach (var owner in owners)
                    {
                        var inventories = from invs in owner.OWNERS
                                          group invs by invs.AppId into invGroup
                                          select from i in invGroup
                                                 select i;

                        foreach (var inv in inventories)
                        {
                            foreach (var item in inv)
                            {
                                var b = SearchItem(owner.ID64, item.Name, item.AppId, item.AppContext).Result;

                                return b.Item1 && b.Item2;
                            }
                        }
                    }

                    return false;
                });
        }
        public void SetupViewMode(int modeNumber)
        {
            switch (modeNumber)
            {
                case 1:
                default:
                    Width = 800;
                    Height = 480;
                    frame.Navigate(new Uri("Views/TaskLine.xaml", UriKind.Relative));
                    break;
                case 2:
                    Width = 640;
                    Height = 540;
                    frame.Navigate(new Uri("Views/NotificationEvent.xaml", UriKind.Relative));
                    break;
            }
        }

        Task<Tuple<bool, bool>> SearchItem(string id64, string name, string appid, int appcontxt, string lid = "", int count = 5000)
        {
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string lastid = lid;
                string tail = string.IsNullOrWhiteSpace(lastid) ? string.Empty : $"&start_assetid={lastid}";

                string str = $"http://steamcommunity.com/inventory/{id64}/{appid}/{appcontxt}?l={App.LANGUAGE}&count={count}{tail}";

                using (WebClient wc = new WebClient())
                {
                    Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                    if (inv.Success)
                    {
                        foreach (var item in inv.descriptions)
                            if (item.name == name)
                            {
                                int amount = 0;

                                foreach (var item2 in inv.assets)
                                    if (item.classid == item2.classid)
                                        amount++;

                                //TODO

                                return new Tuple<bool, bool>(true, true);
                            }
                        if (inv.Next)
                            return SearchItem(id64, name, appid, appcontxt, inv.last_assetid).Result;
                        else
                            return new Tuple<bool, bool>(true, false);
                    }
                    else
                        return new Tuple<bool, bool>(false, false);
                }
            });
        }

        #region Window Events
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }
        private void btnCloseWinodwClick(object sender, RoutedEventArgs e) => Close();
        #endregion

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW = this;

            TO = File.Exists(App.TASK) ? JsonConvert.DeserializeObject<TaskObject>(File.ReadAllText(App.TASK)) : new TaskObject();
            TO.Updated += TOUpdated;

            SetupViewMode(1);
        }

        private async void TOUpdated()
        {
            using (StreamWriter sw = File.CreateText(App.TASK))
                await sw.WriteLineAsync(JsonConvert.SerializeObject(TO));
        }
        private async void TimerTick(object sender, EventArgs e)
        {
            bool b = await Parsing();

            if (b)
                (new NotificationWindow() { NotificationTitle = "Hi!", NotificationMsg = "Your item was found! Enjoy, Dear!" }).Show();

            timer.Start();
        }
    }
}
