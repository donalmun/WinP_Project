using FoodApp.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;

namespace FoodApp.Views
{
    public sealed partial class RevenueView : Page
    {
        public RevenueView()
        {
            this.InitializeComponent();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Handle the filter button click event
            var fromDate = FromDatePicker.Date;
            var toDate = ToDatePicker.Date;

            // Assuming you have a method in your ViewModel to filter data
            var viewModel = (RevenueViewModel)this.DataContext;
            viewModel.FilterRevenueData(fromDate, toDate);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main page
            Frame.Navigate(typeof(MainPage));
        }
    }
}