using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FoodApp.Service.DataAccess;
using FoodApp.Helper;
using System.Text.Json;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

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

        public string GetChartDataJson()
        {
            var chartData = OrdersData.Select(o => new { Date = o.Order_Date.ToString("MMM dd"), Amount = o.Total_Amount });
            return JsonSerializer.Serialize(chartData);
        }

        public async Task<string> GetMonthlyRevenueDataJsonAsync()
        {
            var chartData = _allOrders?
                .GroupBy(o => new { o.Order_Date.Year, o.Order_Date.Month })
                .Select(g => new { Month = $"{g.Key.Year}-{g.Key.Month}", Amount = g.Sum(o => o.Total_Amount) })
                .Select(cd => (dynamic)cd) // Cast to dynamic
                .ToList() ?? new List<dynamic>();

            return JsonSerializer.Serialize(new
            {
                labels = chartData.Select(cd => cd.Month).ToList(),
                data = chartData.Select(cd => cd.Amount).ToList()
            });
        }

        public async Task<string> GetDailyRevenueDataJsonAsync()
        {
            var chartData = _allOrders?
                .GroupBy(o => o.Order_Date.Date)
                .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Amount = g.Sum(o => o.Total_Amount) })
                .Select(cd => (dynamic)cd) // Cast to dynamic
                .ToList() ?? new List<dynamic>();

            return JsonSerializer.Serialize(new
            {
                labels = chartData.Select(cd => cd.Date).ToList(),
                data = chartData.Select(cd => cd.Amount).ToList()
            });
        }


        public async Task<string> GetSalesByCategoryDataJsonAsync()
        {
            var productDao = new ProductDao();
            var salesData = await productDao.GetSalesByCategoryAsync();

            var chartData = salesData.Select(kvp => new { Category = kvp.Key, Amount = kvp.Value }).ToList();

            return JsonSerializer.Serialize(new
            {
                labels = chartData.Select(cd => cd.Category).ToList(),
                data = chartData.Select(cd => cd.Amount).ToList(),
                colors = chartData.Select(cd => GetRandomColor()).ToList()
            });
        }


        // Helper method to generate random colors
        private string GetRandomColor()
        {
            var random = new Random();
            return $"rgba({random.Next(0,255)}, {random.Next(0,255)}, {random.Next(0,255)}, 0.6)";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}