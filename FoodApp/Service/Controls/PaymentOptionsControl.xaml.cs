using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Service.Controls
{
    public sealed partial class PaymentOptionsControl : UserControl
    {
        public event RoutedEventHandler PaymentConfirmed;
        public event RoutedEventHandler PaymentCanceled; // New event

        public PaymentOptionsControl()
        {
            this.InitializeComponent();
            BankTransferRadioButton.Checked += PaymentMethodChanged;
            BankTransferRadioButton.Unchecked += PaymentMethodChanged;
        }

        private void PaymentMethodChanged(object sender, RoutedEventArgs e)
        {
            BankDetailsPanel.Visibility = BankTransferRadioButton.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentConfirmed?.Invoke(this, e);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            PaymentCanceled?.Invoke(this, e); // Raise the PaymentCanceled event
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
    }
}