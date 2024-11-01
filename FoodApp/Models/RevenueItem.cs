using System;
using System.ComponentModel;

namespace FoodApp.Models
{
    public class RevenueItem : INotifyPropertyChanged
    {
        private DateTime _date;
        private decimal _totalSales;
        private int _totalOrders;

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public decimal TotalSales
        {
            get => _totalSales;
            set
            {
                if (_totalSales != value)
                {
                    _totalSales = value;
                    OnPropertyChanged(nameof(TotalSales));
                }
            }
        }

        public int TotalOrders
        {
            get => _totalOrders;
            set
            {
                if (_totalOrders != value)
                {
                    _totalOrders = value;
                    OnPropertyChanged(nameof(TotalOrders));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}