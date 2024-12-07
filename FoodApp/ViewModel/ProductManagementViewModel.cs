using FoodApp.Helper;
using FoodApp.Service.DataAccess;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using System.Linq;
using FoodApp.Views.Controls;

namespace FoodApp.ViewModels
{
    public class ProductManagementViewModel : INotifyPropertyChanged
    {
        public XamlRoot XamlRoot { get; set; }

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(nameof(Products)); }
        }

        private ObservableCollection<Product> _filteredProducts;
        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set { _filteredProducts = value; OnPropertyChanged(nameof(FilteredProducts)); }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                // Removed ApplyFilter from setter
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(nameof(SelectedProduct)); }
        }

        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand LoadProductsCommand { get; }
        public ICommand SearchCommand { get; }

        private readonly ProductDao _productDao;

        public ProductManagementViewModel()
        {
            _productDao = new ProductDao();
            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            DeleteCommand = new RelayCommand<Product>(async (product) => await DeleteProductAsync(product));
            EditCommand = new RelayCommand<Product>(async (product) => await EditProductAsync(product));
            AddCommand = new RelayCommand(async () => await AddProductAsync());
            LoadProductsCommand = new RelayCommand(async () => await LoadProductsAsync());
            SearchCommand = new RelayCommand(ApplyFilter);
            LoadProductsCommand.Execute(null);
        }

        private async Task LoadProductsAsync()
        {
            var products = await _productDao.GetAllAsync();
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            else
            {
                var lowerSearchText = SearchText.ToLower();
                var filtered = Products.Where(p => p.Name.ToLower().Contains(lowerSearchText));
                FilteredProducts = new ObservableCollection<Product>(filtered);
            }
        }

        private async Task DeleteProductAsync(Product product)
        {
            if (product == null) return;

            var dialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete this product?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await _productDao.DeleteAsync(product.Id);
                Products.Remove(product);
                ApplyFilter();
                await ShowMessageAsync("Success", "Product deleted successfully.");
            }
        }

        private async Task EditProductAsync(Product product)
        {
            if (product == null) return;

            var editDialog = new ContentDialog
            {
                Title = "Edit Product",
                Content = new EditProductControl(product),
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await editDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await _productDao.UpdateAsync(product);
                await ShowMessageAsync("Success", "Product updated successfully.");
                LoadProductsCommand.Execute(null);
            }
        }

        private async Task AddProductAsync()
        {
            var newProduct = new Product();
            var editControl = new EditProductControl(newProduct);

            var addDialog = new ContentDialog
            {
                Title = "Add New Product",
                Content = editControl,
                PrimaryButtonText = "Add",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
                IsPrimaryButtonEnabled = false // Disable the Add button initially
            };

            // Subscribe to the ValidityChanged event
            editControl.ValidityChanged += (s, isValid) =>
            {
                addDialog.IsPrimaryButtonEnabled = isValid;
            };

            var result = await addDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await _productDao.AddAsync(newProduct);
                Products.Add(newProduct);
                ApplyFilter();
                await ShowMessageAsync("Success", "Product added successfully.");
            }
        }


        private async Task ShowMessageAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}