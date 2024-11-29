using System.Collections.ObjectModel;
using FoodApp.Service.DataAccess;
using FoodApp.Helper;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System;

namespace FoodApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDao<Product> _productDao;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<InvoiceItem> _invoiceItems;
        private Product _selectedProduct;

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        private InvoiceItem _selectedInvoiceItem;

        public InvoiceItem SelectedInvoiceItem
        {
            get => _selectedInvoiceItem;
            set => SetProperty(ref _selectedInvoiceItem, value);
        }
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);

        }

        public ObservableCollection<InvoiceItem> InvoiceItems
        {
            get => _invoiceItems;
            set
            {
                if (SetProperty(ref _invoiceItems, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                    UpdateInvoiceItemIndexes();
                    SubscribeToInvoiceItemChanges();
                }
            }
        }

        public double TotalAmount => InvoiceItems.Sum(item => item.TotalPrice);

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                if (SetProperty(ref _searchKeyword, value))
                {
                    // Trigger search whenever the keyword changes
                    SearchProducts();
                }
            }
        }

        public ICommand SaveProductCommand { get; }
        public ICommand AddNewProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand SearchCommand { get; }

        public MainViewModel()
        {
            //_productDao = new MockDao<Product>();
            _productDao = new ProductDao();
            Products = new ObservableCollection<Product>();
            InvoiceItems = new ObservableCollection<InvoiceItem>();
            InvoiceItems.CollectionChanged += InvoiceItems_CollectionChanged;

            SearchCommand = new RelayCommand(() => SearchProducts());
            SaveProductCommand = new RelayCommand(() => SaveProduct());
            AddNewProductCommand = new RelayCommand(() => AddNewProduct());
            EditProductCommand = new RelayCommand<Product>(EditProduct);
            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);

            // Check database connection
            TestDatabaseConnection();

            LoadProducts();

            SelectedInvoiceItem = new InvoiceItem
            {
                SurchargeType = "%",
                DiscountType = "%"
            };
        }

        //private void SearchProducts()
        //{
        //    if (string.IsNullOrEmpty(SearchKeyword))
        //    {
        //        // If the search keyword is empty, load all products
        //        LoadProducts();
        //    }
        //    else
        //    {
        //        // Filter products based on the search keyword
        //        var filteredProducts = _products.Where(p => p.Name.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0);
        //        Products = new ObservableCollection<Product>(filteredProducts);
        //    }
        //}

        private async void SaveProduct()
        {
            if (SelectedProduct != null)
            {
                if (SelectedProduct.Id == 0)
                {
                    await _productDao.AddAsync(SelectedProduct);
                }
                else
                {
                    await _productDao.UpdateAsync(SelectedProduct);
                }
                LoadProducts();
            }
        }

        private void AddNewProduct()
        {
            SelectedProduct = new Product();
        }

        private void EditProduct(object parameter)
        {
            if (parameter is Product product)
            {
                SelectedProduct = product;
            }
        }

        private async void DeleteProduct(object parameter)
        {
            if (parameter is Product product)
            {
                await _productDao.DeleteAsync(product.Id);
                LoadProducts();
            }
        }

        private void SearchProducts()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                Products = new ObservableCollection<Product>(_allProducts);
            }
            else
            {
                var filteredProducts = _allProducts
                    .Where(p => p.Name.IndexOf(SearchKeyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                Products = new ObservableCollection<Product>(filteredProducts);
            }
        }

        private async void TestDatabaseConnection()
        {
            bool isConnected = await _productDao.TestConnectionAsync();
            if (!isConnected)
            {
                // Xử lý khi kết nối thất bại (ví dụ: hiển thị thông báo lỗi cho người dùng)
                Debug.WriteLine("Failed to connect to the database.");
            }
        }

        private async void LoadProducts()
        {
            var products = await _productDao.GetAllAsync();
            _allProducts = new ObservableCollection<Product>(products);
            Products = new ObservableCollection<Product>(_allProducts);
            //foreach (var product in products)
            //{
            //    Products.Add(product);
            //}
        }

        public void AddToInvoice(Product product)
        {
            var existingItem = InvoiceItems.FirstOrDefault(i => i.Product == product);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var newItem = new InvoiceItem { Product = product, Quantity = 1 };
                newItem.PropertyChanged += InvoiceItem_PropertyChanged;
                InvoiceItems.Add(newItem);
            }
            OnPropertyChanged(nameof(TotalAmount));
            UpdateInvoiceItemIndexes();
        }

        private void InvoiceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalAmount));
            UpdateInvoiceItemIndexes();
            SubscribeToInvoiceItemChanges();
        }

        public void RemoveFromInvoice(Product product)
        {
            var itemToRemove = InvoiceItems.FirstOrDefault(i => i.Product == product);
            if (itemToRemove != null)
            {
                itemToRemove.PropertyChanged -= InvoiceItem_PropertyChanged;
                InvoiceItems.Remove(itemToRemove);
                OnPropertyChanged(nameof(TotalAmount));
                UpdateInvoiceItemIndexes();
            }
        }


        private void UpdateInvoiceItemIndexes()
        {
            for (int i = 0; i < InvoiceItems.Count; i++)
            {
                InvoiceItems[i].Index = i + 1;
            }
        }

        private void SubscribeToInvoiceItemChanges()
        {
            foreach (var item in InvoiceItems)
            {
                item.PropertyChanged -= InvoiceItem_PropertyChanged;
                item.PropertyChanged += InvoiceItem_PropertyChanged;
            }
        }

        private void InvoiceItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InvoiceItem.TotalPrice))
            {
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public async Task FilterProductsByCategoryAsync(int categoryId)
        {
            // Clear the current products
            Products.Clear();

            // Load all products from the data access layer
            var allProducts = await _productDao.GetAllAsync();

            // Filter products by the specified category ID
            var filteredProducts = allProducts.Where(p => p.Category_Id == categoryId);

            // Add the filtered products to the Products collection
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
            }
        }

    }
}