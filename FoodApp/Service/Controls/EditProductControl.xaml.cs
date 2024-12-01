using Microsoft.UI.Xaml.Controls;
using System;
using FoodApp.Service.DataAccess;
using System.ComponentModel;

namespace FoodApp.Service.Controls
{
    public sealed partial class EditProductControl : UserControl
    {
        public event EventHandler<bool> ValidityChanged;

        public EditProductControl(Product product)
        {
            InitializeComponent();
            DataContext = product;

            // Subscribe to property changes
            if (product != null)
            {
                product.PropertyChanged += Product_PropertyChanged;
                CheckValidity();
            }
        }

        private void Product_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckValidity();
        }

        private void CheckValidity()
        {
            var product = DataContext as Product;
            bool isValid = product != null &&
                !string.IsNullOrWhiteSpace(product.Name) &&
                product.Cost > 0 &&
                !string.IsNullOrWhiteSpace(product.Image) &&
                product.Category_Id > 0;

            ValidityChanged?.Invoke(this, isValid);
        }
    }
}
