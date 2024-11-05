using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace FoodApp.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                // Add your login logic here
                if (username == "admin" && password == "password")
                {
                    // Navigate to the main page
                    this.Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    // Show an error message
                    var dialog = new ContentDialog
                    {
                        Title = "Login Failed",
                        Content = "Invalid username or password.",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot // Ensure the dialog is associated with the current XAML root
                    };
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An unexpected error occurred: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot // Ensure the dialog is associated with the current XAML root
                };
                await errorDialog.ShowAsync();
            }
        }
    }
}