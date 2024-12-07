// WinP_Project\FoodApp\Service\Controls\EditCustomerControl.xaml.cs
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Views.Controls
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