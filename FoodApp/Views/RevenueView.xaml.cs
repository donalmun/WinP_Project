using FoodApp.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage.Streams; // Add this using directive
using Windows.Storage; // Add this using directive
using System.IO; // Add this using directive

namespace FoodApp.Views
{
    public sealed partial class RevenueView : Page
    {
        public RevenueView()
        {
            this.InitializeComponent();

            if (this.DataContext is RevenueViewModel viewModel)
            {
                viewModel.ShowMessageAction = async (message) =>
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Date Error",
                        Content = message,
                        CloseButtonText = "Try Again",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot
                    };

                    await dialog.ShowAsync();
                };
            }

            LoadChart();
        }

        private async void LoadChart()
        {
            try
            {
                // Ensure CoreWebView2 is initialized
                await ChartWebView.EnsureCoreWebView2Async();

                // Load HTML content from file
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var file = await folder.GetFileAsync(@"Assets\chart.html");
                var fileStream = await file.OpenReadAsync();

                using (var reader = new StreamReader(fileStream.AsStream())) // Use AsStream() instead of AsStreamForRead()
                {
                    string htmlContent = await reader.ReadToEndAsync();
                    ChartWebView.NavigateToString(htmlContent);
                }

                // Load default chart data
                if (this.DataContext is RevenueViewModel viewModel)
                {
                    // Await the asynchronous method to get the JSON string
                    string chartDataJson = await viewModel.GetMonthlyRevenueDataJsonAsync();

                    // Pass the JSON object directly without wrapping it in single quotes
                    string script = $"loadBarChart({chartDataJson}, 'Revenue by Month');";
                    await ChartWebView.CoreWebView2.ExecuteScriptAsync(script);
                }
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred while loading the chart: {ex.Message}",
                    CloseButtonText = "OK",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };

                await dialog.ShowAsync();
            }
        }

        private async void ChartTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext is RevenueViewModel viewModel)
            {
                try
                {
                    string chartType = ((ComboBoxItem)ChartTypeComboBox.SelectedItem).Content.ToString();

                    string script = string.Empty;

                    switch (chartType)
                    {
                        case "Revenue by Month":
                            string monthlyDataJson = await viewModel.GetMonthlyRevenueDataJsonAsync();
                            // Pass JSON without single quotes
                            script = $"loadBarChart({monthlyDataJson}, 'Revenue by Month');";
                            break;
                        case "Revenue by Day":
                            string dailyDataJson = await viewModel.GetDailyRevenueDataJsonAsync();
                            // Pass JSON without single quotes
                            script = $"loadBarChart({dailyDataJson}, 'Revenue by Day');";
                            break;
                        case "Sales by Category":
                            string salesDataJson = await viewModel.GetSalesByCategoryDataJsonAsync();
                            // Pass JSON without single quotes
                            script = $"loadPieChart({salesDataJson}, 'Sales by Category');";
                            break;
                    }

                    if (!string.IsNullOrEmpty(script))
                    {
                        await ChartWebView.CoreWebView2.ExecuteScriptAsync(script);
                    }
                }
                catch (Exception ex)
                {
                    var dialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = $"An error occurred while loading the chart: {ex.Message}",
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.XamlRoot
                    };

                    await dialog.ShowAsync();
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main page
            Frame.Navigate(typeof(OrderPage));
        }
    }
}
