﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;

namespace FoodApp.Service.DataAccess
{
    internal class MockDao<T> : IDao<T>
    {
        private readonly List<T> _data = new List<T>();

        public MockDao()
        {
            if (typeof(T) == typeof(Product))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Product { Id = 1, Name = "Trà Chanh The Mát", Description = "Giải khát ngay cùng trà chanh thơm thanh", Cost = 30000, Image = "/Assets/Trachanh.jpg", Detail_Id = 101, Category_Id = 201,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 2, Name = "Trà sữa chân châu đường đen", Description = "Ngon ngọt từng giọt", Cost = 20000, Image = "/Assets/trasua.jpg", Detail_Id = 102, Category_Id = 202,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Detail_Id = 103, Category_Id = 203,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Detail_Id = 103, Category_Id = 203,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Detail_Id = 103, Category_Id = 203,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 1, Name = "Trà Chanh The Mát", Description = "Giải khát ngay cùng trà chanh thơm thanh", Cost = 30000, Image = "/Assets/Trachanh.jpg", Detail_Id = 101, Category_Id = 201,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 2, Name = "Trà sữa chân châu đường đen", Description = "Ngon ngọt từng giọt", Cost = 20000, Image = "/Assets/trasua.jpg", Detail_Id = 102, Category_Id = 202,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Detail_Id = 103, Category_Id = 203,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Detail_Id = 103, Category_Id = 203,Created_At  = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Detail_Id = 103, Category_Id = 203,Created_At  = DateTime.Now },
                });
            }
            else if (typeof(T) == typeof(Users))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Users { Id = 1, Username = "user1", Password = "pass1", Email = "user1@example.com", Address = "Address 1", Phone = "1234567890", Created_At = DateTime.Now },
                    (T)(object)new Users { Id = 2, Username = "user2", Password = "pass2", Email = "user2@example.com", Address = "Address 2", Phone = "1234567891", Created_At = DateTime.Now }
                });
            }
            else if (typeof(T) == typeof(Order))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Order { Id = 1, Order_Date = DateTime.Now, Cost = 100.0f, Status = "Pending", User_Id = 1, Created_At = DateTime.Now },
                    (T)(object)new Order { Id = 2, Order_Date = DateTime.Now, Cost = 200.0f, Status = "Completed", User_Id = 2, Created_At = DateTime.Now }
                });
            }
            else if (typeof(T) == typeof(Detail))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Detail { Id = 1, Quantity = 2, Cost = 20.0f, Order_Id = 1, Product_Id = 1,Created_At = DateTime.Now },
                    (T)(object)new Detail { Id = 2, Quantity = 1, Cost = 10.0f, Order_Id = 2, Product_Id = 2, Created_At = DateTime.Now }
                });
            }
            else if (typeof(T) == typeof(Category))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Category { Id = 1, Name = "Category 1", Created_At = DateTime.Now },
                    (T)(object)new Category { Id = 2, Name = "Category 2", Created_At = DateTime.Now }
                });
            }
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_data);
        }

        public Task<T> GetByIdAsync(int id)
        {
            var entity = _data.FirstOrDefault(e => (int)e.GetType().GetProperty("Id").GetValue(e) == id);
            return Task.FromResult(entity);
        }

        public Task<T> AddAsync(T entity)
        {
            _data.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(T entity)
        {
            var id = (int)entity.GetType().GetProperty("Id").GetValue(entity);
            var index = _data.FindIndex(e => (int)e.GetType().GetProperty("Id").GetValue(e) == id);
            if (index != -1)
            {
                _data[index] = entity;
            }
            return Task.FromResult(entity);
        }

        public Task<T> DeleteAsync(int id)
        {
            var entity = _data.FirstOrDefault(e => (int)e.GetType().GetProperty("Id").GetValue(e) == id);
            if (entity != null)
            {
                _data.Remove(entity);
            }
            return Task.FromResult(entity);
        }
    }
}
