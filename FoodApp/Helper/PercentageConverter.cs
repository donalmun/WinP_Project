using Microsoft.UI.Xaml.Data;
using System;

namespace FoodApp.Helper
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double percentage)
            {
                return $"{percentage}%"; // Thêm ký hiệu % vào cuối
            }
            if (value is decimal decimalPercentage)
            {
                return $"{decimalPercentage}%"; // Tương tự cho decimal
            }
            if (value is int intPercentage)
            {
                return $"{intPercentage}%"; // Tương tự cho int
            }
            return "0%";
        }


        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
