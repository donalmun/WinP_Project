using FoodApp.ViewModels;
using FoodApp.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace FoodApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void GoToOrderPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to OrderPage
            this.ContentFrame.Navigate(typeof(OrderPage));
        }

        private void GoToRevenueView_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to RevenueView
            this.ContentFrame.Navigate(typeof(RevenueView));
        }
    }
}
