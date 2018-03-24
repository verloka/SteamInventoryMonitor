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
using System.Collections.Generic;
using Verloka.HelperLib.Settings;

namespace SteamInventoryMonitor.Task
{
    public partial class MainWindow : Window
    {
        public TaskObject TO { get; private set; }

        DispatcherTimer timer;
        List<TaskItem> Finded;

        RegSettings rs;

        public MainWindow()
        {
            rs = new RegSettings("SteamInventoryMonitor");
            App.NOTIFICATION_DELAY_S = rs.GetValue("NOTIFICATION_DELAY_S", 5);

            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, rs.GetValue("UpdateTimerDelay", 60));
            timer.Start();
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
        decimal RandomNumber(Random rnd, int precision, int scale)
        {
            if (rnd == null)
                throw new ArgumentNullException("randomNumberGenerator");
            if (!(precision >= 1 && precision <= 28))
                throw new ArgumentOutOfRangeException("precision", precision, "Precision must be between 1 and 28.");
            if (!(scale >= 0 && scale <= precision))
                throw new ArgumentOutOfRangeException("scale", precision, "Scale must be between 0 and precision.");

            Decimal d = 0m;
            for (int i = 0; i < precision; i++)
            {
                int r = rnd.Next(0, 10);
                d = d * 10m + r;
            }
            for (int s = 0; s < scale; s++)
            {
                d /= 10m;
            }
            return d;
        }
        public bool Pred(int value, int argument, int method)
        {
            switch (method)
            {
                case 0 when value == argument:
                    return true;
                case 1 when value != argument:
                    return true;
                case 2 when value > argument:
                    return true;
                case 3 when value >= argument:
                    return true;
                case 4 when value < argument:
                    return true;
                case 5 when value <= argument:
                    return true;
                default:
                    return false;
            }
        }
        public Task<bool> UpdateInformation()
        {
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                if (TO == null || TO.IsEmpty)
                    return false;

                var items = TO.Items.Concat(TO.ItemsNF);

                //group by ID64
                var id64 = from own in items
                           group own by own.OwnerID64 into ownGroup
                           select from i in ownGroup
                                  select i;

                foreach (var owner in id64)
                {
                    //group by AppID
                    var appid = from invs in owner
                                group invs by invs.AppId into invGroup
                                select from i in invGroup
                                       select i;


                    //parse Inventories
                    foreach (var inv in appid)
                    {
                        var d = SearchItems(inv);
                        d.Wait();
                        Finded.AddRange(d.Result);
                    }
                }

                return Finded.Count > 0;
            });
        }

        Task<List<TaskItem>> SearchItems(IEnumerable<TaskItem> items, string lid = "", int count = 5000)
        {
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                string lastid = lid;
                string tail = string.IsNullOrWhiteSpace(lastid) ? string.Empty : $"&start_assetid={lastid}";

                List<TaskItem> tis = new List<TaskItem>();

                string str = $"http://steamcommunity.com/inventory/{items.First().OwnerID64}/{items.First().AppId}/{items.First().AppContext}?l={App.LANGUAGE}&count={count}{tail}";

                using (WebClient wc = new WebClient())
                {
                    Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                    if (inv.Success)
                    {
                        foreach (var desc in inv.descriptions)
                            foreach (var item in items)
                            {
                                if (desc.name == item.Name)
                                {
                                    int amount = 0;

                                    foreach (var item2 in inv.assets)
                                        if (desc.classid == item2.classid)
                                            amount++;

                                    if (Pred(amount, item.CompareArgument, item.CompareMethod))
                                        tis.Add(item);
                                    
                                }
                            }


                        if (inv.Next)
                        {
                            var d = SearchItems(items.Except(tis), inv.last_assetid);
                            d.Wait();
                            return d.Result;
                        }
                    }
                }

                return tis;
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
            if (File.Exists(App.TASK))
                File.Delete(App.TASK);

            using (StreamWriter sw = File.CreateText(App.TASK))
                await sw.WriteLineAsync(JsonConvert.SerializeObject(TO));
        }
        private async void TimerTick(object sender, EventArgs e)
        {
            if (Finded != null)
                Finded.Clear();

            Finded = new List<TaskItem>();

            if (await UpdateInformation())
                if (Finded.Count > 1)
                {
                    Random rnd = new Random((int)DateTime.Now.Ticks);

                    (new NotificationWindow()
                    {
                        NotificationTitle = $"Items: [{Finded.Count}]",
                        NotificationMsg = "Your items was found! Enjoy, Dear!",
                        NotificationIcon = Finded[rnd.Next(Finded.Count)].IconUrl
                    }).Show();
                }
                else
                {
                    (new NotificationWindow()
                    {
                        NotificationTitle = Finded[0].Name,
                        NotificationMsg = "Your item was found! Enjoy, Dear!",
                        NotificationIcon = Finded[0].IconUrl
                    }).Show();
                }


            timer.Start();
        }
    }
}
