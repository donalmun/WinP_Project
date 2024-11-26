using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class ProductDao : MySqlDao<Product>
    {
        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            var resultList = new List<Product>();

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM Product", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Product entity = MapToEntity(reader);
                            resultList.Add(entity);
                        }
                    }
                }
            }
            return resultList;
        }

        public override async Task<Product> GetByIdAsync(int id)
        {
            Product entity = default;

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM Product WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            entity = MapToEntity(reader);
                        }
                    }
                }
            }
            return entity;
        }

        public override async Task<Product> AddAsync(Product entity)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("INSERT INTO Product (Name, Description, Cost, Category_Id, Image) VALUES (@Name, @Description, @Cost, @Category_Id, @Image)", connection))
                {
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Cost", entity.Cost);
                    command.Parameters.AddWithValue("@Category_Id", entity.Category_Id);
                    command.Parameters.AddWithValue("@Image", entity.Image);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }
            return entity;
        }

        public override async Task<Product> UpdateAsync(Product entity)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("UPDATE Product SET Name = @Name, Description = @Description, Cost = @Cost, Category_Id = @Category_Id, Image = @Image WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Name", entity.Name);
                    command.Parameters.AddWithValue("@Description", entity.Description);
                    command.Parameters.AddWithValue("@Cost", entity.Cost);
                    command.Parameters.AddWithValue("@Category_Id", entity.Category_Id);
                    command.Parameters.AddWithValue("@Image", entity.Image);
                    command.Parameters.AddWithValue("@Id", entity.Id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return entity;
        }

        public override async Task<Product> DeleteAsync(int id)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("DELETE FROM Product WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            return default;
        }

        protected override Product MapToEntity(IDataReader reader)
        {
            return new Product
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = (string)reader["Description"],
                Cost = Convert.ToSingle(reader["Cost"]),
                Category_Id = (int)reader["Category_Id"],
                Image = (string)reader["Image"],
                Created_At = (DateTime)reader["Created_At"]
            };
        }


    }
}
