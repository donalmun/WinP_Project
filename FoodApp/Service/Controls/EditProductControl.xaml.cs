// EditProductControl.xaml.cs
using Microsoft.UI.Xaml.Controls;

namespace FoodApp.Service.Controls
{
    public sealed partial class EditProductControl : UserControl
    {
        public Product UpdatedProduct { get; private set; }

        public EditProductControl(Product product)
        {
            this.InitializeComponent();
            this.DataContext = product;
            UpdatedProduct = product;
        }
    }
}