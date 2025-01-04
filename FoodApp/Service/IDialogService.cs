// FoodApp\Services\IDialogService.cs
using System.Threading.Tasks;

namespace FoodApp.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// Displays a message dialog with the specified title and message.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="message">The message content of the dialog.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ShowMessageAsync(string title, string message);
    }
}
