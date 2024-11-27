using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class DetailDAO : MySqlDao<Detail>
    {
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
                Created_At = Convert.ToDateTime(reader["Created_At"]),
                // Assuming Product and Order are loaded separately
            };
        }

        // Implement other CRUD operations as needed
    }
}