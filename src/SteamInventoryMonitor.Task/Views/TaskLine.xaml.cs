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
            foreach (var item in App.MAIN_WINDOW.TO.Items)
            {
                ItemControll ctrl = new ItemControll()
                {
                    UID = item.UID,
                    ItemName = item.Name,
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
            App.MAIN_WINDOW.TO.Remove(uid, nf);
            LoadItems();
        }
        private void CtrlUpdated(string uid, int compareMethod, int compareArgument, bool nf) => App.MAIN_WINDOW.TO.Update(uid, compareMethod, compareArgument, nf);
        private void btnRemoveClick(object sender, RoutedEventArgs e)
        {
            App.MAIN_WINDOW.TO.Clear();
            spItems?.Children.Clear();
        }
    }
}
