using Microsoft.Win32;
using Newtonsoft.Json;
using SteamInventoryMonitor.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Verloka.HelperLib.Settings;

namespace SteamInventoryMonitor
{
    public partial class MainWindow : Window
    {
        public bool ShowEmptyInventories
        {
            get => rs.GetValue("ShowEmptyInventories", false);
            set => rs.SetValue("ShowEmptyInventories", value);
        }
        public bool CashingImages
        {
            get => rs.GetValue("CashingImages", true);
            set => rs.SetValue("CashingImages", value);
        }
        public bool Startup
        {
            get
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                return (string)key.GetValue("SteamInventoryMonitor") == null ? false : true;
            }
            set
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (!value)
                    key.DeleteValue("SteamInventoryMonitor", false);
                else
                    key.SetValue("SteamInventoryMonitor", $"path to task");
            }
        }

        TaskObject TO;

        TaskItem TI;
        string ItemName = string.Empty;
        string AppId = string.Empty;
        bool IsNotFound = false;

        RegSettings rs;

        public MainWindow()
        {
            rs = new RegSettings("SteamInventoryMonitor");
            TO = File.Exists(App.TASK) ? JsonConvert.DeserializeObject<TaskObject>(File.ReadAllText(App.TASK)) : new TaskObject();

            InitializeComponent();
            DataContext = this;
        }

        public void AddItem(TaskItem ti)
        {
            TI = ti;

            tbName.Text = $"Name: {ti.Name}";
            tbType.Text = $"Type: {ti.GetVar<string>("type")}";

            tbAmount.Text = $"Amount: {ti.GetVar<int>("amount")}";
            imgItem.Source = new BitmapImage(new Uri(ti.IconUrl));

            tbMarketableYes.Visibility = ti.GetVar<int>("marketable") == 1 ? Visibility.Visible : Visibility.Collapsed;
            tbMarketableNo.Visibility = tbMarketableYes.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            tbMarketableUn.Visibility = Visibility.Collapsed;

            tbTradableYes.Visibility = ti.GetVar<int>("tradable") == 1 ? Visibility.Visible : Visibility.Collapsed;
            tbTradableNo.Visibility = tbTradableYes.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            tbTradableUn.Visibility = Visibility.Collapsed;

            IsNotFound = false;
            gridAdd.Visibility = Visibility.Visible;
        }
        public void AddItem(string name, string appid)
        {
            ItemName = name;
            AppId = appid;

            tbName.Text = $"Name: {ItemName}";
            tbType.Text = "Type: NaN";

            tbAmount.Text = "Amount: 0";
            imgItem.Source = new BitmapImage(new Uri("/SteamInventoryMonitor;component/Icons/empty.png", UriKind.RelativeOrAbsolute));

            tbMarketableYes.Visibility = Visibility.Collapsed;
            tbMarketableNo.Visibility = Visibility.Collapsed;
            tbMarketableUn.Visibility = Visibility.Visible;

            tbTradableYes.Visibility = Visibility.Collapsed;
            tbTradableNo.Visibility = Visibility.Collapsed;
            tbTradableUn.Visibility = Visibility.Visible;

            IsNotFound = true;
            gridAdd.Visibility = Visibility.Visible;
        }
        public void ShowAnimGrid(bool show, string text)
        {
            gridAnim.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            tbOperationText.Text = text;
        }
        public void SetupViewMode(int modeNumber)
        {
            switch (modeNumber)
            {
                case 0:
                default:
                    Width = 520;
                    Height = 280;
                    frame.Navigate(new Uri("Views/LoginPage.xaml", UriKind.Relative));
                    break;
                case 1:
                    Width = 640;
                    Height = 540;
                    frame.Navigate(new Uri("Views/ProfilePage.xaml", UriKind.Relative));
                    break;
            }
        }
        public void SetupTitle(string title, bool full = false) => Title = full ? title : $"Steam Inventory Monitor: {title}";
        public void ShowAbout(bool show) => gridAbout.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        public void UpdateStatus() => tbStatus.Text = (TO == null || (TO.ExistItems.Count == 0 && TO.NotExistsItems.Count == 0)) ? "empty line" : $"{TO.ExistItems.Count + TO.NotExistsItems.Count} items in task";

        #region Window Events
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }
        private void btnCloseWinodwClick(object sender, RoutedEventArgs e) => Close();
        private void btnInfoClick(object sender, RoutedEventArgs e) => ShowAbout(true);
        #endregion

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW = this;

            UpdateStatus();
            SetupViewMode(0);
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void btnCloseAboutClick(object sender, RoutedEventArgs e) => ShowAbout(false);
        private void menuItemAboutClick(object sender, RoutedEventArgs e) => ShowAbout(true);
        private void menuItemExitClick(object sender, RoutedEventArgs e) => Close();
        private async void windowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (StreamWriter sw = File.CreateText(App.TASK))
                await sw.WriteLineAsync(JsonConvert.SerializeObject(TO));
        }
        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            int.TryParse(tbValue.Text, out int i);

            if (IsNotFound)
                TO.NotExistsItems.Add(new Tuple<string, string, string, int, int>(App.ID64, AppId, ItemName, cbComparer.SelectedIndex == -1 ? 0 : cbComparer.SelectedIndex, i));
            else
            {
                TI.CompareMethod = cbComparer.SelectedIndex == -1 ? 0 : cbComparer.SelectedIndex;
                TI.CompareArgument = i;
                TO.ExistItems.Add(TI);
            }

            gridAdd.Visibility = Visibility.Collapsed;
            App.MAIN_WINDOW.UpdateStatus();
        }
        private void tbValuePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
    }
}
