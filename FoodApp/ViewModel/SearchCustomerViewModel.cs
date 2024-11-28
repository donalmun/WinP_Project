using FoodApp.Service.DataAccess;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace FoodApp.ViewModel
{
    public class SearchCustomerViewModel : INotifyPropertyChanged
    {
        private string _searchPhone;
        private string _customerInfo;
        private Visibility _customerVisibility;

        private readonly CustomerDAO _customerDao;

        public SearchCustomerViewModel()
        {
            _customerDao = new CustomerDAO();
            SearchCommand = new RelayCommand(async () => await SearchCustomerAsync(), CanSearch);
            CustomerVisibility = Visibility.Collapsed;
        }

        // Số điện thoại để tìm kiếm
        public string SearchPhone
        {
            get => _searchPhone;
            set
            {
                if (_searchPhone != value)
                {
                    _searchPhone = value;
                    OnPropertyChanged(nameof(SearchPhone));
                    ((RelayCommand)SearchCommand).RaiseCanExecuteChanged();
                }
            }
        }

        // Thông tin khách hàng
        public string CustomerInfo
        {
            get => _customerInfo;
            set
            {
                if (_customerInfo != value)
                {
                    _customerInfo = value;
                    OnPropertyChanged(nameof(CustomerInfo));
                }
            }
        }

        // Hiển thị hoặc ẩn thông tin khách hàng
        public Visibility CustomerVisibility
        {
            get => _customerVisibility;
            set
            {
                if (_customerVisibility != value)
                {
                    _customerVisibility = value;
                    OnPropertyChanged(nameof(CustomerVisibility));
                }
            }
        }

        // Lệnh tìm kiếm
        public ICommand SearchCommand { get; }

        // Kiểm tra khả năng thực thi lệnh tìm kiếm
        private bool CanSearch()
        {
            return !string.IsNullOrWhiteSpace(SearchPhone);
        }

        // Thực hiện tìm kiếm khách hàng
        private async Task SearchCustomerAsync()
        {
            var customer = await _customerDao.GetCustomerByPhoneAsync(SearchPhone.Trim());

            if (customer != null)
            {
                CustomerInfo = $"ID: {customer.Id}\n" +
                               $"Tên: {customer.Full_Name}\n" +
                               $"Email: {customer.Email}\n" +
                               $"Địa chỉ: {customer.Address}\n" +
                               $"Điểm thưởng: {customer.Loyalty_Points}\n" +
                               $"Ngày đăng ký: {customer.Created_At?.ToString("dd/MM/yyyy")}";
                CustomerVisibility = Visibility.Visible;
            }
            else
            {
                CustomerInfo = "Không tìm thấy khách hàng với số điện thoại đã nhập.";
                CustomerVisibility = Visibility.Collapsed;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // *** Lớp RelayCommand nội bộ được lồng bên trong SearchCustomerViewModel ***
        private class RelayCommand : ICommand
        {
            private readonly Func<Task> _executeAsync;
            private readonly Func<bool> _canExecute;

            public event EventHandler? CanExecuteChanged;

            public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
            {
                _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
                _canExecute = canExecute ?? (() => true);
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute();
            }

            public async void Execute(object? parameter)
            {
                await _executeAsync();
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}