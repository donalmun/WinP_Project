using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    (T)(object)new Product { Id = 1, Name = "Trà Chanh The Mát", Description = "Giải khát ngay cùng trà chanh thơm thanh", Cost = 30000, Image = "/Assets/Trachanh.jpg", Category_Id = 1, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 2, Name = "Trà sữa chân châu đường đen", Description = "Ngon ngọt từng giọt", Cost = 20000, Image = "/Assets/trasua.jpg", Category_Id = 2, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Category_Id = 3, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 4, Name = "Mỳ ý", Description = "Mỳ ý sốt bò bằm", Cost = 40000, Image = "/Assets/myy.png", Category_Id = 4, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 5, Name = "Bánh mì", Description = "Bánh mì thịt nguội", Cost = 20000, Image = "/Assets/banhmi.jpg", Category_Id = 5, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 6, Name = "Cơm gà", Description = "Cơm gà sốt cay", Cost = 30000, Image = "/Assets/comga.jpg", Category_Id = 6, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 7, Name = "Cơm chiên", Description = "Cơm chiên thập cẩm", Cost = 25000, Image = "/Assets/comchien.jpg", Category_Id = 7, Created_At = DateTime.Now },
                    (T)(object)new Product { Id = 8, Name = "Bún riêu", Description = "Bún riêu cua", Cost = 30000, Image = "/Assets/bunrieu.jpg", Category_Id = 8, Created_At = DateTime.Now },
                });
            }
            else if (typeof(T) == typeof(Users))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Users { Id = 1, Username = "user1", Password = "pass1", Created_At = DateTime.Now, Orders = new List<Order>() },
                    (T)(object)new Users { Id = 2, Username = "user2", Password = "pass2", Created_At = DateTime.Now, Orders = new List<Order>() }
                });
            }
            else if (typeof(T) == typeof(Customer))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Customer { Id = 1, Full_Name = "Nguyen Van A", Phone = "0901234567", Email = "a@example.com", Address = "123 Đường A", Loyalty_Points = 100, Created_At = DateTime.Now, Orders = new List<Order>() },
                    (T)(object)new Customer { Id = 2, Full_Name = "Tran Thi B", Phone = "0907654321", Email = "b@example.com", Address = "456 Đường B", Loyalty_Points = 150, Created_At = DateTime.Now, Orders = new List<Order>() }
                });
            }
            else if (typeof(T) == typeof(Order))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Order { Id = 1, Order_Date = DateTime.Now, Total_Amount = 100000, Status = 0, Customer_Id = 1, Table_Id = 1, Created_At = DateTime.Now, Details = new List<Detail>(), Payments = new List<Payment>() },
                    (T)(object)new Order { Id = 2, Order_Date = DateTime.Now, Total_Amount = 200000, Status = 1, Customer_Id = 2, Table_Id = 2, Created_At = DateTime.Now, Details = new List<Detail>(), Payments = new List<Payment>() }
                });
            }
            else if (typeof(T) == typeof(Detail))
            {
                var orders = new List<Order>
                {
                    new Order { Id = 1, Order_Date = DateTime.Now, Total_Amount = 100000, Status = 0, Customer_Id = 1, Table_Id = 1, Created_At = DateTime.Now },
                    new Order { Id = 2, Order_Date = DateTime.Now, Total_Amount = 200000, Status = 1, Customer_Id = 2, Table_Id = 2, Created_At = DateTime.Now }
                };

                var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Trà Chanh The Mát", Description = "Giải khát ngay cùng trà chanh thơm thanh", Cost = 30000, Image = "/Assets/Trachanh.jpg", Category_Id = 1, Created_At = DateTime.Now },
                    new Product { Id = 2, Name = "Trà sữa chân châu đường đen", Description = "Ngon ngọt từng giọt", Cost = 20000, Image = "/Assets/trasua.jpg", Category_Id = 2, Created_At = DateTime.Now },
                    new Product { Id = 3, Name = "Sinh tố dâu", Description = "Cung cấp dinh dưỡng tươi ngon", Cost = 30000, Image = "/Assets/sinhto.jpg", Category_Id = 3, Created_At = DateTime.Now },
                };

                //_data.AddRange(new List<T>
                //{
                //    (T)(object)new Detail { Id = 1, Quantity = 2, Unit_Price = 30000, Sub_Total = 60000, Order_Id = 1, Product_Id = 1, Created_At = DateTime.Now, Product = products[0], Order = orders[0] },
                //    (T)(object)new Detail { Id = 2, Quantity = 1, Unit_Price = 20000, Sub_Total = 20000, Order_Id = 2, Product_Id = 2, Created_At = DateTime.Now, Product = products[1], Order = orders[1] }
                //});
            }
            else if (typeof(T) == typeof(Category))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Category { Id = 1, Name = "Thức Uống", Created_At = DateTime.Now, Products = new List<Product>() },
                    (T)(object)new Category { Id = 2, Name = "Món Ăn", Created_At = DateTime.Now, Products = new List<Product>() }
                });
            }
            else if (typeof(T) == typeof(Payment))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Payment { Id = 1, Order_Id = 1, Payment_Method = "Cash", Payment_Date = DateTime.Now, Amount_Paid = 100000m, Order = ((MockDao<Order>)new MockDao<Order>()).GetByIdAsync(1).Result },
                    (T)(object)new Payment { Id = 2, Order_Id = 2, Payment_Method = "Credit Card", Payment_Date = DateTime.Now, Amount_Paid = 200000m, Order = ((MockDao<Order>)new MockDao<Order>()).GetByIdAsync(2).Result }
                });
            }
            else if (typeof(T) == typeof(Table))
            {
                _data.AddRange(new List<T>
                {
                    (T)(object)new Table { Id = 1, Table_Name = "A1", Capacity = 4, Status = 0 },
                    (T)(object)new Table { Id = 2, Table_Name = "B1", Capacity = 2, Status = 1 }
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

        public Task<bool> TestConnectionAsync()
        {
            throw new NotImplementedException();
        }
    }
}