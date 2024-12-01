using FoodApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp;

public class Customer : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Full_Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    private int _loyaltyPoints;
    public int Loyalty_Points
    {
        get => _loyaltyPoints;
        set
        {
            _loyaltyPoints = value;
            OnPropertyChanged(nameof(Loyalty_Points));
        }
    }
    public DateTime? Created_At { get; set; }

    public ICollection<Order> Orders { get; set; }

    public override string ToString()
    {
        return $"{Full_Name} - {Phone}";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
