using Newtonsoft.Json;
using SteamInventoryMonitor.Controlls;
using SteamInventoryMonitor.Model;
using SteamInventoryMonitor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SteamInventoryMonitor.Views
{
    public partial class ProfilePage : Page
    {
        public Player Player { get; set; }
        public List<InventoryObject> Inventories { get; set; }
        public TaskItem SelectedItem { get; set; }

        string searchAppid = "753"; //steam
        int searchAppcontext = 6;   //2 - games item, 6 - steam items (bg, emotion, cards, gems)
        int searchCount = 5000;

        public ProfilePage()
        {
            InitializeComponent();
        }

        Task<bool> GetUserInformation()
        {
            return Task.Factory.StartNew(() =>
           {
               string url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={App.KEY}&steamids={App.ID64}";

               using (WebClient wc = new WebClient())
               {
                   UserInformation ui = JsonConvert.DeserializeObject<UserInformation>(wc.DownloadString(url));
                   if (ui.Success)
                   {
                       Player = ui.Player;
                       return true;
                   }
                   return false;
               }
           });
        }
        Task<int> GetInventoryItemsCount(string appid, int appcontext)
        {
            return Task.Factory.StartNew(() =>
            {
                int count = 1, rezult = 0;

                try
                {
                    string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

                    using (WebClient wc = new WebClient())
                    {
                        Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                        if (inv.IsSuccess)
                            rezult = inv.InventoryCount;
                    }
                }
                catch { rezult = -1; }

                return rezult;
            });
        }
        Task<Tuple<bool, bool>> SearchItem(string name, string lid = "")
        {
            return Task.Factory.StartNew(() =>
            {
                string lastid = lid;
                string tail = string.IsNullOrWhiteSpace(lastid) ? string.Empty : $"&start_assetid={lastid}";

                string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{searchAppid}/{searchAppcontext}?l={App.LANGUAGE}&count={searchCount}{tail}";

                using (WebClient wc = new WebClient())
                {
                    Inventory inv;

                    try { inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str)); }
                    catch { return new Tuple<bool, bool>(false, false); }

                    if (inv.IsSuccess)
                    {
                        foreach (var item in inv.Descriptions)
                            if (item.name == name)
                            {
                                SelectedItem = new TaskItem
                                {
                                    Name = item.name,
                                    AppId = searchAppid,
                                    AppContext = searchAppcontext,
                                    ClassId = item.classid,
                                    IconUrl = $"{App.IMG_URL}{item.icon_url}",
                                    OwnerID64 = App.ID64,
                                    OwnerName = Player.personaname,
                                    OwnerAvatar = Player.avatarmedium
                                };

                                SelectedItem.SetVar("type", item.type);
                                SelectedItem.SetVar("marketable", item.marketable);
                                SelectedItem.SetVar("tradable", item.tradable);


                                int amount = 0;

                                foreach (var item2 in inv.Assets)
                                    if (item.classid == item2.classid)
                                        amount++;

                                SelectedItem.SetVar("amount", amount);

                                return new Tuple<bool, bool>(true, true);
                            }
                        if (inv.IsNext)
                            return SearchItem(name, inv.LastAssteId).Result;
                        else
                            return new Tuple<bool, bool>(true, false);
                    }
                    else
                        return new Tuple<bool, bool>(false, false);
                }
            });
        }

        private async void PageLoaded(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW.ShowAnimGrid(true, "loading inventories of user...");

            if (await GetUserInformation())
            {
                tbUserName.Text = Player.personaname;
                imgAva.Source = new BitmapImage(new Uri(Player.avatarmedium));
                App.MAIN_WINDOW.SetupTitle(tbUserName.Text);
                tbOnline.Visibility = Player.personastate > 0 ? Visibility.Visible : Visibility.Collapsed;
                tbOffline.Visibility = tbOnline.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                Inventories = JsonConvert.DeserializeObject<List<InventoryObject>>(File.ReadAllText(App.INVENTORIES));

                int hiden = 0;

                foreach (var item in Inventories)
                {
                    int c = await GetInventoryItemsCount(item.AppID, item.AppContext);
                    if (c == -1)
                        hiden++;
                    else if (App.MAIN_WINDOW.ShowEmptyInventories || c > 0)
                    {
                        InventoryButton ib = new InventoryButton()
                        {
                            InventoryName = item.Name,
                            InventoryIcon = $"{Directory.GetCurrentDirectory()}/{item.IconPath}",
                            InventoryCount = c
                        };
                        ib.Click += InventoryClickClick;

                        if (spInventories.Children.Count == 0)
                        {
                            searchAppid = item.AppID;
                            searchAppcontext = item.AppContext;
                            ib.InventoryCheked = true;
                        }
                        else
                        {
                            ib.Margin = new Thickness(20, 0, 0, 0);
                            ib.InventoryCheked = false;
                        }

                        spInventories.Children.Add(ib);
                    }
                }

                if (hiden == Inventories.Count)
                    (new MessageWindow("Information", "Possible user profile is hidden or inventory is hidden.", Core.MessageWindowIcon.Info, Core.MessageWindowIconColor.Blue)).ShowDialog();

                App.MAIN_WINDOW.ShowAnimGrid(false, string.Empty);
            }
            else
                ;//error
        }
        private async void btnSearchItemClick(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW.ShowAnimGrid(true, "searching...");

            var result = await SearchItem(tbSearchItemName.Text);

            if (result.Item1)
            {
                if (result.Item2)
                {
                    EMPTY_LIST.IsChecked = false;

                    tbName.Text = $"Name: {SelectedItem.Name}";
                    tbType.Text = $"Type: {SelectedItem.GetVar<string>("type")}";

                    tbMarketableYes.Visibility = SelectedItem.GetVar<int>("marketable") == 1 ? Visibility.Visible : Visibility.Collapsed;
                    tbMarketableNo.Visibility = tbMarketableYes.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                    tbTradableYes.Visibility = SelectedItem.GetVar<int>("tradable") == 1 ? Visibility.Visible : Visibility.Collapsed;
                    tbTradableNo.Visibility = tbTradableYes.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                    tbAmount.Text = $"Amount: {SelectedItem.GetVar<int>("amount")}";
                    imgItemPreview.Source = new BitmapImage(new Uri(SelectedItem.IconUrl));
                }
                else
                {
                    EMPTY_LIST.IsChecked = true;
                    tbEmpty.Text = "item not found...";
                    imgItemPreview.Source = new BitmapImage(new Uri("/SteamInventoryMonitor;component/Icons/empty.png", UriKind.RelativeOrAbsolute));
                }

                App.MAIN_WINDOW.ShowAnimGrid(false, string.Empty);
                btnAdd.IsEnabled = true;
            }
            else
                ;//error
        }
        private void btnLogoutClick(object sender, RoutedEventArgs e) => App.MAIN_WINDOW.SetupViewMode(0);
        private void InventoryClickClick(InventoryButton sender, string tag)
        {
            foreach (var item in spInventories.Children)
                (item as InventoryButton).InventoryCheked = false;

            foreach (var item in Inventories)
                if (item.Name == tag)
                {
                    searchAppid = item.AppID;
                    searchAppcontext = item.AppContext;
                    sender.InventoryCheked = true;
                }
        }
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (EMPTY_LIST.IsChecked.Value)
            {
                if (string.IsNullOrWhiteSpace(tbSearchItemName.Text))
                    return;

                App.MAIN_WINDOW.AddItem(tbSearchItemName.Text, searchAppid, searchAppcontext);
            }
            else
                App.MAIN_WINDOW.AddItem(SelectedItem);
        }
    }
}
