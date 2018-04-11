using Newtonsoft.Json;
using SteamInventoryMonitor.Models;
using System.Net;
using System.Text.RegularExpressions;
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

        public Task<bool> GetUserByLogin()
        {
            return Task.Factory.StartNew(() =>
                {
                    bool result = false;

                    try
                    {
                        string url = $"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={App.KEY}&vanityurl={LoginInfo}";

                        using (WebClient wc = new WebClient())
                        {
                            UserID64 id64 = JsonConvert.DeserializeObject<UserID64>(wc.DownloadString(url));
                            if (id64.Success)
                            {
                                App.ID64 = id64.ID64;
                                result = true;
                            }
                            else
                                result = false;
                        }
                    }
                    catch { result = false; }

                    return result;
                });
        }
        Task<bool> GetUserByID64()
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string url = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={App.KEY}&steamids={App.ID64}";

                    using (WebClient wc = new WebClient())
                    {
                        UserInformation ui = JsonConvert.DeserializeObject<UserInformation>(wc.DownloadString(url));
                        return ui.Success;
                    }
                }
                catch {  return false; }
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

            if (Regex.IsMatch(LoginInfo, @"^\d{17}$") && await GetUserByID64())
            {
                App.ID64 = LoginInfo;
                App.MAIN_WINDOW.SetupViewMode(1);
            }
            else if (await GetUserByLogin())
                App.MAIN_WINDOW.SetupViewMode(1);
            else
            {
                App.MAIN_WINDOW.ShowAnimGrid(false, string.Empty);
                new MessageWindow("Warning!", "User with that login not found!", Core.MessageWindowIcon.Warning, Core.MessageWindowIconColor.Blue).ShowDialog();
            }
        }
    }
}
