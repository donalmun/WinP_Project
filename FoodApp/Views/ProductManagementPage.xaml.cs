// ProductManagementPage.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FoodApp.ViewModels;

namespace FoodApp.Views
{
    public sealed partial class ProductManagementPage : Page
    {
        public ProductManagementViewModel ViewModel { get; }

        public ProductManagementPage()
        {
            this.InitializeComponent();
            ViewModel = new ProductManagementViewModel();
            this.DataContext = ViewModel;
            this.Loaded += ProductManagementPage_Loaded;
        }

        private void ProductManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = this.Content.XamlRoot;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage));
        }
    }
}