﻿// WinP_Project\FoodApp\ViewModel\SearchCustomerViewModel.cs
using FoodApp.Service.DataAccess;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
            DeleteCommand = new RelayCommand<XamlRoot>(async (xamlRoot) => await DeleteCustomerAsync(xamlRoot));

            CustomerVisibility = Visibility.Collapsed;
        }

        // Added DeleteCommand property
        public ICommand DeleteCommand { get; }

        private async Task DeleteCustomerAsync(XamlRoot xamlRoot)
        {
            var dialog = new ContentDialog
            {
                Title = "Xác nhận xóa",
                Content = "Bạn có chắc chắn muốn xóa khách hàng này không?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = xamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await _customerDao.DeleteCustomerByPhoneAsync(SearchPhone.Trim());
                CustomerInfo = string.Empty;
                CustomerVisibility = Visibility.Collapsed;
                OnShowMessageRequested("Thông báo", "Khách hàng đã được xóa.");
            }
        }
        // Event to notify the View to display messages
        public event EventHandler<MessageEventArgs>? ShowMessageRequested;

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
                CustomerInfo = $"Tên: {customer.Full_Name}\n" +
                               $"Email: {customer.Email}\n" +
                               $"Địa chỉ: {customer.Address}\n" +
                               $"Điểm thưởng: {customer.Loyalty_Points}\n" +
                               $"Ngày đăng ký: {customer.Created_At?.ToString("dd/MM/yyyy")}";
                CustomerVisibility = Visibility.Visible;
            }
            else
            {
                // Invoke the event with appropriate title and message
                OnShowMessageRequested("Thông báo", "Số điện thoại không tồn tại.");
            }
        }

        // Method to raise the ShowMessageRequested event
        protected virtual void OnShowMessageRequested(string title, string message)
        {
            ShowMessageRequested?.Invoke(this, new MessageEventArgs(title, message));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // *** Updated RelayCommand classes ***

        // Non-generic RelayCommand for commands without parameters
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

        // Generic RelayCommand for commands with parameters
        private class RelayCommand<T> : ICommand
        {
            private readonly Func<T, Task> _executeAsync;
            private readonly Func<T, bool>? _canExecute;

            public event EventHandler? CanExecuteChanged;

            public RelayCommand(Func<T, Task> executeAsync, Func<T, bool>? canExecute = null)
            {
                _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
                _canExecute = canExecute;
            }

            public bool CanExecute(object? parameter)
            {
                if (_canExecute == null)
                    return true;
                if (parameter is T t)
                    return _canExecute(t);
                return false;
            }

            public async void Execute(object? parameter)
            {
                if (parameter is T t)
                    await _executeAsync(t);
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    // Custom EventArgs to hold message information
    public class MessageEventArgs : EventArgs
    {
        public string Title { get; }
        public string Content { get; }

        public MessageEventArgs(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}