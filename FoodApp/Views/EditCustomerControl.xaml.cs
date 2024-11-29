// WinP_Project\FoodApp\Views\EditCustomerControl.xaml.cs
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Views
{
    public sealed partial class EditCustomerControl : UserControl
    {
        public Customer UpdatedCustomer { get; private set; }

        public EditCustomerControl(Customer customer)
        {
            this.InitializeComponent();
            this.DataContext = customer;
            UpdatedCustomer = customer;
        }
    }
}