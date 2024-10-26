using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace FoodApp.Helper
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double amount)
            {
                return $"{amount:N0} VNĐ"; // Định dạng số với dấu phẩy và thêm " VNĐ"
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}