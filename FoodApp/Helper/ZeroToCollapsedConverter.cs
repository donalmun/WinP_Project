using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FoodApp.Helper
{
    public class ZeroToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double doubleValue)
            {
                // Nếu giá trị là 0, trả về Collapsed, ngược lại là Visible
                return doubleValue == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible; // Mặc định hiển thị nếu không phải là số
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}