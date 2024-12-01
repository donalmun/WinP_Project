using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp
{
    public class Detail : INotifyPropertyChanged
    {
        private int _quantity;
        private double _subTotal;
        private double _unitPrice;
        private Product _product;
        private string _note;
        private double _discount;
        private double _surcharge;
        private string _discountType;
        private string _surchargeType;
        private int _id;
        private int _orderId;
        private int _productId;


        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public int Order_Id
        {
            get => _orderId;
            set
            {
                if (_orderId != value)
                {
                    _orderId = value;
                    OnPropertyChanged(nameof(Order_Id));
                }
            }
        }

        public int Product_Id
        {
            get => _productId;
            set
            {
                if (_productId != value)
                {
                    _productId = value;
                    OnPropertyChanged(nameof(Product_Id));
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

        public float Unit_Price
        {
            get => (float)_unitPrice;
            set
            {
                if (_unitPrice != value)
                {
                    _unitPrice = value;
                    CalculateTotalPrice();
                    OnPropertyChanged(nameof(Unit_Price));
                }
            }
        }

        public float Sub_Total
        {
            get => (float)_subTotal;
            set
            {
                if (_subTotal != value)
                {
                    _subTotal = value;
                    OnPropertyChanged(nameof(Sub_Total));
                }
            }
        }

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
                    Unit_Price = (float)(_product?.Cost ?? 0);
                    CalculateTotalPrice();
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Unit_Price));
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

        private void CalculateTotalPrice()
        {
            if (Product != null && Quantity > 0)
            {
                double discountAmount = DiscountType == "%" ? (Unit_Price * Quantity * Discount / 100) : Discount;
                double surchargeAmount = SurchargeType == "%" ? (Unit_Price * Quantity * Surcharge / 100) : Surcharge;

                Debug.WriteLine($"Discount: {discountAmount}, Surcharge: {surchargeAmount}");
                Debug.WriteLine($"DiscountType: {DiscountType}, SurchargeType: {SurchargeType}");
                Sub_Total = (float)((Unit_Price * Quantity) - discountAmount + surchargeAmount);
                OnPropertyChanged(nameof(Sub_Total));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
