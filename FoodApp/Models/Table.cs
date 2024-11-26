using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp;

public class Table : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Table_Name { get; set; }
    public int Capacity { get; set; }
    public byte Status { get; set; }

    public ICollection<Order> Orders { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
