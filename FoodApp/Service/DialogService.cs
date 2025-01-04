// FoodApp\Services\DialogService.cs
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace FoodApp.Services
{
    public class DialogService : IDialogService
    {
        private readonly Page _page;
        private ContentDialog? _currentDialog;

        public DialogService(Page page)
        {
            _page = page;
        }

        public async Task ShowMessageAsync(string title, string message)
        {
            // If a dialog is already open, wait for it to close before showing the next one
            while (_currentDialog != null)
            {
                await Task.Delay(100);
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = _page.XamlRoot,
                DefaultButton = ContentDialogButton.Close
            };

            _currentDialog = dialog;

            await dialog.ShowAsync();

            _currentDialog = null;
        }
    }
}
