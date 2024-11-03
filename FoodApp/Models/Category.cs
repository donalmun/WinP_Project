using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp;
public class Category : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Created_At { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}