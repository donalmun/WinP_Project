using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

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
            if (value is int intAmount)
            {
                return $"{intAmount:N0} VNĐ";
            }
            if (value is decimal decimalAmount)
            {
                return $"{decimalAmount:N0} VNĐ";
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
