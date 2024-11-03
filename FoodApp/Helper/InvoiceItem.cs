using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FoodApp.Helper
{
    public class InvoiceItem : INotifyPropertyChanged
    {
        private int _quantity;
        private double _totalPrice;
        private double _unitPrice;
        private Product _product;
        private string _note;
        private double _discount;
        private double _surcharge;
        private string _discountType;
        private string _surchargeType;

        public int Index { get; set; }

        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        public Product Product
        {
            get => _product;
            set
            {
                if (_product != value)
                {
                    _product = value;
                    UnitPrice = _product?.Cost ?? 0;
                    CalculateTotalPrice();
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UnitPrice));
                }
            }
        }

        public double Discount
        {
            get => _discount;
            set
            {
                if (_discount != value)
                {
                    _discount = value;
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(Discount));
                }
            }
        }

        public string DiscountType
        {
            get => _discountType;
            set
            {
                if (_discountType != value)
                {
                    _discountType = value;
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(DiscountType));
                }
            }
        }

        public double Surcharge
        {
            get => _surcharge;
            set
            {
                if (_surcharge != value)
                {
                    _surcharge = value;
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(Surcharge));
                }
            }
        }

        public string SurchargeType
        {
            get => _surchargeType;
            set
            {
                if (_surchargeType != value)
                {
                    _surchargeType = value;
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(SurchargeType));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    CalculateTotalPrice();
                    OnPropertyChanged();
                }
            }
        }

        public double TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public double UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(UnitPrice));
                }
            }
        }

        private void CalculateTotalPrice()
        {
            if (Product != null && Quantity > 0)
            {
                double discountAmount = DiscountType == "%" ? (UnitPrice * Quantity * Discount / 100) : Discount;
                double surchargeAmount = SurchargeType == "%" ? (UnitPrice * Quantity * Surcharge / 100) : Surcharge;

                Debug.WriteLine($"Discount: {discountAmount}, Surcharge: {surchargeAmount}");
                Debug.WriteLine($"DiscountType: {DiscountType}, SurchargeType: {SurchargeType}");
                TotalPrice = (UnitPrice * Quantity) - discountAmount + surchargeAmount;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}