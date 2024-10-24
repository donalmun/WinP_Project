using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp;

public class MainViewModel
{
    public List<Product> Products { get; set; }

    public MainViewModel()
    {
        IDao dao = new MockDao();
        Products = dao.GetProducts();
    }
}
