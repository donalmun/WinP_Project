// WinP_Project\FoodApp\Service\Controls\EditCustomerControl.xaml.cs
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Service.Controls
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