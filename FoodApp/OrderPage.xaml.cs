// Assuming this is part of your OrderPage.xaml.cs file

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using FoodApp.ViewModels;

namespace FoodApp
{
    public partial class OrderPage : global::Microsoft.UI.Xaml.Controls.Page
    {

        public MainViewModel ViewModel { get; }
        // Constructor
        public OrderPage()
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
