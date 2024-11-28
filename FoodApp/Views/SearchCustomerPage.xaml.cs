using FoodApp.ViewModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace FoodApp.Views
{
    public sealed partial class SearchCustomerPage : Page
    {
        private readonly SearchCustomerViewModel _viewModel;

        public SearchCustomerPage()
        {
            this.InitializeComponent();

            // Initialize the ViewModel
            _viewModel = new SearchCustomerViewModel();

            // Set the DataContext for data binding
            this.DataContext = _viewModel;

            // Subscribe to the ShowMessageRequested event
            _viewModel.ShowMessageRequested += ViewModel_ShowMessageRequested;
        }

        // Event handler to display messages
        private async void ViewModel_ShowMessageRequested(object sender, MessageEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = e.Title,
                Content = e.Content,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot // Ensure the dialog has a XamlRoot
            };

            await dialog.ShowAsync();
        }

        // Handle navigation back to the OrderPage
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // Additional initialization if needed
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            // Unsubscribe from events to prevent memory leaks
            _viewModel.ShowMessageRequested -= ViewModel_ShowMessageRequested;
        }
    }
}
