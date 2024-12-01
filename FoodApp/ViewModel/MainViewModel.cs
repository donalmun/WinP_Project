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
        private readonly OrderDAO _orderDao;
        private readonly IDao<Product> _productDao;
        private readonly CustomerDAO _customerDao;
        private readonly TableDAO _tableDao;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Product> _allProducts;
        private ObservableCollection<Detail> _details;
        private ObservableCollection<Table> _tables;
        private Product _selectedProduct;
        private Table _selectedTable;
        private Customer _selectedCustomer;

        public Product SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public ObservableCollection<Table> Tables
        {
            get => _tables;
            set => SetProperty(ref _tables, value);
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private ObservableCollection<Customer> _suggestedCustomers;
        public ObservableCollection<Customer> SuggestedCustomers
        {
            get => _suggestedCustomers;
            set => SetProperty(ref _suggestedCustomers, value);
        }

        private Detail _selectedDetailItem;

        public Detail SelectedDetailItem
        {
            get => _selectedDetailItem;
            set => SetProperty(ref _selectedDetailItem, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        public ObservableCollection<Detail> Details
        {
            get => _details;
            set
            {
                if (SetProperty(ref _details, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                    SubscribeToDetailItemChanges();
                }
            }
        }

        public double TotalAmount => Details.Sum(item => item.Sub_Total);

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

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public ICommand SaveProductCommand { get; }
        public ICommand AddNewProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SaveOrderCommand { get; }
        public ICommand SearchCustomerCommand { get; }

        public MainViewModel()
        {
            _orderDao = new OrderDAO();
            _productDao = new ProductDao();
            _customerDao = new CustomerDAO();
            _tableDao = new TableDAO();
            Products = new ObservableCollection<Product>();
            Details = new ObservableCollection<Detail>();
            Tables = new ObservableCollection<Table>();
            Details.CollectionChanged += DetailItems_CollectionChanged;

            SearchCommand = new RelayCommand(() => SearchProducts());
            SaveProductCommand = new RelayCommand(() => SaveProduct());
            AddNewProductCommand = new RelayCommand(() => AddNewProduct());
            EditProductCommand = new RelayCommand<Product>(EditProduct);
            DeleteProductCommand = new RelayCommand<Product>(DeleteProduct);
            SaveOrderCommand = new RelayCommand(async () => await SaveOrderAsync());
            SuggestedCustomers = new ObservableCollection<Customer>();

            // Check database connection
            TestDatabaseConnection();

            LoadProducts();
            LoadTables();

            SelectedDetailItem = new Detail
            {
                SurchargeType = "%",
                DiscountType = "%"
            };
        }

        private async void LoadTables()
        {
            try
            {
                var tables = await _tableDao.GetAllAsync();
                Tables = new ObservableCollection<Table>(tables);
                Debug.WriteLine($"Loaded {Tables.Count} tables.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading tables: {ex.Message}");
            }
        }

        private async Task SaveOrderAsync()
        {
            var order = new Order
            {
                Order_Date = DateTime.Now,
                Total_Amount = (float)TotalAmount,
                Status = 1,
                Customer_Id = SelectedCustomer?.Id,
                Table_Id = SelectedTable?.Id,
                Details = Details.ToList()
            };

            try
            {
                await _orderDao.AddAsync(order);
                DisplayMessage("Order and details saved successfully.");
            }
            catch (Exception ex)
            {
                DisplayMessage($"An error occurred while saving the order: {ex.Message}");
            }
        }

        public async Task SearchCustomersAsync(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                var filteredCustomers = await _customerDao.SearchCustomersByPhoneAsync(query);

                SuggestedCustomers.Clear();
                foreach (var customer in filteredCustomers)
                {
                    SuggestedCustomers.Add(customer);
                }
            }
            else
            {
                SuggestedCustomers.Clear();
            }
        }

        private void DisplayMessage(string message)
        {
            // Implement a method to display messages to the user
            Debug.WriteLine(message);
        }

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
                // Handle connection failure (e.g., display an error message to the user)
                Debug.WriteLine("Failed to connect to the database.");
            }
        }

        private async void LoadProducts()
        {
            var products = await _productDao.GetAllAsync();
            _allProducts = new ObservableCollection<Product>(products);
            Products = new ObservableCollection<Product>(_allProducts);
        }

        public void AddToDetail(Product product)
        {
            var existingItem = Details.FirstOrDefault(i => i.Product_Id == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
                existingItem.Sub_Total = existingItem.Quantity * existingItem.Unit_Price;
            }
            else
            {
                var newItem = new Detail
                {
                    Product_Id = product.Id,
                    Product = product,
                    Quantity = 1,
                    Unit_Price = product.Cost,
                    Sub_Total = product.Cost,
                    DiscountType = "%",
                    SurchargeType = "%",
                };
                newItem.PropertyChanged += DetailItem_PropertyChanged;
                Details.Add(newItem);
            }
            OnPropertyChanged(nameof(TotalAmount));
        }

        public void RemoveFromDetail(Detail detail)
        {
            if (Details.Contains(detail))
            {
                detail.PropertyChanged -= DetailItem_PropertyChanged;
                Details.Remove(detail);
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void DetailItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalAmount));
            SubscribeToDetailItemChanges();
        }

        private void SubscribeToDetailItemChanges()
        {
            foreach (var item in Details)
            {
                item.PropertyChanged -= DetailItem_PropertyChanged;
                item.PropertyChanged += DetailItem_PropertyChanged;
            }
        }

        private void DetailItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Detail.Sub_Total))
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
