using FoodApp.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks; // Thêm thư viện này để sử dụng AsTask

namespace FoodApp.Views
{
    public partial class RegisterMembership : Page
    {
        public RegisterMembership()
        {
            InitializeComponent();
            var viewModel = new RegisterMembershipViewModel();
            DataContext = viewModel;

            viewModel.ShowMessageRequested += async (message) =>
            {
                var dialog = new ContentDialog
                {
                    Title = "Thông báo",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync().AsTask(); // Sử dụng AsTask() để chuyển đổi
            };

            viewModel.NavigateToOrderPageRequested += () =>
            {
                Frame.Navigate(typeof(OrderPage));
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage));
        }
    }
}