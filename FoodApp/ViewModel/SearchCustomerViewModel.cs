﻿// WinP_Project\FoodApp\ViewModel\SearchCustomerViewModel.cs
using FoodApp.Helper;
using FoodApp.Service.DataAccess;
using FoodApp.Views;
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
            SearchCommand = new AsyncRelayCommand(SearchCustomerAsync, CanSearch);
            DeleteCommand = new AsyncRelayCommand<XamlRoot>(DeleteCustomerAsync);
            EditCommand = new AsyncRelayCommand<XamlRoot>(EditCustomerAsync);

            CustomerVisibility = Visibility.Collapsed;
        }

        // EditCommand property
        public ICommand EditCommand { get; }

        // DeleteCommand property
        public ICommand DeleteCommand { get; }

        // SearchCommand property
        public ICommand SearchCommand { get; }

        // SearchPhone property
        public string SearchPhone
        {
            get => _searchPhone;
            set
            {
                if (_searchPhone != value)
                {
                    _searchPhone = value;
                    OnPropertyChanged(nameof(SearchPhone));
                    ((AsyncRelayCommand)SearchCommand).RaiseCanExecuteChanged();
                }
            }
        }

        // CustomerInfo property
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

        // CustomerVisibility property
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

        // CanSearch method
        private bool CanSearch()
        {
            return !string.IsNullOrWhiteSpace(SearchPhone);
        }

        // SearchCustomerAsync method
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
                OnShowMessageRequested("Thông báo", "Số điện thoại không tồn tại.");
            }
        }

        // DeleteCustomerAsync method
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

        // EditCustomerAsync method
        private async Task EditCustomerAsync(XamlRoot xamlRoot)
        {
            var customer = await _customerDao.GetCustomerByPhoneAsync(SearchPhone.Trim());
            if (customer == null)
            {
                OnShowMessageRequested("Thông báo", "Không tìm thấy khách hàng để chỉnh sửa.");
                return;
            }

            var dialog = new ContentDialog
            {
                Title = "Chỉnh sửa thông tin khách hàng",
                PrimaryButtonText = "Lưu",
                CloseButtonText = "Hủy",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = xamlRoot,
                Content = new EditCustomerControl(customer) // EditCustomerControl UserControl
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (dialog.Content is EditCustomerControl editControl)
                {
                    var updatedCustomer = editControl.UpdatedCustomer;
                    await _customerDao.UpdateAsync(updatedCustomer);
                    CustomerInfo = $"Tên: {updatedCustomer.Full_Name}\n" +
                                   $"Email: {updatedCustomer.Email}\n" +
                                   $"Địa chỉ: {updatedCustomer.Address}\n" +
                                   $"Điểm thưởng: {updatedCustomer.Loyalty_Points}\n" +
                                   $"Ngày đăng ký: {updatedCustomer.Created_At?.ToString("dd/MM/yyyy")}";
                    OnShowMessageRequested("Thông báo", "Thông tin khách hàng đã được cập nhật.");
                }
            }
        }

        // Event to notify the View to display messages
        public event EventHandler<MessageEventArgs>? ShowMessageRequested;

        // Method to raise the ShowMessageRequested event
        protected virtual void OnShowMessageRequested(string title, string message)
        {
            ShowMessageRequested?.Invoke(this, new MessageEventArgs(title, message));
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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