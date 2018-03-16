using Newtonsoft.Json;
using SteamInventoryMonitor.Core;
using SteamInventoryMonitor.Core.Models;
using System;
using System.Collections.Generic;
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

        void LoadSteamInventoryItemsCount()
        {
            string appid = "753"; //steam
            int appcontext = 6;
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    tbSteamItemCount.Text = inv.total_inventory_count.ToString();
                }
            }
        }
        void LoadCSGOInventoryItemsCount()
        {
            string appid = "730"; //csgo
            int appcontext = 2;
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    tbCSGOInv.Text = inv.total_inventory_count.ToString();
                }
            }
        }
        void LoadDotaInventoryItemsCount()
        {
            string appid = "570"; //dota 2
            int appcontext = 2;
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    tbDotaInv.Text = inv.total_inventory_count.ToString();
                }
            }
        }
        void LoadTFInventoryItemsCount()
        {
            string appid = "440"; //tf 2
            int appcontext = 2;
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    tbTFInv.Text = inv.total_inventory_count.ToString();
                }
            }
        }
        void LoadPBGInventoryItemsCount()
        {
            string appid = "578080"; //pbg
            int appcontext = 2;
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    tbPBGInv.Text = inv.total_inventory_count.ToString();
                }
            }
        }
        void LoadUnturnedInventoryItemsCount()
        {
            string appid = "304930"; //unturned
            int appcontext = 2;
            int count = 1;

            string str = $"http://steamcommunity.com/inventory/{Player.steamid}/{appid}/{appcontext}?l={App.LANGUAGE}&count={count}";

            using (WebClient wc = new WebClient())
            {
                Inventory inv = JsonConvert.DeserializeObject<Inventory>(wc.DownloadString(str));

                if (inv.Success)
                {
                    tbUnturnedInv.Text = inv.total_inventory_count.ToString();
                }
            }
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

                    LoadSteamInventoryItemsCount();
                    LoadCSGOInventoryItemsCount();
                    LoadDotaInventoryItemsCount();
                    LoadTFInventoryItemsCount();
                    LoadPBGInventoryItemsCount();
                    LoadUnturnedInventoryItemsCount();
                }
            }
        }
        private void btnSearchItemClick(object sender, RoutedEventArgs e) => SearchItem(tbSearchItemName.Text);
        private void btnLogoutClick(object sender, RoutedEventArgs e) => App.MAIN_WINDOW.SetupViewMode(0);

        private void InventoryButtonClick(object sender, MouseButtonEventArgs e)
        {
            switch ((sender as Grid).Name)
            {
                case "STEAM":
                default:
                    STEAM.Opacity = 1;
                    CSGO.Opacity = .5;
                    DOTA2.Opacity = .5;
                    TF2.Opacity = .5;
                    PUBG.Opacity = .5;
                    UNTURNED.Opacity = .5;

                    searchAppid = "753";
                    searchAppcontext = 6;
                    break;
                case "CSGO":
                    STEAM.Opacity = .5;
                    CSGO.Opacity = 1;
                    DOTA2.Opacity = .5;
                    TF2.Opacity = .5;
                    PUBG.Opacity = .5;
                    UNTURNED.Opacity = .5;

                    searchAppid = "730";
                    searchAppcontext = 2;
                    break;
                case "DOTA2":
                    STEAM.Opacity = .5;
                    CSGO.Opacity = .5;
                    DOTA2.Opacity = 1;
                    TF2.Opacity = .5;
                    PUBG.Opacity = .5;
                    UNTURNED.Opacity = .5;

                    searchAppid = "570";
                    searchAppcontext = 2;
                    break;
                case "TF2":
                    STEAM.Opacity = .5;
                    CSGO.Opacity = .5;
                    DOTA2.Opacity = .5;
                    TF2.Opacity = 1;
                    PUBG.Opacity = .5;
                    UNTURNED.Opacity = .5;

                    searchAppid = "440";
                    searchAppcontext = 2;
                    break;
                case "PUBG":
                    STEAM.Opacity = .5;
                    CSGO.Opacity = .5;
                    DOTA2.Opacity = .5;
                    TF2.Opacity = .5;
                    PUBG.Opacity = 1;
                    UNTURNED.Opacity = .5;

                    searchAppid = "578080";
                    searchAppcontext = 2;
                    break;
                case "UNTURNED":
                    STEAM.Opacity = .5;
                    CSGO.Opacity = .5;
                    DOTA2.Opacity = .5;
                    TF2.Opacity = .5;
                    PUBG.Opacity = .5;
                    UNTURNED.Opacity = 1;

                    searchAppid = "304930";
                    searchAppcontext = 2;
                    break;
            }
        }
    }
}
