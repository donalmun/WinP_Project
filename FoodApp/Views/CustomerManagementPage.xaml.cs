// WinP_Project\FoodApp\Views\CustomerManagementPage.xaml.cs
using FoodApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Views
{
    public sealed partial class CustomerManagementPage : Page
    {
        private CustomerManagementViewModel ViewModel => (CustomerManagementViewModel)this.DataContext;

        public CustomerManagementPage()
        {
            this.InitializeComponent();
            this.Loaded += CustomerManagementPage_Loaded;
        }

        private void CustomerManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = this.Content.XamlRoot;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage));
        }
    }
}