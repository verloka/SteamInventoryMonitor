using SteamInventoryMonitor.Task.Controlls;
using System.Windows;
using System.Windows.Controls;

namespace SteamInventoryMonitor.Task.Views
{
    public partial class NotificationLine : Page
    {
        public NotificationLine()
        {
            InitializeComponent();
        }

        void LoadItems()
        {
            spItems.Children.Clear();
            foreach (var item in ((MainWindow)Application.Current.MainWindow).Finded)
            {
                ItemControll ctrl = new ItemControll()
                {
                    UID = item.UID,
                    ItemName = item.Name,
                    UserName = item.OwnerName,
                    UserIcon = item.OwnerAvatar,
                    CompareMethod = item.CompareMethod,
                    CompareArgument = item.CompareArgument,
                    ItemIcon = item.IconUrl,
                    NF = false,
                    Editable = false
                };

                spItems.Children.Add(ctrl);
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            LoadItems();
            ((MainWindow)Application.Current.MainWindow).SetupHomeButtonIsVisible(true);
        }
        private void pageUnloaded(object sender, RoutedEventArgs e) => ((MainWindow)Application.Current.MainWindow).SetupHomeButtonIsVisible(false);
    }
}
