using FoodApp;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace FoodApp;

public class Users : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime Created_At { get; set; }

    public ICollection<Order> Orders { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
