
using FoodApp.ViewModels;
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

        private void GoToLoginPage_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to LoginPage
            this.ContentFrame.Navigate(typeof(LoginPage));
        }

    }

}








