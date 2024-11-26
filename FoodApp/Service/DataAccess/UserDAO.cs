// WinP_Project\FoodApp\Service\DataAccess\UserDAO.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class UserDao : MySqlDao<Users>
    {
        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            Users user = null;

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = MapToEntity(reader);
                        }
                    }
                }
            }
            return user;
        }

        protected override Users MapToEntity(IDataReader reader)
        {
            return new Users
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                Created_At = Convert.ToDateTime(reader["Created_At"]),
            };
        }
    }
}