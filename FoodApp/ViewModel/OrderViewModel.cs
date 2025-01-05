// FoodApp\ViewModels\OrderViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FoodApp.Helper;
using FoodApp.Service.DataAccess;
using System.Linq;
using System.Diagnostics;

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

        private const int LoyaltyPointsThreshold = 1000;
        private const double DiscountPercentage = 10.0;

        private bool _areTablesAvailable;
        public bool AreTablesAvailable
        {
            get => _areTablesAvailable;
            set => SetProperty(ref _areTablesAvailable, value);
        }

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
            set
            {
                if (SetProperty(ref _selectedTable, value))
                {
                    if (_selectedTable != null)
                    {
                        // Remove the call to set table status here
                        // SetSelectedTableAsync(_selectedTable); // Remove or comment out this line
                        LoadOrderForSelectedTableAsync(_selectedTable).ConfigureAwait(false);
                    }
                    else
                    {
                        // Clear the order if no table is selected
                        ClearOrder();
                    }
                }
            }
        }

        private double _orderTaxPercentage;
        public double OrderTaxPercentage
        {
            get => _orderTaxPercentage;
            set
            {
                if (SetProperty(ref _orderTaxPercentage, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(currentTotal));
                }
            }
        }

        private double _orderDiscountPercentage;
        public double OrderDiscountPercentage
        {
            get => _orderDiscountPercentage;
            set
            {
                if (SetProperty(ref _orderDiscountPercentage, value))
                {
                    OnPropertyChanged(nameof(TotalAmount));
                    OnPropertyChanged(nameof(currentTotal));
                }
            }
        }

        public void ClearOrder()
        {
            Details.Clear();
            SelectedCustomer = null;
            // Do not reset SelectedTable here to keep the current table selected

            // Reset discount and tax percentages
            OrderDiscountPercentage = 0;
            OrderTaxPercentage = 0;

            // Notify that TotalAmount and currentTotal have changed
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(currentTotal));

            _isDiscountApplied = false; // Reset the discount flag
        }

        // Changed from async void to async Task for better exception handling
        private async Task SetSelectedTableAsync(Table selectedTable)
        {
            // Update table status to 'in use' (1)
            await UpdateTableStatusAsync(selectedTable, 1);

            // Load the order for the selected table (currently clears the order)
            await LoadOrderForSelectedTableAsync(selectedTable);

            // Reload the tables list to exclude the selected table
            await LoadTablesAsync();
        }

        // Implement the method to load order details and customer info
        private async Task LoadOrderForSelectedTableAsync(Table selectedTable)
        {
            // Always clear the current order details when a new table is selected
            ClearOrder();

            // Optionally, you can still load existing orders if needed in the future
            // var orders = await _orderDao.GetOrdersByTableIdAsync(selectedTable.Id);
            // if (orders != null && orders.Any())
            // {
            //     var latestOrder = orders.OrderByDescending(o => o.Order_Date).FirstOrDefault();
            //     if (latestOrder != null)
            //     {
            //         await _orderDao.LoadOrderDetailsAsync(latestOrder);
            //         Details = new ObservableCollection<Detail>(latestOrder.Details);
            //         SelectedCustomer = latestOrder.Customer;
            //     }
            // }

            // Notify that TotalAmount and currentTotal have changed after clearing
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(currentTotal));
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
                    OnPropertyChanged(nameof(currentTotal));
                    SubscribeToDetailItemChanges();
                }
            }
        }

        public double currentTotal => Details.Sum(item => item.Sub_Total);

        public double TotalAmount => CalculateTotalAmount();

        public double CalculateTotalAmount()
        {
            double subtotal = Details.Sum(item => item.Sub_Total);
            double taxAmount = subtotal * (Math.Max(0, Math.Min(OrderTaxPercentage, 100)) / 100);
            double total = subtotal + taxAmount;
            double discountAmount = total * (Math.Max(0, Math.Min(OrderDiscountPercentage, 100)) / 100);
            return total - discountAmount;
        }

        public void UpdateOrderDiscountTax(double tax, double discount)
        {
            OrderTaxPercentage = tax;
            OrderDiscountPercentage = discount;
        }

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

        // Action to display messages via DialogService
        public Func<string, Task>? ShowMessageAction { get; set; }

        // Action to be invoked when payment is requested
        public Action? PaymentRequested { get; set; }

        // Flag to track discount application
        private bool _isDiscountApplied = false;

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

            // Initialize the PaymentCommand to include discount logic
            PaymentCommand = new AsyncRelayCommand(HandlePaymentAsync);
            SuggestedCustomers = new ObservableCollection<Customer>();

            // Check database connection
            TestDatabaseConnection();

            LoadProducts();
            LoadTablesAsync().ConfigureAwait(false); // Changed to async Task and called without await

            OrderTaxPercentage = 0;
            OrderDiscountPercentage = 0;

            SelectedDetailItem = new Detail
            {
                SurchargeType = "%",
                DiscountType = "%"
            };
        }

        private async Task HandlePaymentAsync()
        {
            // Kiểm tra có chọn khách hàng hay chưa
            if (SelectedCustomer != null)
            {
                // Kiểm tra số điểm khách hàng
                if (SelectedCustomer.Loyalty_Points >= 1000)
                {
                    // Giảm 10% tổng hóa đơn
                    double discountAmount = TotalAmount * 0.1;
                    OrderDiscountPercentage = 10;   // Cập nhật UI nếu cần
                    _isDiscountApplied = true;

                    // Trừ 10%
                    // Lưu ý: sau khi set OrderDiscountPercentage = 10 thì TotalAmount đã tự tính lại 
                    // nên ở đây *có thể* bạn không cần trừ thủ công. 
                    // Nếu vẫn muốn làm thủ công thì:
                    // TotalAmount -= discountAmount;

                    // Trừ 1000 điểm
                    SelectedCustomer.Loyalty_Points -= 1000;

                    // Thông báo
                    await ShowMessage("Bạn đã dùng 1000 điểm để được giảm giá 10% cho đơn hàng này.");
                }
                else
                {
                    // Nếu không đủ 1000 điểm thì chỉ cộng điểm
                    await AwardLoyaltyPointsAsync();
                    await ShowMessage("Chưa đủ 1000 điểm để giảm giá. Điểm của bạn đã được cộng thêm.");
                }
            }

            // Mở popup payment
            PaymentRequested?.Invoke();
        }

        private void ApplyDiscount()
        {
            // Apply a 10% discount
            OrderDiscountPercentage = DiscountPercentage; // Triggers OnPropertyChanged for TotalAmount
            _isDiscountApplied = true;

            // Deduct 1500 loyalty points
            SelectedCustomer.Loyalty_Points -= LoyaltyPointsThreshold;
            // Note: Update the customer in the database after saving the order

            // Notify the user about the applied discount
            //ShowMessage("Đơn hàng được giảm giá 10% do khách hàng có đủ điểm thưởng.").ConfigureAwait(false);
        }

        private async Task AwardLoyaltyPointsAsync()
        {
            if (SelectedCustomer != null && TotalAmount > 0)
            {
                await UpdateCustomerLoyaltyPointsAsync(SelectedCustomer, TotalAmount);
                await ShowMessage("Bạn đã nhận được điểm thưởng cho đơn hàng này!").ConfigureAwait(false);
            }
        }

        private async Task LoadTablesAsync()
        {
            try
            {
                var tables = await _tableDao.GetAllAsync();
                var emptyTables = tables.Where(t => t.Status == 0);
                Tables = new ObservableCollection<Table>(emptyTables);

                // Update the availability flag
                AreTablesAvailable = Tables.Any();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Debug.WriteLine($"Error loading tables: {ex.Message}");
                await ShowMessage("Không thể tải danh sách bàn.");
                AreTablesAvailable = false;
            }
        }

        private async void LoadProducts()
        {
            try
            {
                var products = await _productDao.GetAllAsync();
                _allProducts = new ObservableCollection<Product>(products);
                Products = new ObservableCollection<Product>(_allProducts);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading products: {ex.Message}");
                await ShowMessage("Không thể tải danh sách sản phẩm.");
            }
        }

        private async void TestDatabaseConnection()
        {
            bool isConnected = await _productDao.TestConnectionAsync();
            if (!isConnected)
            {
                // Handle connection failure
                await ShowMessage("Database connection failed. Please check your settings.");
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
            OnPropertyChanged(nameof(currentTotal));
        }

        public void RemoveFromDetail(Detail detail)
        {
            if (Details.Contains(detail))
            {
                detail.PropertyChanged -= DetailItem_PropertyChanged;
                Details.Remove(detail);
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(currentTotal));
            }
        }

        private void DetailItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(currentTotal));
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
                OnPropertyChanged(nameof(currentTotal));
            }
        }

        public async Task FilterProductsByCategoryAsync(int categoryId)
        {
            if (categoryId == 0)
            {
                // Display all products
                Products = new ObservableCollection<Product>(_allProducts);
            }
            else
            {
                // Filter products by the selected category
                Products = new ObservableCollection<Product>(_allProducts.Where(p => p.Category_Id == categoryId));
            }

            // Notify that the Products collection has changed
            OnPropertyChanged(nameof(Products));
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

        public async Task SaveOrderAsync(bool isPayment)
        {
            if (SelectedTable == null || Details == null || Details.Count == 0)
            {
                await ShowMessage("Vui lòng chọn bàn và thêm món vào đơn hàng.");
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

            try
            {
                await _orderDao.AddAsync(order);

                if (_isDiscountApplied)
                {
                    await ShowMessage("Đơn hàng được giảm giá 10% do khách hàng có đủ điểm thưởng.");
                    // Loyalty points deduction already handled in ApplyDiscount
                    await _customerDao.UpdateAsync(SelectedCustomer);
                }
                else
                {
                    await ShowMessage("Đơn hàng đã được lưu thành công.");
                }

                // Award loyalty points if no discount was applied
                // if (!_isDiscountApplied)
                // {
                //     await AwardLoyaltyPointsAsync();
                // }

                if (isPayment)
                {
                    // Set table status to 'occupied'
                    await UpdateTableStatusAsync(SelectedTable, 1);

                    // Reset the order if it's a payment
                    ClearOrder();
                    _isDiscountApplied = false; // Reset the discount flag for the next order
                }

                // Reload the tables list after saving the order
                await LoadTablesAsync();
            }
            catch (Exception ex)
            {
                await ShowMessage($"Lỗi: {ex.Message}");
            }
        }

        private async Task UpdateTableStatusAsync(Table table, int status)
        {
            try
            {
                Debug.WriteLine("table status before: " + table.Status);
                table.Status = status; // 1 for 'in use', 0 for 'empty'
                Debug.WriteLine("table status after: " + table.Status);
                await _tableDao.UpdateAsync(table);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating table status: {ex.Message}");
                await ShowMessage("Không thể cập nhật trạng thái bàn.");
            }
        }

        public async Task UpdateCustomerLoyaltyPointsAsync(Customer customer, double amount)
        {
            // 1 point for every 1000 units of currency spent, minimum 1 point if amount >= 1000
            int pointsToAdd = (int)(amount / 1000);
            if (pointsToAdd < 1 && amount >= 1000)
            {
                pointsToAdd = 1;
            }

            if (pointsToAdd > 0)
            {
                Debug.WriteLine($"Adding {pointsToAdd} loyalty points to customer ID: {customer.Id}");
                customer.Loyalty_Points += pointsToAdd;
                Debug.WriteLine($"Customer loyalty points after addition: {customer.Loyalty_Points}");
                await _customerDao.UpdateAsync(customer);
            }
        }

        private async Task ShowMessage(string message)
        {
            // Use the action provided by the View to display messages
            if (ShowMessageAction != null)
            {
                await ShowMessageAction(message);
            }
        }
    }
}
