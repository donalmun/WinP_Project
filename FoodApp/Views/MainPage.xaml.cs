using FoodApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;

namespace FoodApp
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Product product)
            {
                ViewModel.AddToInvoice(product);
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle checkout button click event here
            DisplayCheckoutMessage();
        }

        private void DisplayCheckoutMessage()
        {
            var dialog = new ContentDialog
            {
                Title = "Thanh toán",
                Content = "Bạn đã nhấn nút thanh toán.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot // Set the XamlRoot property
            };

            _ = dialog.ShowAsync();
        }
    }
}