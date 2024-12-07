// AIChatViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FoodApp.Service.DataAccess;
using Microsoft.UI.Xaml.Media;

namespace FoodApp.ViewModel
{
    public class AIChatViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string ApiUrl = "https://api.jarvis.cx/api/v1/ai-chat";
        private const string SignInUrl = "https://api.jarvis.cx/api/v1/auth/sign-in";
        private const string Email = "thpz99@gmail.com";
        private const string Password = "Phuong56@";

        public ObservableCollection<ChatMessage> ChatMessages { get; } = new ObservableCollection<ChatMessage>();

        private string _userInput;
        public string UserInput
        {
            get => _userInput;
            set
            {
                if (_userInput != value)
                {
                    _userInput = value;
                    OnPropertyChanged(nameof(UserInput));
                    // Debugging
                    System.Diagnostics.Debug.WriteLine($"UserInput set to: {_userInput}");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AIChatViewModel()
        {
            InitializeHttpClientAsync();
        }

        private async Task InitializeHttpClientAsync()
        {
            string bearerToken = await GetBearerTokenAsync(Email, Password);
            httpClient.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrEmpty(bearerToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            if (!httpClient.DefaultRequestHeaders.Contains("Accept"))
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        private async Task<string> GetBearerTokenAsync(string email, string password)
        {
            var loginData = new { email, password };
            string json = JsonSerializer.Serialize(loginData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(SignInUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                return authResponse?.Token?.AccessToken ?? string.Empty;
            }
            return string.Empty;
        }

        public async Task SendMessageAsync()
        {
            // Debugging
            System.Diagnostics.Debug.WriteLine("SendMessageAsync called.");

            if (string.IsNullOrEmpty(UserInput))
            {
                System.Diagnostics.Debug.WriteLine("UserInput is null or empty.");
                return;
            }

            ChatMessages.Add(new ChatMessage($"You: {UserInput}"));
            string userMessage = await PrepareUserMessageAsync(UserInput);
            UserInput = string.Empty;
            OnPropertyChanged(nameof(UserInput));

            var aiRequest = new AiChatRequest
            {
                Assistant = new Assistant { Id = "gpt-4o", Model = "dify" },
                Content = userMessage
            };

            string jsonRequest = JsonSerializer.Serialize(aiRequest, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            try
            {
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(ApiUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var aiResponse = JsonSerializer.Deserialize<AiChatResponse>(responseBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (aiResponse?.Message != null)
                    {
                        ChatMessages.Add(new ChatMessage($"AI: {aiResponse.Message}"));
                    }
                    else
                    {
                        ChatMessages.Add(new ChatMessage("Error: Unexpected AI response."));
                    }
                }
                else
                {
                    ChatMessages.Add(new ChatMessage($"Error: AI API {response.StatusCode} - {response.ReasonPhrase}"));
                }
            }
            catch (Exception ex)
            {
                ChatMessages.Add(new ChatMessage($"Error: {ex.Message}"));
            }
        }

        private async Task<string> PrepareUserMessageAsync(string input)
        {
            ProductDao productDao = new ProductDao();
            var products = await productDao.GetAllAsync();
            string productsJson = JsonSerializer.Serialize(products, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });

            return $"{productsJson} Đây là toàn bộ món ăn của tôi, {input}";
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Inner ChatMessage Class
        public class ChatMessage : INotifyPropertyChanged
        {
            private string _message;
            public string Message
            {
                get => _message;
                set
                {
                    if (_message != value)
                    {
                        _message = value;
                        OnPropertyChanged(nameof(Message));
                    }
                }
            }

            public ChatMessage(string message)
            {
                Message = message;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        // Models for Auth response
        public class AuthResponse
        {
            [JsonPropertyName("token")]
            public TokenInfo Token { get; set; }
        }

        public class TokenInfo
        {
            [JsonPropertyName("accessToken")]
            public string AccessToken { get; set; }

            [JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }
        }

        // Models for AI chat request and response
        public class Assistant
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("model")]
            public string Model { get; set; }
        }

        public class AiChatRequest
        {
            [JsonPropertyName("assistant")]
            public Assistant Assistant { get; set; }

            [JsonPropertyName("content")]
            public string Content { get; set; }
        }

        public class AiChatResponse
        {
            [JsonPropertyName("conversationId")]
            public string ConversationId { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("remainingUsage")]
            public int RemainingUsage { get; set; }
        }
    }
}