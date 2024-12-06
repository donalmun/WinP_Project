using FoodApp.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

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
}