//WinP_Project\FoodApp\Service\DataAccess\CustomerDAO.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class CustomerDAO : MySqlDao<Customer>
    {
        public override async Task<Customer> AddAsync(Customer customer)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("INSERT INTO Customer (Full_Name, Phone, Email, Loyalty_Points, Address, Created_At) VALUES (@FullName, @Phone, @Email, @Loyalty_Points, @Address, @CreatedAt)", connection))
                {
                    command.Parameters.AddWithValue("@FullName", customer.Full_Name);
                    command.Parameters.AddWithValue("@Phone", customer.Phone);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Address", customer.Address);
                    command.Parameters.AddWithValue("@CreatedAt", customer.Created_At);
                    command.Parameters.AddWithValue("@Loyalty_Points", 0);

                    await command.ExecuteNonQueryAsync();
                    customer.Id = (int)command.LastInsertedId;
                }
            }

            return customer;
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phone)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM Customer WHERE Phone = @Phone", connection))
                {
                    command.Parameters.AddWithValue("@Phone", phone);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return MapToEntity(reader);
                        }
                    }
                }
            }

            return null;
        }
        public async Task DeleteCustomerByPhoneAsync(string phone)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("DELETE FROM Customer WHERE Phone = @Phone", connection))
                {
                    command.Parameters.AddWithValue("@Phone", phone);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<Customer> UpdateAsyncByPhoneAsync(Customer customer)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("UPDATE Customer SET Full_Name = @FullName, Email = @Email, Address = @Address, Loyalty_Points = @LoyaltyPoints WHERE Phone = @Phone", connection))
                {
                    command.Parameters.AddWithValue("@FullName", customer.Full_Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Address", customer.Address);
                    command.Parameters.AddWithValue("@LoyaltyPoints", customer.Loyalty_Points);
                    command.Parameters.AddWithValue("@Phone", customer.Phone);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return customer;
        }

        // CustomerDAO.cs
        public override async Task<Customer> UpdateAsync(Customer customer)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("UPDATE Customer SET Full_Name = @FullName, Email = @Email, Address = @Address, Loyalty_Points = @Loyalty_Points WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@FullName", customer.Full_Name);
                    command.Parameters.AddWithValue("@Email", customer.Email);
                    command.Parameters.AddWithValue("@Address", customer.Address);
                    command.Parameters.AddWithValue("@Loyalty_Points", customer.Loyalty_Points);
                    command.Parameters.AddWithValue("@Id", customer.Id);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return customer;
        }


        protected override Customer MapToEntity(IDataReader reader)
        {
            return new Customer
            {
                Id = Convert.ToInt32(reader["Id"]),
                Full_Name = reader["Full_Name"].ToString(),
                Phone = reader["Phone"].ToString(),
                Email = reader["Email"].ToString(),
                Address = reader["Address"].ToString(),
                Loyalty_Points = Convert.ToInt32(reader["Loyalty_Points"]),
                Created_At = reader["Created_At"] as DateTime?
            };
        }

        public async Task<IEnumerable<Customer>> SearchCustomersByPhoneAsync(string phoneQuery)
        {
            var customers = new List<Customer>();
            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand("SELECT * FROM Customer WHERE Phone LIKE @PhoneQuery", connection))
                {
                    command.Parameters.AddWithValue("@PhoneQuery", $"%{phoneQuery}%");
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var customer = MapToEntity(reader);
                            customers.Add(customer);
                        }
                    }
                }
            }
            return customers;
        }
    }
}