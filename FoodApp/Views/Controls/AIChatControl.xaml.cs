// AIChatControl.xaml.cs
using FoodApp.ViewModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Threading.Tasks;

namespace FoodApp.Views.Controls
{
    public sealed partial class AIChatControl : UserControl
    {
        public AIChatViewModel ViewModel { get; }

        public AIChatControl()
        {
            this.InitializeComponent();
            ViewModel = (AIChatViewModel)this.DataContext;
        }

        private async void SendMessage_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await ViewModel.SendMessageAsync();
            ScrollToBottom();
        }

        private async void ChatInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !e.KeyStatus.IsKeyReleased)
            {
                await ViewModel.SendMessageAsync();
                ScrollToBottom();
                e.Handled = true;
            }
        }

        private void ScrollToBottom()
        {
            // Scroll to the bottom of the ScrollViewer after a small delay to ensure UI is updated
            ChatScrollViewer.UpdateLayout();
            ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ExtentHeight);
        }
    }
}