using System.Globalization;
using System.Windows.Data;

namespace CokeOvenSystem.ViewModels
{
    public class DatabaseStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLoaded)
            {
                return isLoaded ? "已加载" : "未加载";
            }
            return "未知状态";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DatabaseStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isLoaded)
            {
                return isLoaded ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
            }
            return System.Windows.Media.Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}