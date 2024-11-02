using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp.Models
{
    public class Detail : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public float Cost { get; set; }
        public int Order_Id { get; set; }
        public int Product_Id { get; set; }
        public DateTime Created_At { get; set; }
        public ICollection<Detail> Details { get; set; }

        public Product Product { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}


