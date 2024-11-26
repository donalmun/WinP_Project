using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp
{
    public class Detail : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public float Unit_Price { get; set; }
        public float Sub_Total { get; set; }
        public int Order_Id { get; set; }
        public int Product_Id { get; set; }
        public DateTime Created_At { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}



