// File: WinP_Project/FoodApp/Service/Controls/PaymentOptionsControl.xaml.cs

using FoodApp.Service.BankQR;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace FoodApp.Service.Controls
{
    public sealed partial class PaymentOptionsControl : UserControl
    {
        public event RoutedEventHandler PaymentConfirmed;
        public event RoutedEventHandler PaymentCanceled;

        private VietQRService _vietQRService;

        // Dependency Properties for OrderSubTotal and OrderId
        public static readonly DependencyProperty OrderSubTotalProperty =
            DependencyProperty.Register("OrderSubTotal", typeof(float), typeof(PaymentOptionsControl), new PropertyMetadata(0f, OnOrderDetailsChanged));

        public static readonly DependencyProperty OrderIdProperty =
            DependencyProperty.Register("OrderId", typeof(string), typeof(PaymentOptionsControl), new PropertyMetadata(string.Empty, OnOrderDetailsChanged));

        public float OrderSubTotal
        {
            get { return (float)GetValue(OrderSubTotalProperty); }
            set
            {
                SetValue(OrderSubTotalProperty, value);
            }
        }

     

        public PaymentOptionsControl()
        {
            this.InitializeComponent();
            _vietQRService = new VietQRService();
            BankTransferRadioButton.Checked += PaymentMethodChanged;
            BankTransferRadioButton.Unchecked += PaymentMethodChanged;
        }

        private static void OnOrderDetailsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PaymentOptionsControl;
            control?.GenerateVietQRAsync();
        }

        private async void PaymentMethodChanged(object sender, RoutedEventArgs e)
        {
            bool isBankTransfer = BankTransferRadioButton.IsChecked == true;
            BankDetailsPanel.Visibility = isBankTransfer ? Visibility.Visible : Visibility.Collapsed;

            if (isBankTransfer)
            {
                await GenerateVietQRAsync();
            }
            else
            {
                QRCodeImage.Source = null;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentConfirmed?.Invoke(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentCanceled?.Invoke(this, e);
        }

        public string SelectedPaymentMethod
        {
            get
            {
                if (CashRadioButton.IsChecked == true)
                {
                    return "Cash";
                }
                else if (BankTransferRadioButton.IsChecked == true)
                {
                    return "BankTransfer";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private async Task GenerateVietQRAsync()
        {
            Console.WriteLine($"GenerateVietQRAsync called with OrderSubTotal: {OrderSubTotal}");

            if (OrderSubTotal <= 0)
            {
                QRCodeImage.Source = null;
                return;
            }

            try
            {
                byte[] qrBytes = await _vietQRService.GenerateVietQRAsync(OrderSubTotal);
                BitmapImage bitmapImage = new BitmapImage();

                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(qrBytes.AsBuffer());
                    stream.Seek(0);
                    await bitmapImage.SetSourceAsync(stream);
                }

                QRCodeImage.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                var dialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                await dialog.ShowAsync();
            }
        }
    }
}