// TableManagementPage.xaml.cs
using FoodApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Views
{
    public sealed partial class TableManagementPage : Page
    {
        private TableManagementViewModel ViewModel => (TableManagementViewModel)this.DataContext;

        public TableManagementPage()
        {
            this.InitializeComponent();
            this.Loaded += TableManagementPage_Loaded;
        }

        private void TableManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = this.Content.XamlRoot;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OrderPage));
        }
    }
}
