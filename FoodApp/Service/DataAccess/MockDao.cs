using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp;

public class MockDao : IDao
{
    public List<Product>GetProducts()
    {
        var result = new List<Product>() {
                new() {
                    Id = 1,
                    Name = "Pizza",
                    Description = "Pizza with tomato and cheese",
                    Cost = 10.5f,
                    Detail_Id = 1,
                    Category_Id = 1,
                    Image = "Assets/pizza.jpg",
                    Created_At = DateTime.Now
                },
                new() {
                    Id = 2,
                    Name = "Hamburger",
                    Description = "Hamburger with meat and cheese",
                    Cost = 8.5f,
                    Detail_Id = 2,
                    Category_Id = 1,
                    Image = "Assets/hamburger.jpg",
                    Created_At = DateTime.Now

                },
                new() {
                    Id = 3,
                    Name = "Coke",
                    Description = "Coke with sugar",
                    Cost = 2.5f,
                    Detail_Id = 3,
                    Category_Id = 2,
                    Image = "Assets/coke.jpg",
                    Created_At = DateTime.Now
                },
            };

        return result;
    }
}

