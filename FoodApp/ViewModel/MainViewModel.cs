using System.Collections.ObjectModel;
using FoodApp.Service.DataAccess;
using FoodApp.Helper;
using System.Linq;
using System.Collections.Specialized;
using FoodApp.Models;

namespace FoodApp.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly IDao<Product> _productDao;
        private ObservableCollection<Product> _products;
        private ObservableCollection<InvoiceItem> _invoiceItems;

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
                }
            }
        }

        public double TotalAmount => InvoiceItems.Sum(item => item.TotalPrice);

        public MainViewModel()
        {
            _productDao = new MockDao<Product>();
            Products = new ObservableCollection<Product>();
            InvoiceItems = new ObservableCollection<InvoiceItem>();
            InvoiceItems.CollectionChanged += InvoiceItems_CollectionChanged;
            LoadProducts();
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
                InvoiceItems.Add(new InvoiceItem { Product = product, Quantity = 1 });
            }
            OnPropertyChanged(nameof(TotalAmount));
            UpdateInvoiceItemIndexes();
        }

        private void InvoiceItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalAmount));
            UpdateInvoiceItemIndexes();
        }

        private void UpdateInvoiceItemIndexes()
        {
            for (int i = 0; i < InvoiceItems.Count; i++)
            {
                InvoiceItems[i].Index = i + 1; // Assuming you want 1-based index
            }
        }
    }
}