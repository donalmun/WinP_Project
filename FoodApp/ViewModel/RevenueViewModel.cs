using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FoodApp.Service.DataAccess;
using FoodApp.Helper;

#nullable enable

namespace FoodApp.ViewModel
{
    public class RevenueViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Order> OrdersData { get; set; }
        private ObservableCollection<Order>? _allOrders;
        private readonly OrderDAO _orderDao;

        // Properties for Date Pickers
        private DateTimeOffset _fromDate = DateTimeOffset.Now.AddMonths(-1);
        public DateTimeOffset FromDate
        {
            get => _fromDate;
            set
            {
                if (_fromDate != value)
                {
                    _fromDate = value;
                    OnPropertyChanged(nameof(FromDate));
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private DateTimeOffset _toDate = DateTimeOffset.Now;
        public DateTimeOffset ToDate
        {
            get => _toDate;
            set
            {
                if (_toDate != value)
                {
                    _toDate = value;
                    OnPropertyChanged(nameof(ToDate));
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // RelayCommand for Filtering
        public RelayCommand FilterCommand { get; }

        // Action Delegate for Showing Messages
        public Action<string>? ShowMessageAction { get; set; }

        public RevenueViewModel()
        {
            _orderDao = new OrderDAO();
            OrdersData = new ObservableCollection<Order>();
            FilterCommand = new RelayCommand(ExecuteFilter, CanExecuteFilter);
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await LoadDataAsync();
            Console.WriteLine($"OrdersData Count: {OrdersData.Count}");
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var data = await _orderDao.GetAllAsync();
                if (data == null)
                {
                    Console.WriteLine("GetAllAsync returned null.");
                    return;
                }

                _allOrders = new ObservableCollection<Order>(data);
                Debug.WriteLine($"Data loaded: {_allOrders.Count} orders");

                // Update the existing OrdersData collection
                OrdersData.Clear();
                foreach (var order in _allOrders)
                {
                    OrdersData.Add(order);
                }

                OnPropertyChanged(nameof(TotalRevenue));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while loading data: {ex.Message}");
                Debug.WriteLine($"Exception: {ex}");
            }
        }

        private bool CanExecuteFilter()
        {
            return _allOrders != null && _allOrders.Any();
        }

        private void ExecuteFilter()
        {
            if (FromDate >= ToDate)
            {
                ShowMessageAction?.Invoke("From Date can not be greater or Equal than To Date");
                return;
            }

            FilterRevenueData(FromDate, ToDate);
        }

        public void FilterRevenueData(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            if (_allOrders == null) return;

            var filteredData = _allOrders
                .Where(o => o.Order_Date.Date >= fromDate.Date && o.Order_Date.Date <= toDate.Date)
                .ToList();

            Console.WriteLine($"All Orders Count: {_allOrders.Count}");
            Console.WriteLine($"Filtered Data Count: {filteredData.Count}");

            // Update the existing OrdersData collection
            OrdersData.Clear();
            foreach (var order in filteredData)
            {
                OrdersData.Add(order);
            }

            OnPropertyChanged(nameof(TotalRevenue));
        }

        public float TotalRevenue => OrdersData?.Sum(o => o.Total_Amount) ?? 0;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}