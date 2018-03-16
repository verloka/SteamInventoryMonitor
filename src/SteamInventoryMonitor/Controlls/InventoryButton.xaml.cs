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

namespace SteamInventoryMonitor.Controlls
{
    public partial class InventoryButton : UserControl
    {
        public event Action<InventoryButton, string> Click;

        public string InventoryName
        {
            get { return GetValue(InventoryNameProperty) as string; }
            set { SetValue(InventoryNameProperty, value); }
        }
        public static readonly DependencyProperty InventoryNameProperty = DependencyProperty.Register("InventoryName", typeof(string), typeof(InventoryButton), null);

        public int InventoryCount
        {
            get { return (int)GetValue(InventoryCountProperty); }
            set { SetValue(InventoryCountProperty, value); }
        }
        public static readonly DependencyProperty InventoryCountProperty = DependencyProperty.Register("InventoryCount", typeof(int), typeof(InventoryButton), null);

        public string InventoryIcon
        {
            get { return GetValue(InventoryIconProperty) as string; }
            set { SetValue(InventoryIconProperty, value); }
        }
        public static readonly DependencyProperty InventoryIconProperty = DependencyProperty.Register("InventoryIcon", typeof(string), typeof(InventoryButton), null);

        public bool InventoryCheked
        {
            get { return (bool)GetValue(InventoryChekedProperty); }
            set
            {
                root.Opacity = value ? 1 : .5;
                SetValue(InventoryChekedProperty, value);
            }
        }
        public static readonly DependencyProperty InventoryChekedProperty = DependencyProperty.Register("InventoryCheked", typeof(bool), typeof(InventoryButton), null);

        public InventoryButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void InventoryButtonClick(object sender, MouseButtonEventArgs e) => Click?.Invoke(this, (sender as Grid).Tag.ToString());
    }
}
