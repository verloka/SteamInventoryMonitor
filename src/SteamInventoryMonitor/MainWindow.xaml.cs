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

namespace SteamInventoryMonitor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
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
                    Height = 480;
                    frame.Navigate(new Uri("Views/ProfilePage.xaml", UriKind.Relative));
                    break;
            }
        }
        public void SetupTitle(string title, bool full = false) => Title = full ? title : $"Steam Inventory Monitor: {title}";

        #region Window Events
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }
        private void btnCloseWinodwClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
        /*
        private void btnMinimazeClick()
        {
        }
        private void btnHelpClick()
        {
        }*/
        #endregion

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW = this;
            SetupViewMode(0);
        }
    }
}
