using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace FoodApp;

public class Product : INotifyPropertyChanged
{
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _id;
    public int Id
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged();
            }
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    private string _description;    
    public string Description 
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged();
            }
        }
    }

    private float _cost;
    public float Cost 
    { 
        get => _cost;
        set
        {
            if (_cost != value)
            {
                _cost = value;
                OnPropertyChanged();
            }
        }
    }

    private int _category_Id;
    public int Category_Id 
    { 
        get => _category_Id;
        set
        {
            if (_category_Id != value)
            {
                _category_Id = value;
                OnPropertyChanged();
            }
        }
    }
    
    public string Image { get; set; }
    public DateTime Created_At { get; set; }

    // Add this property
    public ICollection<Detail> Details { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}