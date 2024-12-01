using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class DetailDAO : MySqlDao<Detail>
    {
        public async Task<Detail> AddAsync(Detail detail, MySqlConnection connection, MySqlTransaction transaction)
        {
            // Tạo chuỗi bổ sung cho Note nếu có Discount hoặc Surcharge
            string additionalNote = string.Empty;

            if (detail.Discount > 0)
            {
                additionalNote += $"\n Giảm giá: {detail.Discount} {detail.DiscountType}. ";
            }

            if (detail.Surcharge > 0)
            {
                additionalNote += $"\n Phụ phí: {detail.Surcharge} {detail.SurchargeType}. ";
            }

            string insertDetailQuery = @"
                INSERT INTO Detail (Quantity, Unit_Price, Sub_Total, Note, Order_Id, Product_Id)
                VALUES (@Quantity, @UnitPrice, @SubTotal, @Note, @OrderId, @ProductId);
                SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(insertDetailQuery, connection, transaction);
            cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
            cmd.Parameters.AddWithValue("@UnitPrice", detail.Unit_Price);
            cmd.Parameters.AddWithValue("@SubTotal", detail.Sub_Total);
            cmd.Parameters.AddWithValue("@Note", detail.Note + additionalNote ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderId", detail.Order_Id);
            cmd.Parameters.AddWithValue("@ProductId", detail.Product_Id);

            detail.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return detail;
        }

        public override async Task<IEnumerable<Detail>> GetAllAsync()
        {
            var details = new List<Detail>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM details", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            details.Add(MapToEntity(reader));
                        }
                    }
                }
            }

            return details;
        }

        protected override Detail MapToEntity(IDataReader reader)
        {
            return new Detail
            {
                Id = Convert.ToInt32(reader["Id"]),
                Quantity = Convert.ToInt32(reader["Quantity"]),
                Unit_Price = Convert.ToSingle(reader["Unit_Price"]),
                Sub_Total = Convert.ToSingle(reader["Sub_Total"]),
                Order_Id = Convert.ToInt32(reader["Order_Id"]),
                Product_Id = Convert.ToInt32(reader["Product_Id"]),
                // Assuming Product and Order are loaded separately
            };
        }

        // Implement other CRUD operations as needed
    }
}