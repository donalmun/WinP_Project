using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;

namespace FoodApp;

public class Users : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public DateTime Created_At { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
