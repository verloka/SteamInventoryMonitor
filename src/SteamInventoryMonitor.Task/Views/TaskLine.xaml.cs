using SteamInventoryMonitor.Task.Controlls;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SteamInventoryMonitor.Task.Views
{
    public partial class TaskLine : Page
    {
        public TaskLine()
        {
            InitializeComponent();
        }

        private void pageLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in App.MAIN_WINDOW.TO.Items)
            {
                spItems.Children.Add(new ItemControll()
                {
                    UID = item.UID,
                    ItemName = item.Name,
                    CompareMethod = item.CompareMethod,
                    CompareArgument = item.CompareArgument,
                    ItemIcon = item.IconUrl
                });
            }
        }
    }
}
