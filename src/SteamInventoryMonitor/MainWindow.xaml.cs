using Microsoft.Win32;
using Newtonsoft.Json;
using SteamInventoryMonitor.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
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

        RegSettings rs;

        public MainWindow()
        {
            InitializeComponent();
            rs = new RegSettings("SteamInventoryMonitor");
            DataContext = this;
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
    }
}
