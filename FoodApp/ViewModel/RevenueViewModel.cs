using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using FoodApp.Service.DataAccess;

namespace FoodApp.ViewModel
{
    public class RevenueViewModel
    {
        public ObservableCollection<Detail> RevenueData { get; set; }
        private ObservableCollection<Detail> _allData;
        private readonly DetailDAO _detailDao;

        public RevenueViewModel()
        {
            _detailDao = new DetailDAO();
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var data = await _detailDao.GetAllAsync();
            _allData = new ObservableCollection<Detail>(data);
            RevenueData = new ObservableCollection<Detail>(_allData);
        }
        

        public void FilterRevenueData(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            // Debugging: Print the fromDate and toDate
            Console.WriteLine($"Filtering from: {fromDate} to: {toDate}");

            var filteredData = _allData.Where(d =>
            {
                // Debugging: Print each Created_At date
                Console.WriteLine($"Checking Created_At: {d.Created_At}");
                return d.Created_At >= fromDate && d.Created_At <= toDate;
            }).ToList();

            RevenueData.Clear();
            foreach (var item in filteredData)
            {
                RevenueData.Add(item);
            }

            // Debugging: Print the count of filtered data
            Console.WriteLine($"Filtered data count: {filteredData.Count}");
        }

        public float TotalRevenue => RevenueData?.Sum(d => d.Quantity * d.Unit_Price) ?? 0;

    }
}