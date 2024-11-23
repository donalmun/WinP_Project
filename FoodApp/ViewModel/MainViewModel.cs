using System.Collections.ObjectModel;
using FoodApp.Service.DataAccess;
using FoodApp.Helper;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FoodApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDao<Product> _productDao;
        private ObservableCollection<Product> _products;
        private ObservableCollection<InvoiceItem> _invoiceItems;

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

        public MainViewModel()
        {
            //_productDao = new MockDao<Product>();
            _productDao =  new ProductDao();
            Products = new ObservableCollection<Product>();
            InvoiceItems = new ObservableCollection<InvoiceItem>();
            InvoiceItems.CollectionChanged += InvoiceItems_CollectionChanged;

            // Kiểm tra kết nối cơ sở dữ liệu
            TestDatabaseConnection();

            LoadProducts();

            SelectedInvoiceItem = new InvoiceItem
            {
                SurchargeType = "%",
                DiscountType = "%"
            };
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
            foreach (var product in products)
            {
                Products.Add(product);
            }
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