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

        public OrderDAO()
        {
            _detailDao = new DetailDAO();
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

    }
}
