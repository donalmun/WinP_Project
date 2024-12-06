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
                
                Status = Convert.ToInt32(reader["status"])
            };
        }
       
        public override async Task<Table> UpdateAsync(Table table)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("UPDATE `Table` SET table_name = @TableName, status = @Status WHERE id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@TableName", table.Table_Name);
                    command.Parameters.AddWithValue("@Status", table.Status);
                    command.Parameters.AddWithValue("@Id", table.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }
            return table;
        }

        // TableDAO.cs
        public async Task<IEnumerable<Table>> SearchTablesByNameAsync(string nameQuery)
        {
            var tables = new List<Table>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM `Table` WHERE table_name LIKE @NameQuery", connection))
                {
                    command.Parameters.AddWithValue("@NameQuery", $"%{nameQuery}%");
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

        public override async Task<Table> AddAsync(Table table)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("INSERT INTO `Table` (table_name, status) VALUES (@TableName, @Status)", connection))
                {
                    command.Parameters.AddWithValue("@TableName", table.Table_Name);
                    
                    command.Parameters.AddWithValue("@Status", table.Status);

                    await command.ExecuteNonQueryAsync();
                    table.Id = (int)command.LastInsertedId;
                }
            }
            return table;
        }

        public override async Task<Table> DeleteAsync(int id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("DELETE FROM `Table` WHERE id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return null;
        }

    }

}
