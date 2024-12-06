// FoodApp\Service\Controls\AIChatControl.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace FoodApp.Service.Controls
{
    public sealed partial class AIChatControl : UserControl
    {
        public AIChatControl()
        {
            this.InitializeComponent();
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void ChatInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void SendMessage()
        {
            string userMessage = ChatInputBox.Text.Trim();
            if (!string.IsNullOrEmpty(userMessage))
            {
                // Display user message
                var userMessageBlock = new TextBlock
                {
                    Text = $"You: {userMessage}",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 10),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black)
                };
                ChatMessagesPanel.Children.Add(userMessageBlock);

                // Clear input
                ChatInputBox.Text = "";

                // Scroll to bottom
                ChatScrollViewer.UpdateLayout();
                ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ExtentHeight);

                // Simulate AI response (replace with actual AI logic)
                var aiResponse = new TextBlock
                {
                    Text = $"AI: You said '{userMessage}'. How can I help you further?",
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 10),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue)
                };
                ChatMessagesPanel.Children.Add(aiResponse);

                // Scroll to bottom
                ChatScrollViewer.UpdateLayout();
                ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ExtentHeight);
            }
        }
    }
}
