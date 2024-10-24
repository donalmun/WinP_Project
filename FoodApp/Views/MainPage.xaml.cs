using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FoodApp
{
    public sealed partial class MainPage : Page
    {

        public MainViewModel ViewModel { get; set; } 
        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new MainViewModel();
        }
       

        private void GoToLoginPage_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(LoginPage));
        }
    }
}
