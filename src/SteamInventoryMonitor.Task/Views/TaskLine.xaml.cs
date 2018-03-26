using SteamInventoryMonitor.Task.Controlls;
using System.Windows;
using System.Windows.Controls;

namespace SteamInventoryMonitor.Task.Views
{
    public partial class TaskLine : Page
    {
        public TaskLine()
        {
            InitializeComponent();
        }

        void LoadItems()
        {
            spItems.Children.Clear();
            foreach (var item in ((MainWindow)Application.Current.MainWindow).TO.Items)
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
                    NF = false
                };

                ctrl.Updated += CtrlUpdated;
                ctrl.Removed += CtrlRemoved;

                spItems.Children.Add(ctrl);
            }
        }

        private void pageLoaded(object sender, RoutedEventArgs e) => LoadItems();

        private void CtrlRemoved(string uid, bool nf)
        {
            ((MainWindow)Application.Current.MainWindow).TO.Remove(uid, nf);
            LoadItems();
        }
        private void CtrlUpdated(string uid, int compareMethod, int compareArgument, bool nf) => ((MainWindow)Application.Current.MainWindow).TO.Update(uid, compareMethod, compareArgument, nf);
        private void btnRemoveClick(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).TO.Clear();
            spItems?.Children.Clear();
        }
    }
}
