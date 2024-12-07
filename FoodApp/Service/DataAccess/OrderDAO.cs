// OrderDAO.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class OrderDAO : MySqlDao<Order>
    {
        private readonly DetailDAO _detailDao;
        private readonly ProductDao _productDao;
        private readonly CustomerDAO _customerDao;

        public OrderDAO()
        {
            _detailDao = new DetailDAO();
            _productDao = new ProductDao();
            _customerDao = new CustomerDAO();
        }

        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            var orders = new List<Order>();
            using var connection = GetConnection();
            await connection.OpenAsync();

            string query = "SELECT * FROM Orders";

            using var cmd = new MySqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var order = new Order
                {
                    Id = reader.GetInt32("Id"),
                    Order_Date = reader.GetDateTime("order_date"),
                    Total_Amount = reader.GetFloat("Total_Amount"),
                    Status = reader.GetByte("Status"),
                    Customer_Id = reader.IsDBNull(reader.GetOrdinal("Customer_Id")) ? (int?)null : reader.GetInt32("Customer_Id"),
                    Table_Id = reader.IsDBNull(reader.GetOrdinal("Table_Id")) ? (int?)null : reader.GetInt32("Table_Id")
                };
                orders.Add(order);
            }

            return orders;
        }

        public override async Task<Order> AddAsync(Order order)
        {
            using var connection = GetConnection();
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                string insertOrderQuery = @"
                    INSERT INTO Orders (order_date, Total_Amount, Status, Customer_Id, Table_Id)
                    VALUES (@OrderDate, @TotalAmount, @Status, @CustomerId, @TableId);
                    SELECT LAST_INSERT_ID();";

                using var cmd = new MySqlCommand(insertOrderQuery, connection, transaction);
                cmd.Parameters.AddWithValue("@OrderDate", order.Order_Date);
                cmd.Parameters.AddWithValue("@TotalAmount", order.Total_Amount);
                cmd.Parameters.AddWithValue("@Status", order.Status);
                cmd.Parameters.AddWithValue("@CustomerId", order.Customer_Id ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TableId", order.Table_Id ?? (object)DBNull.Value);

                order.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                foreach (var detail in order.Details)
                {
                    detail.Order_Id = order.Id;
                    await _detailDao.AddAsync(detail, connection, transaction);
                }

                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task LoadOrderDetailsAsync(Order order)
        {
            if (order == null) return;

            var details = new List<Detail>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Detail WHERE Order_Id = @OrderId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", order.Id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var detail = new Detail
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Order_Id = Convert.ToInt32(reader["Order_Id"]),
                                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                                Quantity = Convert.ToInt32(reader["Quantity"]),
                                Unit_Price = Convert.ToSingle(reader["Unit_Price"]),
                                Sub_Total = Convert.ToSingle(reader["Sub_Total"]),
                                Note = reader["Note"].ToString()
                            };

                            // Load product details
                            detail.Product = await _productDao.GetByIdAsync(detail.Product_Id);

                            details.Add(detail);
                        }
                    }
                }
                // Load customer details if Customer_Id is not null
                if (order.Customer_Id.HasValue)
                {
                    order.Customer = await _customerDao.GetByIdAsync(order.Customer_Id.Value);
                }

            }

            order.Details = details;
        }

        public async Task<List<Order>> GetOrdersByTableIdAsync(int tableId)
        {
            var orders = new List<Order>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                string query = @"
                    SELECT o.*, c.*
                    FROM `Orders` o
                    LEFT JOIN Customer c ON o.Customer_Id = c.Id
                    WHERE o.Table_Id = @TableId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TableId", tableId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var order = MapToEntity(reader);
                            orders.Add(order);
                        }
                    }
                }
            }
            return orders;
        }

        protected override Order MapToEntity(IDataReader reader)
        {
            var order = new Order
            {
                Id = Convert.ToInt32(reader["Id"]),
                Order_Date = Convert.ToDateTime(reader["Order_Date"]),
                Total_Amount = Convert.ToSingle(reader["Total_Amount"]),
                Status = Convert.ToByte(reader["Status"]),
                Customer_Id = reader["Customer_Id"] as int?,
                Table_Id = reader["Table_Id"] as int?,
                // Initialize Details as empty list
                Details = new List<Detail>()
            };

            // Map Customer data if present
            if (!reader.IsDBNull(reader.GetOrdinal("Customer_Id")))
            {
                order.Customer = new Customer
                {
                    Id = Convert.ToInt32(reader["Customer_Id"]),
                    Full_Name = reader["Full_Name"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    Address = reader["Address"].ToString(),
                    Loyalty_Points = Convert.ToInt32(reader["Loyalty_Points"]),
                    Created_At = Convert.ToDateTime(reader["Created_At"])
                };
            }


            return order;
        }


    }
}