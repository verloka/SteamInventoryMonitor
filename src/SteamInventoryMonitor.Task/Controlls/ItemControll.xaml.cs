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

namespace SteamInventoryMonitor.Task.Controlls
{
    public partial class ItemControll : UserControl
    {
        public event Action<string, int, int, bool> Updated;
        public event Action<string, bool> Removed;

        public string UID
        {
            get { return GetValue(UIDProperty) as string; }
            set { SetValue(UIDProperty, value); }
        }
        public static readonly DependencyProperty UIDProperty = DependencyProperty.Register("UID", typeof(string), typeof(ItemControll), null);

        public bool NF
        {
            get { return (bool)GetValue(NFProperty); }
            set { SetValue(NFProperty, value); }
        }
        public static readonly DependencyProperty NFProperty = DependencyProperty.Register("NF", typeof(bool), typeof(ItemControll), null);

        public string ItemName
        {
            get { return GetValue(InventoryNameProperty) as string; }
            set { SetValue(InventoryNameProperty, value); }
        }
        public static readonly DependencyProperty InventoryNameProperty = DependencyProperty.Register("ItemName", typeof(string), typeof(ItemControll), null);

        public string ItemIcon
        {
            get { return GetValue(ItemIconProperty) as string; }
            set { SetValue(ItemIconProperty, value); }
        }
        public static readonly DependencyProperty ItemIconProperty = DependencyProperty.Register("ItemIcon", typeof(string), typeof(ItemControll), null);

        public int CompareMethod
        {
            get { return (int)GetValue(CompareMethodProperty); }
            set { SetValue(CompareMethodProperty, value); }
        }
        public static readonly DependencyProperty CompareMethodProperty = DependencyProperty.Register("CompareMethod", typeof(int), typeof(ItemControll), null);

        public int CompareArgument
        {
            get { return (int)GetValue(CompareArgumentProperty); }
            set { SetValue(CompareArgumentProperty, value); }
        }
        public static readonly DependencyProperty CompareArgumentProperty = DependencyProperty.Register("CompareArgument", typeof(int), typeof(ItemControll), null);

        public ItemControll()
        {
            InitializeComponent();
            DataContext = this;
        }

        void UpdateDescription() => tbDesc.Text = $"Show notification when item {(cbComparer.SelectedItem as ComboBoxItem).Content.ToString()} {CompareArgument} in inventory";

        private void tbValuePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
        private void userControlLoaded(object sender, RoutedEventArgs e)
        {
            UpdateDescription();
        }
        private void btnDoneClick(object sender, RoutedEventArgs e)
        {
            Updated?.Invoke(UID, CompareMethod, CompareArgument, NF);
            UpdateDescription();
        }
        private void btnRemoveClick(object sender, RoutedEventArgs e) => Removed?.Invoke(UID, NF);
    }
}
