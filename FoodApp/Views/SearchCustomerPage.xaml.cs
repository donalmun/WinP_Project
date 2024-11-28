using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Views
{
    public sealed partial class SearchCustomerPage : Page
    {
        public SearchCustomerPage()
        {
            this.InitializeComponent();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage));
        }
    }
}