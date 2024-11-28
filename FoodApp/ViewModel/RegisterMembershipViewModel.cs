using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FoodApp.Service.DataAccess;

namespace FoodApp.ViewModel
{
    public class RegisterMembershipViewModel : INotifyPropertyChanged
    {
        private string _fullName;
        private string _phone;
        private string _email;
        private string _address;
        private readonly CustomerDAO _customerDao;

        public RegisterMembershipViewModel()
        {
            _customerDao = new CustomerDAO();
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public ICommand RegisterCommand { get; }

        public event Action<string> ShowMessageRequested;
        public event Action NavigateToOrderPageRequested;

        private async Task RegisterAsync()
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Phone) ||
                string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Address))
            {
                ShowMessageRequested?.Invoke("Vui lòng điền đầy đủ thông tin.");
                return;
            }

            // Kiểm tra xem số điện thoại đã tồn tại chưa
            var existingCustomer = await _customerDao.GetCustomerByPhoneAsync(Phone);
            if (existingCustomer != null)
            {
                ShowMessageRequested?.Invoke("Số điện thoại đã được đăng ký.");
                return;
            }

            var customer = new Customer
            {
                Full_Name = FullName,
                Phone = Phone,
                Email = Email,
                Address = Address,
                Created_At = DateTime.Now
            };

            await _customerDao.AddAsync(customer);

            // Đăng ký thành công
            ShowMessageRequested?.Invoke("Đăng ký khách hàng thành công.");

            // Chuyển đến OrderPage
            NavigateToOrderPageRequested?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private event EventHandler _canExecuteChanged;

        public RelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public async void Execute(object parameter)
        {
            await _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                CommandManagerHelper.AddWeakReferenceHandler(ref _canExecuteChanged, value, 2);
            }
            remove
            {
                CommandManagerHelper.RemoveWeakReferenceHandler(_canExecuteChanged, value);
            }
        }
    }

    internal static class CommandManagerHelper
    {
        internal static void AddWeakReferenceHandler(ref EventHandler handler, EventHandler value, int defaultListSize)
        {
            var list = handler?.GetInvocationList();
            if (list != null)
            {
                foreach (var existingHandler in list)
                {
                    if (existingHandler.Target == value.Target && existingHandler.Method == value.Method)
                    {
                        return;
                    }
                }
            }

            handler += value;
        }

        internal static void RemoveWeakReferenceHandler(EventHandler handler, EventHandler value)
        {
            handler -= value;
        }
    }
}