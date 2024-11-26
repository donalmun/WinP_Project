using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace FoodApp.Service.DataAccess
{
    public class MySqlDao<T> : IDao<T>
    {
        private readonly string _connectionString;

        public MySqlDao()
        {
            _connectionString = """
                Server=localhost;
                Port=3307;
                Database=foodapp_db;
                User=root;
                Password=admin1234; 
             """;
        }

        protected MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Database connection successful.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection failed: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<T> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        protected virtual T MapToEntity(IDataReader reader)
        {
            throw new NotImplementedException("MapToEntity needs to be implemented.");
        }
    }
}
