using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp.Models
{
    public class Product : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Cost { get; set; }
        public int Detail_Id { get; set; }
        public int Category_Id { get; set; }
        public string Image { get; set; }
        public DateTime Created_At { get; set; }
        public ICollection<Detail> Details { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}



