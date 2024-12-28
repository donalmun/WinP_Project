using FoodApp.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace FoodApp.Views
{
    public sealed partial class RevenueView : Page
    {
        public RevenueView()
        {
            this.InitializeComponent();

            if (this.DataContext is RevenueViewModel viewModel)
            {
                viewModel.ShowMessageAction = async (message) =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Date Error",
                        Content = message,
                        CloseButtonText = "Try Again",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot
                    };

                    await dialog.ShowAsync();
                };
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Điều hướng trở lại trang chính
            Frame.Navigate(typeof(OrderPage));
        }
    }

    // Converter for Formatting Revenue
    public class RevenueFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is float floatValue)
            {
                return floatValue.ToString("N0", CultureInfo.CurrentCulture);
            }
            else if (value is double doubleValue)
            {
                return doubleValue.ToString("N0", CultureInfo.CurrentCulture);
            }
            else if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N0", CultureInfo.CurrentCulture);
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
