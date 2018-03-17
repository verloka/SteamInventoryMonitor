using Newtonsoft.Json;
using SteamInventoryMonitor.Core.Models;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        public Task<bool> GetUser()
        {
            return Task.Factory.StartNew(() =>
                {
                    string url = $"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={App.KEY}&vanityurl={LoginInfo}";

                    using (WebClient wc = new WebClient())
                    {
                        UserID64 id64 = JsonConvert.DeserializeObject<UserID64>(wc.DownloadString(url));
                        if (id64.Success)
                        {
                            App.ID64 = id64.ID64;
                            return true;
                        }
                        return false;
                    }
                });
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW.SetupTitle("Login");
            LoginInfo = string.Empty;
        }
        private async void btnLoginClick(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW.ShowAnimGrid(true, "searching the user...");

            if (await GetUser())
                App.MAIN_WINDOW.SetupViewMode(1);
            else
            {
                App.MAIN_WINDOW.ShowAnimGrid(false, string.Empty);
                //error msg
            }
        }
    }
}
