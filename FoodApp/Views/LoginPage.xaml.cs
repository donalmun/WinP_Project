using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
using System.Security.Cryptography;
using System.Text;
using FoodApp.Service.DataAccess;
using System.Threading.Tasks;

namespace FoodApp.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            LoadCredentials(); // Call LoadCredentials in the constructor
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            try
            {
                if (await AuthenticateUserAsync(username, password))
                {
                    if (RememberMeCheckBox.IsChecked == true)
                    {
                        SaveCredentials(username, password);
                    }
                    else
                    {
                        ClearCredentials();
                    }
                    // Navigate to the main page
                    this.Frame.Navigate(typeof(OrderPage));
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

        private async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            UserDao userDAO = new UserDao();
            var user = await userDAO.GetUserByUsernameAsync(username);

            if (user != null && user.Password == password)
            {
                return true;
            }

            return false;
        }

        private void SaveCredentials(string username, string password)
        {
            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            var entropyInBytes = new byte[20];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(entropyInBytes);
            }
            var encryptedPassword = ProtectedData.Protect(
                passwordInBytes,
                entropyInBytes,
                DataProtectionScope.CurrentUser);
            var encryptedPasswordInBase64 = Convert.ToBase64String(encryptedPassword);
            var entropyInBase64 = Convert.ToBase64String(entropyInBytes);

            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["Username"] = username;
            localSettings.Values["Password"] = encryptedPasswordInBase64;
            localSettings.Values["Entropy"] = entropyInBase64;
            localSettings.Values["RememberMe"] = true;
        }

        private void ClearCredentials()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("Username");
            localSettings.Values.Remove("Password");
            localSettings.Values.Remove("Entropy");
            localSettings.Values["RememberMe"] = false;
        }

        private void LoadCredentials()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("RememberMe") && (bool)localSettings.Values["RememberMe"])
            {
                if (localSettings.Values.ContainsKey("Username"))
                {
                    UsernameTextBox.Text = localSettings.Values["Username"].ToString();
                }
                if (localSettings.Values.ContainsKey("Password") && localSettings.Values.ContainsKey("Entropy"))
                {
                    var encryptedPasswordInBase64 = localSettings.Values["Password"].ToString();
                    var entropyInBase64 = localSettings.Values["Entropy"].ToString();

                    var encryptedPasswordInBytes = Convert.FromBase64String(encryptedPasswordInBase64);
                    var entropyInBytes = Convert.FromBase64String(entropyInBase64);

                    var passwordInBytes = ProtectedData.Unprotect(
                        encryptedPasswordInBytes,
                        entropyInBytes,
                        DataProtectionScope.CurrentUser);

                    PasswordBox.Password = Encoding.UTF8.GetString(passwordInBytes);
                }
                RememberMeCheckBox.IsChecked = true;
            }
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordTextBox.Text = PasswordBox.Password;
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Password = PasswordTextBox.Text;
            }
        }
    }
}