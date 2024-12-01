using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class TableDAO : MySqlDao<Table>
    {
        public override async Task<IEnumerable<Table>> GetAllAsync()
        {
            var tables = new List<Table>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM `Table`", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var table = MapToEntity(reader);
                            tables.Add(table);
                        }
                    }
                }
            }
            return tables;
        }

        protected override Table MapToEntity(IDataReader reader)
        {
            return new Table
            {
                Id = Convert.ToInt32(reader["id"]),
                Table_Name = reader["table_name"].ToString(),
                Capacity = Convert.ToInt32(reader["capacity"]),
                Status = Convert.ToByte(reader["status"])
            };
        }
    }
}
