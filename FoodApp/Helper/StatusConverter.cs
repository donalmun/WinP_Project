// StatusConverter.cs
using Microsoft.UI.Xaml.Data;
using System;

namespace FoodApp.Helper
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int status)
            {
                return status == 1 ? "Occupied" : "Empty";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string status)
            {
                return status == "Occupied" ? (int)1 : (int)0;
            }
            return (byte)0;
        }
    }
}
