// TableManagementViewModel.cs
using FoodApp.Helper;
using FoodApp.Service.DataAccess;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FoodApp.Views.Controls;
using FoodApp.Views;
using System.Linq;

namespace FoodApp.ViewModels
{
    public class TableManagementViewModel : BindableBase
    {
        private readonly OrderDAO _orderDao;
        private readonly TableDAO _tableDao;
        private string _searchTableName;
        private Table _selectedTable;
        private string _tableName;
        private int _status;
        private Order _selectedOrder;

        public TableManagementViewModel()
        {
            _orderDao = new OrderDAO();
            _tableDao = new TableDAO();
            Tables = new ObservableCollection<Table>();
            LoadTablesCommand = new AsyncRelayCommand(LoadTablesAsync);
            AddTableCommand = new AsyncRelayCommand(AddTableAsync);
            EditTableCommand = new AsyncRelayCommand<Table>(EditTableAsync);
            DeleteTableCommand = new AsyncRelayCommand<Table>(DeleteTableAsync);
            SearchCommand = new AsyncRelayCommand(SearchAsync);
            ViewDetailsCommand = new AsyncRelayCommand<Table>(ViewDetailsAsync);

            // Load tables on initialization
            LoadTablesCommand.Execute(null);
        }

        public ObservableCollection<Table> Tables { get; }

        public string SearchTableName
        {
            get => _searchTableName;
            set => SetProperty(ref _searchTableName, value);
        }

        public Table SelectedTable
        {
            get => _selectedTable;
            set => SetProperty(ref _selectedTable, value);
        }

        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }

        public int Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        public ICommand LoadTablesCommand { get; }
        public ICommand AddTableCommand { get; }
        public ICommand EditTableCommand { get; }
        public ICommand DeleteTableCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ViewDetailsCommand { get; } // Added

        public XamlRoot XamlRoot { get; set; }

        private async Task LoadTablesAsync()
        {
            Tables.Clear();
            var tables = await _tableDao.GetAllAsync();
            foreach (var table in tables)
            {
                Tables.Add(table);
            }
        }

        private async Task AddTableAsync()
        {
            var newTable = new Table
            {
                Table_Name = TableName,
                Status = Status
            };

            await _tableDao.AddAsync(newTable);
            Tables.Add(newTable);

            ClearFields();
            await ShowMessageAsync("Success", "Table added successfully.");
        }

        private async Task EditTableAsync(Table table)
        {
            if (table == null) return;

            // Debug output to log the current status
            System.Diagnostics.Debug.WriteLine($"Editing Table: {table.Table_Name}, Status: {table.Status}");

            var dialog = new ContentDialog
            {
                Title = "Edit Table",
                PrimaryButtonText = "Save",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot
            };

            var editControl = new EditTableControl(table);
            dialog.Content = editControl;

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                System.Diagnostics.Debug.WriteLine($"After Editing - Table: {editControl.UpdatedTable.Table_Name},  Status: {editControl.UpdatedTable.Status}");

                await _tableDao.UpdateAsync(editControl.UpdatedTable);
                await LoadTablesAsync();
                await ShowMessageAsync("Success", "Table updated successfully.");
            }
        }

        private async Task DeleteTableAsync(Table table)
        {
            if (table == null) return;

            var dialog = new ContentDialog
            {
                Title = "Confirm Deletion",
                Content = "Are you sure you want to delete this table?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = XamlRoot
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                await _tableDao.DeleteAsync(table.Id);
                Tables.Remove(table);
                await ShowMessageAsync("Success", "Table deleted successfully.");
            }
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTableName))
            {
                await LoadTablesAsync();
            }
            else
            {
                Tables.Clear();
                var tables = await _tableDao.SearchTablesByNameAsync(SearchTableName);
                foreach (var table in tables)
                {
                    Tables.Add(table);
                }
            }
        }

        private async Task ViewDetailsAsync(Table table)
        {
            if (table == null) return;

            await LoadOrderDetailsAsync(table);

            if (SelectedOrder != null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Order Details",
                    Content = new OrderDetailsControl { DataContext = SelectedOrder },
                    CloseButtonText = "Close",
                    XamlRoot = XamlRoot
                };

                await dialog.ShowAsync();
            }
            else
            {
                await ShowMessageAsync("No Order", "No active order found for this table.");
            }
        }

        private async Task LoadOrderDetailsAsync(Table table)
        {
            if (table == null || table.Status == 0) // Assuming 0 means empty
            {
                SelectedOrder = null;
                return;
            }

            // Fetch the orders for the table where the status indicates active
            var orders = await _orderDao.GetOrdersByTableIdAsync(table.Id);

            // Assuming the most recent order is the active one
            var activeOrder = orders?.OrderByDescending(o => o.Order_Date).FirstOrDefault(o => o.Status != 0);

            if (activeOrder != null)
            {
                await _orderDao.LoadOrderDetailsAsync(activeOrder);
                SelectedOrder = activeOrder;
            }
            else
            {
                SelectedOrder = null;
            }
        }


        private void ClearFields()
        {
            TableName = string.Empty;
            Status = 0;
        }

        private async Task ShowMessageAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
