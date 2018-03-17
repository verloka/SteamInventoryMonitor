using Newtonsoft.Json;
using SteamInventoryMonitor.Controlls;
using SteamInventoryMonitor.Core;
using SteamInventoryMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SteamInventoryMonitor.Views
{
    public partial class ProfilePage : Page
    {
        public Player Player { get; set; }
        public List<InventoryObject> Inventories { get; set; }

        string searchAppid = "753"; //steam
        int searchAppcontext = 6;   //2 - games item, 6 - steam items (bg, emotion, cards, gems)
        int searchCount = 5000;

        public ProfilePage()
        {
            InitializeComponent();
        }

        int GetInventoryItemsCount(string appid, int appcontext)
        {
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                    return inv.total_inventory_count;
            }

            return 0;
        }
        void SearchItem(string name, string lid = "")
        {
            string lastid = lid;
            string tail = string.IsNullOrWhiteSpace(lastid) ? string.Empty : $"&start_assetid={lastid}";

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{searchAppid}/{searchAppcontext}?l={App.LANGUAGE}&count={searchCount}{tail}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    foreach (var item in inv.descriptions)
                        if (item.name == name)
                        {
                            tbName.Text = $"Name: {item.name}";
                            tbType.Text = $"Type: {item.type}";

                            tbMarketableYes.Visibility = item.marketable == 1 ? Visibility.Visible : Visibility.Collapsed;
                            tbMarketableNo.Visibility = tbMarketableYes.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                            tbTradableYes.Visibility = item.tradable == 1 ? Visibility.Visible : Visibility.Collapsed;
                            tbTradableNo.Visibility = tbTradableYes.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                            int amount = 0;

                            foreach (var item2 in inv.assets)
                                if (item.classid == item2.classid)
                                    amount++;

                            tbAmount.Text = $"Amount: {amount}";

                            imgItemPreview.Source = new BitmapImage(new Uri($"{App.IMG_URL}{item.icon_url}"));

                            EMPTY_LIST.IsChecked = false;
                            return;
                        }
                    if (inv.Next)
                        SearchItem(name, inv.last_assetid);
                    else
                    {
                        EMPTY_LIST.IsChecked = true;
                        tbEmpty.Text = "item not found...";
                    }
                }
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            string url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={App.KEY}&steamids={App.ID64}";

            using (WebClient wc = new WebClient())
            {
                UserInformation ui = JsonConvert.DeserializeObject<UserInformation>(wc.DownloadString(url));
                if (ui.Success)
                {
                    Player = ui.Player;

                    tbUserName.Text = Player.personaname;

                    App.MAIN_WINDOW.SetupTitle(tbUserName.Text);

                    tbOnline.Visibility = Player.personastate > 0 ? Visibility.Visible : Visibility.Collapsed;
                    tbOffline.Visibility = tbOnline.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

                    imgAva.Source = new BitmapImage(new Uri(Player.avatarmedium));

                    Inventories = JsonConvert.DeserializeObject<List<InventoryObject>>(File.ReadAllText(App.INVENTORIES));

                    foreach (var item in Inventories)
                    {
                        int c = GetInventoryItemsCount(item.AppID, item.AppContext);
                        if (App.OPTION_SHOW_EMPTY_INVENTORY || c > 0)
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
                }
            }
        }

        private void btnSearchItemClick(object sender, RoutedEventArgs e) => SearchItem(tbSearchItemName.Text);
        private void btnLogoutClick(object sender, RoutedEventArgs e) => App.MAIN_WINDOW.SetupViewMode(0);
        private void InventoryClickClick(InventoryButton sender, string tag)
        {
            foreach (var item in spInventories.Children)
                (item as InventoryButton).InventoryCheked = false;

            foreach (var item in Inventories)
                if(item.Name == tag)
                {
                    searchAppid = item.AppID;
                    searchAppcontext = item.AppContext;
                    sender.InventoryCheked = true;
                }
        }
    }
}
