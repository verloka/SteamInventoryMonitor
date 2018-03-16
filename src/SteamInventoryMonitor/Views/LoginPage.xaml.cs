using Newtonsoft.Json;
using SteamInventoryMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamInventoryMonitor.Views
{
    public partial class LoginPage : Page
    {
        public string LoginInfo { get; set; }

        public LoginPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW.SetupTitle("Login");
            LoginInfo = string.Empty;
        }
        private void btnLoginClick(object sender, RoutedEventArgs e)
        {
            string url = $"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={App.KEY}&vanityurl={LoginInfo}";

            using (WebClient wc = new WebClient())
            {
                UserID64 id64 = JsonConvert.DeserializeObject<UserID64>(wc.DownloadString(url));
                if(id64.Success)
                {
                    App.ID64 = id64.ID64;
                    App.MAIN_WINDOW.SetupViewMode(1);
                }
            }
        }
    }
}
