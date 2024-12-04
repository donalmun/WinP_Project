// OrderDAO.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class OrderDAO : MySqlDao<Order>
    {
        private readonly DetailDAO _detailDao;

        public OrderDAO()
        {
            _detailDao = new DetailDAO();
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

        // Existing AddAsync method...
    }
}