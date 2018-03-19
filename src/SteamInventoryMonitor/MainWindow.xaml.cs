using Microsoft.Win32;
using Newtonsoft.Json;
using SteamInventoryMonitor.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
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

        RegSettings rs;

        public MainWindow()
        {
            rs = new RegSettings("SteamInventoryMonitor");
            TO = File.Exists(App.TASK) ? JsonConvert.DeserializeObject<TaskObject>(File.ReadAllText(App.TASK)) : new TaskObject();

            InitializeComponent();
            DataContext = this;
        }

        public void AddItem(TaskItem ti) => TO.ExistItems.Add(ti);
        public void AddItem(string name, string appid) => TO.NotExistsItems.Add(new KeyValuePair<string, string>(appid, name));
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
    }
}
