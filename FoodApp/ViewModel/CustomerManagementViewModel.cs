using FoodApp.Service.DataAccess;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FoodApp.Views;
using FoodApp.Helper;
using FoodApp.Views.Controls;

namespace FoodApp.ViewModels
{
    public class CustomerManagementViewModel : INotifyPropertyChanged
    {
        public XamlRoot XamlRoot { get; set; }
        private string _searchPhone;
        private string _fullName;
        private string _phone;
        private string _email;
        private string _address;
        private Customer _selectedCustomer;

        private readonly CustomerDAO _customerDao;

        public CustomerManagementViewModel()
        {
            _customerDao = new CustomerDAO();
            Customers = new ObservableCollection<Customer>();
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
            SearchCommand = new RelayCommand(async () => await SearchAsync(), () => !string.IsNullOrWhiteSpace(SearchPhone));
            DeleteCustomerCommand = new RelayCommand<Customer>(async (customer) => await DeleteCustomerAsync(customer));
            EditCustomerCommand = new RelayCommand<Customer>(async (customer) => await EditCustomerAsync(customer));
        }

        public ObservableCollection<Customer> Customers { get; }

        public string SearchPhone
        {
            get => _searchPhone;
            set
            {
                _searchPhone = value;
                OnPropertyChanged(nameof(SearchPhone));
                ((RelayCommand)SearchCommand).RaiseCanExecuteChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(nameof(Phone)); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(nameof(Address)); }
        }

        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(nameof(SelectedCustomer)); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand DeleteCustomerCommand { get; }
        public ICommand EditCustomerCommand { get; }

        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Address))
            {
                await ShowMessageAsync("Error", "Please fill in all fields.");
                return;
            }

            var existingCustomer = await _customerDao.GetCustomerByPhoneAsync(Phone);
            if (existingCustomer != null)
            {
                await ShowMessageAsync("Error", "Phone number is already registered.");
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
            Customers.Add(customer);

            await ShowMessageAsync("Success", "Customer registered successfully.");
            ClearRegistrationFields();

            SelectedCustomer = null;
        }

        private async Task SearchAsync()
        {
            Customers.Clear();
            var customer = await _customerDao.GetCustomerByPhoneAsync(SearchPhone.Trim());
            if (customer != null)
            {
                Customers.Add(customer);
                SelectedCustomer = customer;
            }
            else
            {
                await ShowMessageAsync("Info", "Customer not found.");
            }
        }

        private async Task DeleteCustomerAsync(Customer customer)
        {
            if (customer == null) return;

            var dialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete this customer?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await _customerDao.DeleteCustomerByPhoneAsync(customer.Phone);
                Customers.Remove(customer);
                if (SelectedCustomer == customer)
                {
                    SelectedCustomer = null;
                }
                await ShowMessageAsync("Success", "Customer deleted.");
            }
        }

        private async Task EditCustomerAsync(Customer customer)
        {
            if (customer == null) return;

            var editDialog = new ContentDialog
            {
                Title = "Edit Customer",
                Content = new EditCustomerControl(customer),
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await editDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                await _customerDao.UpdateAsync(customer);
                await ShowMessageAsync("Success", "Customer updated.");
                SelectedCustomer = customer;
            }
        }

        private void ClearRegistrationFields()
        {
            FullName = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            Address = string.Empty;
        }

        private async Task ShowMessageAsync(string title, string message)
        {
            if (XamlRoot == null)
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}