// OrderViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FoodApp.Helper;
using FoodApp.Service.DataAccess;
using System.Linq;

namespace FoodApp.ViewModels
{
    public class OrderViewModel : BindableBase
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
        private Detail _selectedDetailItem;

        private string _searchKeyword;
        private string _phoneNumber;

        private ObservableCollection<Customer> _suggestedCustomers;

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

        // OrderViewModel.cs
        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (SetProperty(ref _selectedTable, value))
                {
                    if (_selectedTable != null)
                    {
                        // Call the async method to handle table selection
                        SetSelectedTableAsync(_selectedTable);
                    }
                    else
                    {
                        // Clear details and customer info if no table is selected
                        Details.Clear();
                        SelectedCustomer = null;
                    }
                }
            }
        }

        public void ClearOrder()
        {
            Details.Clear();
            SelectedCustomer = null;
            SelectedTable = null;
            OnPropertyChanged(nameof(TotalAmount));
        }

        private async void SetSelectedTableAsync(Table selectedTable)
        {
            // Update table status
            await UpdateTableStatus(selectedTable, 1); // Assuming 1 represents 'in use'

            // Load the order for the selected table
            await LoadOrderForSelectedTableAsync(selectedTable);
        }

        // Implement the method to load order details and customer info
        private async Task LoadOrderForSelectedTableAsync(Table selectedTable)
        {
            var orders = await _orderDao.GetOrdersByTableIdAsync(selectedTable.Id);

            if (orders != null && orders.Any())
            {
                var latestOrder = orders.OrderByDescending(o => o.Order_Date).FirstOrDefault();

                if (latestOrder != null)
                {
                    await _orderDao.LoadOrderDetailsAsync(latestOrder);
                    Details = new ObservableCollection<Detail>(latestOrder.Details);
                    SelectedCustomer = latestOrder.Customer;
                }
            }
            else
            {
                Details.Clear();
                SelectedCustomer = null;
            }

            OnPropertyChanged(nameof(TotalAmount));
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public ObservableCollection<Customer> SuggestedCustomers
        {
            get => _suggestedCustomers;
            set => SetProperty(ref _suggestedCustomers, value);
        }

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

        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public ICommand SaveOrderCommand { get; }
        public ICommand PaymentCommand { get; }
        public ICommand SearchCommand { get; }

        public OrderViewModel()
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
            SaveOrderCommand = new AsyncRelayCommand(() => SaveOrderAsync(false));

            // Initialize the PaymentCommand
            PaymentCommand = new RelayCommand(async () => await ExecutePaymentAsync());
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

        private async Task ExecutePaymentAsync()
        {
            // Save the order details to the database
            await SaveOrderAsync(true);

            // Update the table status if needed
            if (SelectedTable != null)
            {
                await UpdateTableStatus(SelectedTable, 1);
            }

            // Show a message or perform any other actions needed after payment
            ShowMessage("Payment successful and order saved.");
        }
        private async void LoadTables()
        {
            try
            {
                var tables = await _tableDao.GetAllAsync();
                var emptyTables = tables.Where(t => t.Status == 0);
                Tables = new ObservableCollection<Table>(emptyTables);
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        private async void LoadProducts()
        {
            var products = await _productDao.GetAllAsync();
            _allProducts = new ObservableCollection<Product>(products);
            Products = new ObservableCollection<Product>(_allProducts);
        }

        private async void TestDatabaseConnection()
        {
            bool isConnected = await _productDao.TestConnectionAsync();
            if (!isConnected)
            {
                // Handle connection failure
            }
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

        private void DetailItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
            Products.Clear();
            var allProducts = await _productDao.GetAllAsync();
            var filteredProducts = allProducts.Where(p => p.Category_Id == categoryId);
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
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

        // OrderViewModel.cs
        public async Task SaveOrderAsync(bool isPayment)
        {
            if (SelectedTable == null || Details == null || Details.Count == 0)
            {
                ShowMessage("Please select a table and add items to the order.");
                return;
            }

            var order = new Order
            {
                Order_Date = DateTime.Now,
                Total_Amount = (float)TotalAmount,
                Status = (byte)(isPayment ? 1 : 0),
                Customer_Id = SelectedCustomer?.Id,
                Table_Id = SelectedTable?.Id,
                Details = Details.ToList()
            };

            Console.WriteLine("order status: " + order.Status);

            try
            {
                await _orderDao.AddAsync(order);

                if (SelectedCustomer != null)
                {
                    // Increase customer's loyalty points based on the total amount
                    await UpdateCustomerLoyaltyPointsAsync(SelectedCustomer, TotalAmount);
                }

                // Optionally, clear the form or display a success message
            }
            catch (Exception ex)
            {
                ShowMessage($"Đã xảy ra lỗi: {ex.Message}");

            }
        }
        
        private async Task UpdateTableStatus(Table table, int status)
        {
            Console.WriteLine("table status before: " + table.Status);
            table.Status = status; // 1 for 'in use', 0 for 'empty'
            Console.WriteLine("table status after: " + table.Status);
            await _tableDao.UpdateAsync(table);
        }

        
        private async Task UpdateCustomerLoyaltyPointsAsync(Customer customer, double amount)
        {
            // Example: 1 point for every 1000 units of currency spent
            
            int pointsToAdd = (int)(amount / 1000);
            Console.WriteLine("customer loyalty_point before: " + customer.Loyalty_Points);
            customer.Loyalty_Points += pointsToAdd;
            Console.WriteLine("customer loyalty_point after: " + customer.Loyalty_Points);
            await _customerDao.UpdateAsync(customer);
        }

        private void ShowMessage(string message)
        {
            // Vì ViewModel không có quyền truy cập vào XamlRoot, bạn cần sử dụng một cách khác để hiển thị dialog.
            // Một cách là sử dụng sự kiện hoặc hành động được gán từ code-behind.
            ShowMessageAction?.Invoke(message);
        }

        public Action<string>? ShowMessageAction { get; set; }
    }
}
