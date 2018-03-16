using System;
using System.Globalization;
using System.Windows.Data;

namespace SteamInventoryMonitor.Core
{
    public class BoolToEnabledConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? false : true;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? true : false;
    }
}
