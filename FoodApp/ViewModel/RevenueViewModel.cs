using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FoodApp.Models;
using FoodApp.Service.DataAccess;

namespace FoodApp.ViewModel
{
    public class RevenueViewModel
    {
        public ObservableCollection<Detail> RevenueData { get; set; }
        private ObservableCollection<Detail> _allData;

        public RevenueViewModel()
        {
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var mockDao = new MockDao<Detail>();
            var data = await mockDao.GetAllAsync();
            _allData = new ObservableCollection<Detail>(data);
            RevenueData = new ObservableCollection<Detail>(_allData);
        }

        public void FilterRevenueData(DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            var filteredData = _allData.Where(d => d.Created_At >= fromDate && d.Created_At <= toDate).ToList();
            RevenueData.Clear();
            foreach (var item in filteredData)
            {
                RevenueData.Add(item);
            }
        }
    }
}